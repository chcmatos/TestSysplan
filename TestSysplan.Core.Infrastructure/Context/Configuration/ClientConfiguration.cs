using Microsoft.EntityFrameworkCore.Metadata.Builders;
using TestSysplan.Core.Model;

namespace TestSysplan.Core.Infrastructure.Context.Configuration
{
    internal sealed class ClientConfiguration : BaseConfiguration<Client>
    {
        public override void Configure(EntityTypeBuilder<Client> builder)
        {
            base.Configure(builder);
            
            builder.Property(e => e.Age)
                .HasMaxLength(3)
                .IsRequired();

            builder.Property(e => e.Name)
                .HasMaxLength(100)
                .IsRequired();
        }
    }
}
