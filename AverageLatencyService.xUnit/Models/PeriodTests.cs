using AverageLatencyService.Models;
using FluentAssertions;
using FluentValidation.Results;

namespace AverageLatencyService.xUnit.Models;

public class PeriodTests
{
    public static IEnumerable<object[]> TestValidationData()
    {
        yield return new object[]
        {
            new Period
            {
                StartDate = DateTime.Parse("2022-01-03"),
                EndDate = DateTime.Parse("2022-01-03")
            },
            false,
            new KeyValuePair<string, string>[]
            {
                new("StartDate", "Year must be 2021 for start date."),
                new("EndDate", "Year must be 2021 for end date.")
            }
        };
        
        yield return new object[]
        {
            new Period
            {
                StartDate = DateTime.Parse("2021-01-03"),
                EndDate = DateTime.Parse("2021-01-01")
            },
            false,
            new KeyValuePair<string, string>[]
            {
                new("", "Start date must not be after end date."),
            }
        };
        
        yield return new object[]
        {
            new Period
            {
                StartDate = DateTime.Parse("2021-01-01"),
                EndDate = DateTime.Parse("2021-01-03")
            },
            true,
            Array.Empty<KeyValuePair<string, string>>()
        };
    }

    [Theory]
    [MemberData(nameof(TestValidationData))]
    public void TestValidation(Period period, bool expectedResult, KeyValuePair<string, string>[] expectedErrors)
    {
        // Arrange
        var validator = new PeriodValidator();

        // Act
        var validationResult = validator.Validate(period);

        // Assert
        validationResult.IsValid.Should().Be(expectedResult);
        validationResult.Errors.Should().HaveCount(expectedErrors.Length);
        if (validationResult.Errors.Count != 0)
        {
            validationResult.Errors.Select(_ => new KeyValuePair<string, string>(_.PropertyName, _.ErrorMessage)).Should().Contain(expectedErrors);
        }
    }
}
