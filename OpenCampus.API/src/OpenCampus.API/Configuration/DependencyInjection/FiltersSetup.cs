using System;
using Microsoft.Extensions.DependencyInjection;
using OpenCampus.API.Filters;

namespace OpenCampus.API.Configuration.DependencyInjection;

public static class FiltersSetup
{
    public static IServiceCollection AddApiFilters(this IServiceCollection services)
    {
        if (services == null)
        {
            throw new ArgumentNullException(nameof(services));
        }

        services.AddScoped<ValidateModelFilter>();
        services.AddScoped<ModeratorOnlyFilter>();

        return services;
    }
}
