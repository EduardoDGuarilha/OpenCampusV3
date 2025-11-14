using System;
using System.Collections.Generic;
using System.Linq;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.ModelBinding;

namespace OpenCampus.API.Common.Responses;

public static class ApiResponseFactory
{
    public static IActionResult Success<T>(T data)
    {
        return new OkObjectResult(new ApiResponse<T>(true, data));
    }

    public static IActionResult Success<T>(T data, int statusCode)
    {
        return new ObjectResult(new ApiResponse<T>(true, data))
        {
            StatusCode = statusCode,
        };
    }

    public static IActionResult CreatedAtAction<T>(ControllerBase controller, string actionName, object? routeValues, T data)
    {
        if (controller == null)
        {
            throw new ArgumentNullException(nameof(controller));
        }

        return controller.CreatedAtAction(actionName, routeValues, new ApiResponse<T>(true, data));
    }

    public static IActionResult CreatedAtRoute<T>(ControllerBase controller, string routeName, object? routeValues, T data)
    {
        if (controller == null)
        {
            throw new ArgumentNullException(nameof(controller));
        }

        return controller.CreatedAtRoute(routeName, routeValues, new ApiResponse<T>(true, data));
    }

    public static IActionResult NoContent()
    {
        return new NoContentResult();
    }

    public static IActionResult Unauthorized(string? message = null)
    {
        return new ObjectResult(new ApiResponse<object?>(false, null, BuildErrorPayload("authorization", message ?? "Unauthorized access.")))
        {
            StatusCode = StatusCodes.Status401Unauthorized,
        };
    }

    public static IActionResult Forbidden(string? message = null)
    {
        return new ObjectResult(new ApiResponse<object?>(false, null, BuildErrorPayload("authorization", message ?? "Forbidden.")))
        {
            StatusCode = StatusCodes.Status403Forbidden,
        };
    }

    public static IActionResult ValidationError(ModelStateDictionary modelState)
    {
        if (modelState == null)
        {
            throw new ArgumentNullException(nameof(modelState));
        }

        var errors = modelState
            .Where(entry => entry.Value?.Errors.Count > 0)
            .ToDictionary(
                entry => entry.Key,
                entry => entry.Value!.Errors
                    .Select(error => string.IsNullOrWhiteSpace(error.ErrorMessage) ? "Invalid value." : error.ErrorMessage)
                    .ToArray(),
                StringComparer.Ordinal);

        return new BadRequestObjectResult(new ApiResponse<object?>(false, null, errors));
    }

    public record ApiResponse<T>(bool Success, T? Data, IReadOnlyDictionary<string, string[]>? Errors = null);

    private static IReadOnlyDictionary<string, string[]> BuildErrorPayload(string key, string message)
    {
        return new Dictionary<string, string[]>(1, StringComparer.Ordinal)
        {
            [key] = new[] { message },
        };
    }
}
