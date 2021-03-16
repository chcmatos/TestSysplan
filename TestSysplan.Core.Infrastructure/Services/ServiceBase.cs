using Microsoft.EntityFrameworkCore;
using System;
using System.Collections.Generic;
using System.Linq;
using TestSysplan.Core.Infrastructure.Context;
using TestSysplan.Core.Model;

namespace TestSysplan.Core.Infrastructure.Services
{
    internal abstract class ServiceBase<Entity, Context> : IServiceBase<Entity>
        where Entity : ModelBase, new()
        where Context : ContextBase
    {
        private const int LIST_LIMIT = 250;

        protected readonly Context dbContext;
        protected readonly DbSet<Entity> dbSet;

        public ServiceBase(Context context, DbSet<Entity> dbSet)
        {
            this.dbContext  = context ?? throw new ArgumentNullException(nameof(context));
            this.dbSet      = dbSet ?? throw new ArgumentNullException(nameof(dbSet));
        }

        #region [C]reate
        public Entity Insert(Entity entity)
        {
            dbSet.Add(entity ?? throw new ArgumentNullException(nameof(entity)));
            dbContext.SaveChanges();            
            return entity;
        }
        #endregion

        #region [R]ead
        public bool Exists(Guid uuid)
        {
            return dbSet
                .AsNoTracking()
                .Any(e => e.Uuid == uuid);
        }

        public bool Exists(Entity e)
        {
            return dbSet
                .AsNoTracking()
                .Any(c => c.Uuid == e.Uuid);
        }

        public Entity Get(long id)
        {
            Entity found = dbSet.Find(id);
            if(found != null) dbContext.Entry(found).State = EntityState.Detached;
            return found;
        }

        public Entity Get(Guid uuid)
        {
            return dbSet
                .AsNoTracking()
                .Where(t => t.Uuid == uuid)
                .OrderBy(e => e.Id)
                .Take(1)
                .FirstOrDefault();
        }

        public List<Entity> PagingIndex(int index, int count)
        {
            if (index < 0)
            {
                throw new IndexOutOfRangeException();
            }
            else if (count <= 0)
            {
                throw new ArgumentOutOfRangeException(nameof(count));
            }

            return this.dbSet
                .AsNoTracking()
                .OrderBy(e => e.Id)
                .Skip(index)
                .Take(count)
                .ToList();
        }

        public List<Entity> Paging(int page, int limit)
        {
            page = page < 0 ? 0 : page;
            limit = limit <= 0 ? LIST_LIMIT : limit;
            int index = page * limit;//to skip.
            return PagingIndex(index, limit);
        }

        public List<Entity> List()
        {
            return PagingIndex(0, LIST_LIMIT);
        }
        #endregion

        #region [U]pdate
        public Entity Update(Entity entity)
        {
            if (entity is null)
            {
                throw new ArgumentNullException(nameof(entity));
            }
            else if (entity.Id == default && entity.Uuid == default)
            {
                throw new InvalidOperationException($"Entity \"{typeof(Entity).Name}\" is untrackable, " +
                    "thus can not be updated! " +
                    "Because it does not contains an Id or Uuid.");
            }

            Entity curr = dbSet.Local.FirstOrDefault(entity.EqualsAnyId);

            if (curr != null)
            {
                dbContext.Entry(curr)
                    .CurrentValues
                    .SetValues(entity);
            }
            else if(entity.Id == default)
            {
                curr = dbSet
                   .Where(t => t.Uuid == entity.Uuid)
                   .OrderBy(e => e.Id)
                   .Take(1)
                   .FirstOrDefault();

                if (curr == null)
                {
                    throw new InvalidOperationException($"Entity \"{typeof(Entity).Name}\" with Uuid \"{entity.Uuid}\" does not exist on database!");
                }

                entity.Id = curr.Id;
                dbContext.Entry(curr)
                    .CurrentValues
                    .SetValues(entity);

            }
            else
            {
                dbContext.Entry(entity).State = EntityState.Modified;
            }

            dbContext.SaveChanges();
            return entity;
        }
        #endregion

        #region [D]elete
        private IEnumerable<Entity> AttachRangeNonExists(IEnumerable<Entity> entity)
        {
            foreach(Entity e in entity)
            {
                Entity curr = dbSet.Local.FirstOrDefault(e.EqualsAnyId);                
                if(curr != null || (e.Id == default && (curr = Get(e.Uuid)) != null))
                {
                    yield return curr;
                }
                else if(e.Id == default)
                {
                    #if DEBUG
                    throw new InvalidOperationException($"Entity \"{typeof(Entity).Name}\" is untrackable, " +
                        $"thus can not be attached to delete!");
                    #else
                    continue;
                    #endif
                }
                else
                {
                    yield return e;
                }
            }

        }

        public int Delete(IEnumerable<Guid> id)
        {
            var entity = AttachRangeNonExists(id.Select(i => new Entity() { Uuid = i })).ToList();
            dbSet.RemoveRange(entity);
            return Math.Min(dbContext.SaveChanges(), entity.Count);
        }

        public int Delete(params Guid[] uuid)
        {
            var entity = AttachRangeNonExists(uuid.Select(i => new Entity() { Uuid = i })).ToList();
            dbSet.RemoveRange(entity);
            return Math.Min(dbContext.SaveChanges(), entity.Count);
        }

        public bool Delete(Guid uuid)
        {
            return Delete(new Guid[] { uuid }) == 1;
        }

        public bool Delete(IEnumerable<Entity> entity)
        {
            dbSet.RemoveRange(AttachRangeNonExists(entity));
            return dbContext.SaveChanges() >= entity.Count();
        }

        public bool Delete(params Entity[] entity)
        {
            dbSet.RemoveRange(AttachRangeNonExists(entity));
            return dbContext.SaveChanges() >= entity.Length;
        }
        #endregion

    }
}
