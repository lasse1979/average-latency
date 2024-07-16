namespace AverageLatencyService.Models;

public record Latencies
{
    public string[] Period { get; set; } = [];
    public AverageRequestLatency[] AverageLatencies { get; set; } = [];
}
