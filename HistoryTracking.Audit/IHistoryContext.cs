namespace HistoryTracking.Audit;

internal interface IHistoryContext
{
    long? ActionLogId { get; set; }
}
