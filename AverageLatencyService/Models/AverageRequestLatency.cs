namespace AverageLatencyService.Models;

public record AverageRequestLatency
{
    public int ServiceId { get; set; }
    public int NumberOfRequests { get; set; }
    public int AverageResponseTimeInMs { get; set; }
}
