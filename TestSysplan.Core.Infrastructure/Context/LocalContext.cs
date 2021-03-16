using Microsoft.EntityFrameworkCore;
using TestSysplan.Core.Infrastructure.Context.Configuration;
using TestSysplan.Core.Model;

namespace TestSysplan.Core.Infrastructure.Context
{
    internal sealed class LocalContext : ContextBase
    {
        public DbSet<Client> Clients { get; internal set; }

        public LocalContext(DbContextOptions options) : base(options, null) { }

        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {
            base.OnModelCreating(modelBuilder);          
            modelBuilder.ApplyConfiguration(new ClientConfiguration());
        }
    }
}
