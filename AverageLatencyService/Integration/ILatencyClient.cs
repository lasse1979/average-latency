using AverageLatencyService.Models;

namespace AverageLatencyService.Integration;

public interface ILatencyClient
{
    public IAsyncEnumerable<RequestLatency?> GetLatenciesAsync(string date, CancellationToken cancellationToken);
}
