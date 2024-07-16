using FluentValidation;
using Microsoft.AspNetCore.Mvc;

namespace AverageLatencyService.Models;

public record Period
{
    [FromQuery(Name = "startDate")]
    public DateTime StartDate { get; set; }
    [FromQuery(Name = "endDate")]
    public DateTime EndDate { get; set; }

    public string [] ToArray()
    {
        return [ StartDate.ToString("yyyy-MM-dd"), EndDate.ToString("yyyy-MM-dd")];
    }
}

public class PeriodValidator : AbstractValidator<Period>
{
    public PeriodValidator()
    {
        RuleFor(p => p.StartDate).Must(d => d.Year == 2021).WithMessage("Year must be 2021 for start date.");
        RuleFor(p => p.EndDate).Must(d => d.Year == 2021).WithMessage("Year must be 2021 for end date.");
        RuleFor(p => p).Must(p => p.StartDate <= p.EndDate).WithMessage("Start date must not be after end date.");
    }
}
