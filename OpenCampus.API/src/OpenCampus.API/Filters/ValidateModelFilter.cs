using System;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc.Filters;
using OpenCampus.API.Common.Responses;

namespace OpenCampus.API.Filters;

public sealed class ValidateModelFilter : IAsyncActionFilter
{
    public async Task OnActionExecutionAsync(ActionExecutingContext context, ActionExecutionDelegate next)
    {
        if (context == null)
        {
            throw new ArgumentNullException(nameof(context));
        }

        if (!context.ModelState.IsValid)
        {
            context.Result = ApiResponseFactory.ValidationError(context.ModelState);
            return;
        }

        await next().ConfigureAwait(false);
    }
}
