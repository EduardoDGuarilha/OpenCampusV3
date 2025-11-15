using System;
using Microsoft.Extensions.DependencyInjection;
using OpenCampus.API.Services.Implementations;
using OpenCampus.API.Services.Interfaces;
using OpenCampus.API.Services.Validators;

namespace OpenCampus.API.Configuration.DependencyInjection;

public static class ServiceRegistration
{
    public static IServiceCollection AddApplicationServices(this IServiceCollection services)
    {
        if (services == null)
        {
            throw new ArgumentNullException(nameof(services));
        }

        services.AddScoped<IReviewService, ReviewService>();
        services.AddScoped<ICommentService, CommentService>();
        services.AddScoped<IChangeRequestService, ChangeRequestService>();
        services.AddScoped<IModerationService, ModerationService>();
        services.AddScoped<IInstitutionService, InstitutionService>();
        services.AddScoped<ISubjectService, SubjectService>();
        services.AddScoped<IUserService, UserService>();

        services.AddScoped<ReviewBusinessValidator>();
        services.AddScoped<CommentEligibilityValidator>();

        return services;
    }
}
