using System;
using System.Threading;
using System.Threading.Tasks;
using OpenCampus.API.Common.Constants;
using OpenCampus.API.Common.Exceptions;
using OpenCampus.API.Entities;

namespace OpenCampus.API.Services.Validators;

public class CommentEligibilityValidator
{
    public Task ValidateAsync(User author, Review review, bool isOfficial, CancellationToken cancellationToken = default)
    {
        if (author == null)
        {
            throw new ArgumentNullException(nameof(author));
        }

        if (review == null)
        {
            throw new ArgumentNullException(nameof(review));
        }

        if (!author.IsActive)
        {
            throw new ForbiddenException("Inactive users cannot create comments.", ErrorCodes.UnauthorizedAction);
        }

        if (isOfficial)
        {
            EnsureOfficialEligibility(author);
            return Task.CompletedTask;
        }

        if (!review.Approved)
        {
            throw new ForbiddenException("Comments are only allowed on approved reviews.", ErrorCodes.CommentNotAllowed);
        }

        return Task.CompletedTask;
    }

    private static void EnsureOfficialEligibility(User author)
    {
        if (author.Role == UserRole.Professor || author.Role == UserRole.Institution)
        {
            return;
        }

        throw new ForbiddenException("Only verified professors or institutions may post official comments.", ErrorCodes.CommentNotAllowed);
    }
}