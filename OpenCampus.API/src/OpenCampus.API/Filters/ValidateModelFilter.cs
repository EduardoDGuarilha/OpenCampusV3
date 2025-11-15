using System;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Filters;
using Microsoft.Extensions.Logging;

namespace OpenCampus.API.Filters;

public sealed class ValidateModelFilter : IAsyncActionFilter
{
    private readonly ILogger<ValidateModelFilter> _logger;

    public ValidateModelFilter(ILogger<ValidateModelFilter> logger)
    {
        _logger = logger ?? throw new ArgumentNullException(nameof(logger));
    }

    public async Task OnActionExecutionAsync(ActionExecutingContext context, ActionExecutionDelegate next)
    {
        if (context == null)
        {
            throw new ArgumentNullException(nameof(context));
        }

        if (next == null)
        {
            throw new ArgumentNullException(nameof(next));
        }

        if (!context.ModelState.IsValid)
        {
            _logger.LogWarning("Request validation failed for {Path}.", context.HttpContext.Request.Path);

            var problemDetails = new ValidationProblemDetails(context.ModelState)
            {
                Status = StatusCodes.Status400BadRequest,
                Title = "The request payload is invalid.",
            };

            context.Result = new BadRequestObjectResult(problemDetails);
            return;
        }

        await next();
    }
}
