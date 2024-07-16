
using AverageLatencyService.Models;

namespace AverageLatencyService.Integration;

public class LatencyClient(HttpClient HttpClient) : ILatencyClient
{
    public IAsyncEnumerable<RequestLatency?> GetLatenciesAsync(string date, CancellationToken cancellationToken = default)
    {
        return HttpClient.GetFromJsonAsAsyncEnumerable<RequestLatency>($"latencies?date={date}", cancellationToken);
    }
}
