using System.Linq.Expressions;

namespace HistoryTracking.Audit.Configuration;

public interface IEntityAuditBuilder<T>
{
    IEntityAuditBuilder<T> HasName(string name);
    IEntityAuditBuilder<T> Ignore();
    IPropertyAuditBuilder Property(Expression<Func<T, object>> selector);
}
