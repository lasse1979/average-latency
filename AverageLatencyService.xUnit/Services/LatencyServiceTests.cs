using AverageLatencyService.Integration;
using AverageLatencyService.Models;
using AverageLatencyService.Services;
using FluentAssertions;
using Moq;

namespace AverageLatencyService.xUnit.Services;

public class LatencyServiceTests
{
    public static IEnumerable<object[]> TestGetAverageLatenciesPerServiceData()
    {
        yield return new object[]
        {
            new Period
            {
                StartDate = DateTime.Parse("2021-01-01"),
                EndDate = DateTime.Parse("2021-01-01")
            },
            new Dictionary<string, RequestLatency[]>
            {
                { "2021-01-01", []}
            },
            Array.Empty<AverageRequestLatency>()
        };
        
        yield return new object[]
        {
            new Period
            {
                StartDate = DateTime.Parse("2021-01-01"),
                EndDate = DateTime.Parse("2021-01-01")
            },
            new Dictionary<string, RequestLatency[]>
            {
                { 
                    "2021-01-01", 
                    [ 
                        new RequestLatency { RequestId = 1, ServiceId = 1, Date = DateTime.Now, MilliSecondsDelay = 10 }, 
                        new RequestLatency { RequestId = 1, ServiceId = 1, Date = DateTime.Now, MilliSecondsDelay = 5 }
                    ]
                }
            },
            new AverageRequestLatency[]
            {
                new() { ServiceId = 1, NumberOfRequests = 1, AverageResponseTimeInMs = 10 }
            }
        };
        
        yield return new object[]
        {
            new Period
            {
                StartDate = DateTime.Parse("2021-01-01"),
                EndDate = DateTime.Parse("2021-01-01")
            },
            new Dictionary<string, RequestLatency[]>
            {
                { 
                    "2021-01-01", 
                    [ 
                        new RequestLatency { RequestId = 1, ServiceId = 1, Date = DateTime.Now, MilliSecondsDelay = 10 }, 
                        new RequestLatency { RequestId = 2, ServiceId = 1, Date = DateTime.Now, MilliSecondsDelay = 6 }
                    ]
                }
            },
            new AverageRequestLatency[]
            {
                new() { ServiceId = 1, NumberOfRequests = 2, AverageResponseTimeInMs = 8 }
            }
        };
        
        yield return new object[]
        {
            new Period
            {
                StartDate = DateTime.Parse("2021-01-01"),
                EndDate = DateTime.Parse("2021-01-02")
            },
            new Dictionary<string, RequestLatency[]>
            {
                { 
                    "2021-01-01", 
                    [ 
                        new RequestLatency { RequestId = 1, ServiceId = 1, Date = DateTime.Now, MilliSecondsDelay = 10 }, 
                        new RequestLatency { RequestId = 2, ServiceId = 1, Date = DateTime.Now, MilliSecondsDelay = 6 }
                    ]
                },
                { 
                    "2021-01-02", 
                    [ 
                        new RequestLatency { RequestId = 1, ServiceId = 1, Date = DateTime.Now, MilliSecondsDelay = 10 }, 
                        new RequestLatency { RequestId = 3, ServiceId = 1, Date = DateTime.Now, MilliSecondsDelay = 5 }, 
                        new RequestLatency { RequestId = 2, ServiceId = 2, Date = DateTime.Now, MilliSecondsDelay = 6 },
                        new RequestLatency { RequestId = 3, ServiceId = 2, Date = DateTime.Now, MilliSecondsDelay = 4 }
                    ]
                }
            },
            new AverageRequestLatency[]
            {
                new() { ServiceId = 1, NumberOfRequests = 3, AverageResponseTimeInMs = 7 },
                new() { ServiceId = 2, NumberOfRequests = 2, AverageResponseTimeInMs = 5 }
            }
        };
    }

    [Theory]
    [MemberData(nameof(TestGetAverageLatenciesPerServiceData))]
    public async Task TestGetAverageLatenciesPerService(Period period, Dictionary<string, RequestLatency[]> requestsPerDate, AverageRequestLatency[] expectedResult)
    {
        // Arrange
        var latencyClient = new Mock<ILatencyClient>();
        var target = new LatencyService(latencyClient.Object);

        foreach (var requestsForDate in requestsPerDate)
        {
            latencyClient.Setup(_ => _.GetLatenciesAsync(It.Is<string>(_ => _ == requestsForDate.Key), It.IsAny<CancellationToken>())).Returns(requestsForDate.Value.ToAsyncEnumerable());
        }

        // Act
        var result = await target.GetAverageRequestLatenciesPerServiceAsync(period);

        // Assert
        if (expectedResult.Length == 0)
        {
            result.Should().BeEmpty();
        }
        else
        {
            result.Should().Contain(expectedResult);
        }
    }
}
