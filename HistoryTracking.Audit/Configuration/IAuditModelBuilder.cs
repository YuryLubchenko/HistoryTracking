using System.Reflection;

namespace HistoryTracking.Audit.Configuration;

public interface IAuditModelBuilder
{
    IAuditModelBuilder ApplyConfiguration<T>(IAuditEntityConfiguration<T> configuration);
    IAuditModelBuilder ApplyConfigurationsFromAssembly(Assembly assembly);
}
