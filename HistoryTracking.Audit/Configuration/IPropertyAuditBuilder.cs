namespace HistoryTracking.Audit.Configuration;

public interface IPropertyAuditBuilder
{
    IPropertyAuditBuilder HasName(string name);
    IPropertyAuditBuilder Ignore();
}
