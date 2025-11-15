namespace OpenCampus.API.Common.Constants;

public static class ErrorCodes
{
    public const string ReviewAlreadyExists = "review_already_exists";
    public const string ReviewNotFound = "review_not_found";
    public const string ReviewUnavailable = "review_unavailable";
    public const string CommentNotAllowed = "comment_not_allowed";
    public const string CommentTargetNotFound = "comment_target_not_found";
    public const string UnauthorizedAction = "unauthorized_action";
    public const string ChangeRequestNotFound = "change_request_not_found";
    public const string ChangeRequestAlreadyResolved = "change_request_already_resolved";
    public const string ChangeRequestInvalidPayload = "change_request_invalid_payload";
    public const string TargetEntityNotFound = "target_entity_not_found";
    public const string UserNotFound = "user_not_found";
    public const string UserAlreadyExists = "user_already_exists";
}
