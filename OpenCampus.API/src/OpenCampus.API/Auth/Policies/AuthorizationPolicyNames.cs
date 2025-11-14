namespace OpenCampus.API.Auth.Policies;

public static class AuthorizationPolicyNames
{
    public const string RequireAuthenticatedUser = "RequireAuthenticatedUser";
    public const string RequireStudent = "RequireStudent";
    public const string RequireProfessor = "RequireProfessor";
    public const string RequireInstitution = "RequireInstitution";
    public const string RequireModerator = "RequireModerator";
    public const string RequireOfficialCommenter = "RequireOfficialCommenter";
}
