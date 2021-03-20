using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.ChangeTracking;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;
using TestSysplan.Core.Model;

namespace TestSysplan.Core.Infrastructure.Context
{
    internal abstract class ContextBase : DbContext
    {
        protected readonly string SchemaName;

        protected ContextBase(DbContextOptions options, string schemaName) : base(options) 
        {
            this.SchemaName = schemaName;
            Database.EnsureCreated();
        }

        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {
            modelBuilder.HasDefaultSchema(SchemaName);
            base.OnModelCreating(modelBuilder);
        }

        protected virtual void OnPrevSaveChanges(IEnumerable<EntityEntry> entries) { }
        
        private void OnPrevSaveChanges()
        {
            this.OnAuditEntity(ChangeTracker
               .Entries()
               .Where(e => 
                    e.State == EntityState.Added || 
                    e.State == EntityState.Modified));

            this.OnPrevSaveChanges(ChangeTracker.Entries());
        }

        private void OnAuditEntity(IEnumerable<EntityEntry> entries)
        {
            DateTime now = DateTime.Now;
            
            foreach (EntityEntry entry in entries)
            {
                if(entry.Entity is ModelBase model)
                {
                    switch (entry.State)
                    {
                        case EntityState.Added when model is IAudit audit:
                            audit.Created = now;
                            audit.Updated = null;   
                            break;
                        case EntityState.Modified when model is IAudit audit:
                            audit.Updated = now;
                            entry.Property(nameof(IAudit.Created)).IsModified = false;
                            goto case EntityState.Modified;
                        case EntityState.Modified:
                            entry.Property(nameof(ModelBase.Id)).IsModified = false;
                            entry.Property(nameof(ModelBase.Uuid)).IsModified = false;
                            break;
                        default:
                            entry.Property(nameof(IAudit.Created)).IsModified = false;
                            entry.Property(nameof(IAudit.Updated)).IsModified = false;
                            break;
                    }
                }
            }
        }

        #region SaveChanges
        public override int SaveChanges(bool acceptAllChangesOnSuccess)
        {
            this.OnPrevSaveChanges();
            return base.SaveChanges(acceptAllChangesOnSuccess);
        }

        public override Task<int> SaveChangesAsync(bool acceptAllChangesOnSuccess, CancellationToken cancellationToken = default)
        {
            return Task.Factory.StartNew(this.OnPrevSaveChanges)
                .ContinueWith(r => base.SaveChangesAsync(acceptAllChangesOnSuccess, cancellationToken), cancellationToken)
                .Unwrap();
        }
        #endregion

        protected internal void DetachAllEntities()
        {
            if (this.ChangeTracker.AutoDetectChangesEnabled)
            {
                this.ChangeTracker
                    .Entries()
                    .Where(e => e.State != EntityState.Detached)
                    .AsParallel()
                    .ForAll(e => e.State = EntityState.Detached);
            }
        }
    }
}
