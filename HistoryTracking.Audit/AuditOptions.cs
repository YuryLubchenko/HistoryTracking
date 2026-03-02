namespace HistoryTracking.Audit;

public class AuditOptions
{
    /// <summary>
    /// The Microsoft.FeatureManagement feature flag name that controls whether auditing is active.
    /// When null or empty, auditing is always enabled.
    /// </summary>
    public string FeatureToggleName { get; set; }
}
