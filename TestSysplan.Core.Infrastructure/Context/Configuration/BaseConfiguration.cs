using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using TestSysplan.Core.Model;

namespace TestSysplan.Core.Infrastructure.Context.Configuration
{
    internal abstract class BaseConfiguration<E> : IEntityTypeConfiguration<E> where E : ModelBase
    {
        public virtual void Configure(EntityTypeBuilder<E> builder)
        {
            builder.HasKey(e => e.Id);            
            builder.HasAlternateKey(e => e.Uuid);
            builder.Property(e => e.Uuid).ValueGeneratedOnAdd();
        }
    }
}
