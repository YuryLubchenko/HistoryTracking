namespace HistoryTracking.Audit.Configuration;

public interface IAuditEntityConfiguration<T>
{
    void Configure(IEntityAuditBuilder<T> builder);
}
