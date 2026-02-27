using HistoryTracking.Audit.Configuration;
using WebApp.Entities;

namespace WebApp.AuditConfigurations;

public class ClientAuditConfiguration : IAuditEntityConfiguration<ClientEntity>
{
    public void Configure(IEntityAuditBuilder<ClientEntity> builder)
    {
        builder.HasName("Customer");
        builder.Property(c => c.Email).HasName("EmailAddress");
        builder.Property(c => c.Phone).Ignore();
    }
}
