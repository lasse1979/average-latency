using AverageLatencyService.Integration;
using AverageLatencyService.Models;

namespace AverageLatencyService.Services;

public class LatencyService(ILatencyClient Client)
{

    public async Task<AverageRequestLatency[]> GetAverageRequestLatenciesPerServiceAsync(Period period, CancellationToken cancellationToken = default)
    {
        var requestsPerService = new Dictionary<int, List<RequestLatency>>();

        foreach (var date in GetPeriodDates(period))
        {
            await foreach(var requestLatency in Client.GetLatenciesAsync(date, cancellationToken))
            {
                if (requestLatency == null) continue;

                if (!requestsPerService.ContainsKey(requestLatency.ServiceId))
                {
                    requestsPerService.Add(requestLatency.ServiceId, []);
                }

                requestsPerService[requestLatency.ServiceId].Add(requestLatency);
            }
        }        

        return requestsPerService.Select(_ => 
        {
            var serviceId = _.Key;
            var requests = _.Value.DistinctBy(_ => _.RequestId);

            return new AverageRequestLatency
            {
                ServiceId = serviceId,
                NumberOfRequests = requests.Count(),
                AverageResponseTimeInMs = (int)Math.Round(requests.Average(_ => _.MilliSecondsDelay)),
            };
        }).ToArray();
    }

    private static List<string> GetPeriodDates(Period period)
    {
        var allDates = new List<string>();
        
        for (DateTime date = period.StartDate; date <= period.EndDate; date = date.AddDays(1))
        {
            allDates.Add(date.ToString("yyyy-MM-dd"));
        }

        return allDates;
    }
}
