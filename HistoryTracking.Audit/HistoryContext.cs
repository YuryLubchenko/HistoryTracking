namespace HistoryTracking.Audit;

internal class HistoryContext : IHistoryContext
{
    public long? ActionLogId { get; set; }
}
