using System.Collections.Generic;

namespace OpenCampus.API.Auth.Roles;

public static class RoleSeedData
{
    public static IReadOnlyCollection<RoleSeed> Roles { get; } = new[]
    {
        new RoleSeed(
            RoleNames.Student,
            "Estudante",
            "Pode registrar avaliações anônimas e acompanhar respostas às próprias contribuições."),
        new RoleSeed(
            RoleNames.Professor,
            "Professor",
            "Pode comentar oficialmente avaliações aprovadas e sugerir ajustes estruturais."),
        new RoleSeed(
            RoleNames.Institution,
            "Instituição",
            "Representa órgãos institucionais autorizados a comentar e propor mudanças."),
        new RoleSeed(
            RoleNames.Moderator,
            "Moderador",
            "Responsável por mediação, aprovação e rejeição de avaliações e solicitações de mudança.")
    };
}

public sealed record RoleSeed(string Name, string DisplayName, string Description)
{
    public string NormalizedName => Name.ToUpperInvariant();
}
