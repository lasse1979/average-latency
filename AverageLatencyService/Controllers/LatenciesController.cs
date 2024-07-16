using AverageLatencyService.Models;
using AverageLatencyService.Services;
using FluentValidation;
using Microsoft.AspNetCore.Mvc;

namespace AverageLatencyService.Controllers;

[ApiController]
[Route("latencies")]
public class LatenciesController(IValidator<Period> PeriodValidator, LatencyService AverageLatencyService) : ControllerBase
{
    [HttpGet]
    public async Task<ActionResult<Latencies>> GetAsync(Period period, CancellationToken cancellationToken)
    {
        var periodValidationResult = await PeriodValidator.ValidateAsync(period);

        if (!periodValidationResult.IsValid)
        {
            foreach (var error in periodValidationResult.Errors) 
            {
                ModelState.AddModelError($"period{(string.IsNullOrWhiteSpace(error.PropertyName) ? "" : ".")}{error.PropertyName}", error.ErrorMessage);
            }

            return BadRequest(ModelState);
        }

        return Ok(new Latencies
        {
            Period = period.ToArray(),
            AverageLatencies = await AverageLatencyService.GetAverageRequestLatenciesPerServiceAsync(period, cancellationToken),
        });
    }

}
