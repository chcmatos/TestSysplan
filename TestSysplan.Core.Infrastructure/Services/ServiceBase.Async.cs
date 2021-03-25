using Microsoft.EntityFrameworkCore;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;
using TestSysplan.Core.Infrastructure.Context;
using TestSysplan.Core.Model;

namespace TestSysplan.Core.Infrastructure.Services
{
    internal abstract partial class ServiceBase<Entity, Context> : IServiceBaseAsync<Entity>
        where Entity : ModelBase, new()
        where Context : ContextBase
    {
        #region [C]reate
        public async Task<Entity> InsertAsync(Entity entity, CancellationToken cancellationToken)
        {
            await dbSet.AddAsync(entity ?? throw new ArgumentNullException(nameof(entity)), cancellationToken);
            await dbContext.SaveChangesAsync(cancellationToken);
            OnInsertedCallback(entity);
            return entity;
        }
        #endregion

        #region [R]ead
        public Task<bool> ExistsAsync(Guid uuid, CancellationToken cancellationToken)
        {
            return dbSet
                .AsNoTracking()
                .AnyAsync(e => e.Uuid == uuid, cancellationToken);
        }

        public Task<bool> ExistsAsync(Entity e, CancellationToken cancellationToken)
        {
            return dbSet
                .AsNoTracking()
                .AnyAsync(c => c.Uuid == e.Uuid, cancellationToken);
        }

        public async Task<Entity> GetAsync(long id, CancellationToken cancellationToken)
        {
            Entity found = await dbSet.FindAsync(new object[] { id }, cancellationToken);
            if (found != null) dbContext.Entry(found).State = EntityState.Detached;
            return found;
        }

        public Task<Entity> GetAsync(Guid uuid, CancellationToken cancellationToken)
        {
            return dbSet
                .AsNoTracking()
                .Where(t => t.Uuid == uuid)
                .OrderBy(e => e.Id)
                .Take(1)
                .FirstOrDefaultAsync(cancellationToken);
        }

        public Task<List<Entity>> PagingIndexAsync(int index, int count, CancellationToken cancellationToken)
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
                .ToListAsync(cancellationToken);
        }

        public Task<List<Entity>> PagingAsync(int page, int limit, CancellationToken cancellationToken)
        {
            page = page < 0 ? 0 : page;
            limit = limit <= 0 ? LIST_LIMIT : limit;
            int index = page * limit;//to skip.
            return PagingIndexAsync(index, limit, cancellationToken);
        }

        public Task<List<Entity>> ListAsync(CancellationToken cancellationToken)
        {
            return PagingIndexAsync(0, LIST_LIMIT, cancellationToken);
        }
        #endregion
        
        #region [U]pdate
        public async Task<Entity> UpdateAsync(Entity entity, CancellationToken cancellationToken)
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

            Entity curr = dbSet.Local
                .AsParallel()
                .FirstOrDefault(entity.EqualsAnyId);

            if (curr != null)
            {
                dbContext.Entry(curr)
                    .CurrentValues
                    .SetValues(entity);
            }
            else if (entity.Id == default)
            {
                curr = await dbSet
                   .Where(t => t.Uuid == entity.Uuid)
                   .OrderBy(e => e.Id)
                   .Take(1)
                   .FirstOrDefaultAsync(cancellationToken);

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

            await dbContext.SaveChangesAsync(cancellationToken);
            OnUpdatedCallback(entity);
            return entity;
        }
        #endregion

        #region [D]elete
        private async Task<List<Entity>> AttachRangeNonExistsAsync(IEnumerable<Entity> entity, CancellationToken cancellationToken)
        {
            List<Entity> result = new List<Entity>();

            foreach (Entity e in entity)
            {
                Entity curr = dbSet.Local.FirstOrDefault(e.EqualsAnyId);
                if (curr != null || (e.Id == default && (curr = await GetAsync(e.Uuid, cancellationToken)) != null))
                {
                    result.Add(curr);
                }
                else if (e.Id == default)
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
                    result.Add(e);
                }
            }

            return result;
        }

        private async Task<int> DeleteLocalAsync(IEnumerable<Guid> uuids, CancellationToken cancellationToken)
        {
            var aux = uuids.Select(i => new Entity() { Uuid = i });
            var entity = await AttachRangeNonExistsAsync(aux, cancellationToken);
            dbSet.RemoveRange(entity);
            return await dbContext.SaveChangesAsync(cancellationToken)                
                .ContinueWith(t =>
                {
                    int res = Math.Min(t.Result, entity.Count);
                    if (res > 0) OnDeletedCallback(entity);
                    return res;
                }, TaskContinuationOptions.OnlyOnRanToCompletion);
        }

        public Task<int> DeleteAsync(IEnumerable<Guid> uuids, CancellationToken cancellationToken)
        {
            return DeleteLocalAsync(uuids, cancellationToken);
        }

        public Task<int> DeleteAsync(Guid[] args, CancellationToken cancellationToken)
        {
            return DeleteLocalAsync(args, cancellationToken);
        }

        public Task<int> DeleteAsync(params Guid[] args)
        {
            return DeleteLocalAsync(args, default);
        }

        public Task<bool> DeleteAsync(Guid uuid, CancellationToken cancellationToken)
        {
            return DeleteLocalAsync(new Guid[] { uuid }, cancellationToken).ContinueWith(t => t.Result == 1);
        }

        public async Task<bool> DeleteAsync(IEnumerable<Entity> entity, CancellationToken cancellationToken)
        {
            var att = await AttachRangeNonExistsAsync(entity, cancellationToken);
            dbSet.RemoveRange(att);
            var res = await dbContext.SaveChangesAsync(cancellationToken) >= entity.Count();
            if (res) OnDeletedCallback(att);
            return res;
        }

        public async Task<bool> DeleteAsync(Entity[] entity, CancellationToken cancellationToken)
        {
            var att = await AttachRangeNonExistsAsync(entity, cancellationToken);
            dbSet.RemoveRange(att);
            var res = await dbContext.SaveChangesAsync(cancellationToken) >= entity.Length;
            if (res) OnDeletedCallback(att);
            return res;
        }

        public Task<bool> DeleteAsync(params Entity[] entity)
        {
            return DeleteAsync(entity, default);
        }
        #endregion

    }
}
