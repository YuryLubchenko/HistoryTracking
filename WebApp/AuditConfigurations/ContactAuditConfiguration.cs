using HistoryTracking.Audit.Configuration;
using WebApp.Entities;

namespace WebApp.AuditConfigurations;

public class ContactAuditConfiguration : IAuditEntityConfiguration<ContactEntity>
{
    public void Configure(IEntityAuditBuilder<ContactEntity> builder)
    {
        builder.Property(c => c.ContactType).AlwaysLog();
    }
}
