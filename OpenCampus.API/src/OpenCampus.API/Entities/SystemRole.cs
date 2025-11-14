using OpenCampus.API.Entities.Base;

namespace OpenCampus.API.Entities;

public class SystemRole : AuditableEntity
{
    public string Name { get; set; } = string.Empty;

    public string NormalizedName { get; set; } = string.Empty;

    public string DisplayName { get; set; } = string.Empty;

    public string? Description { get; set; }
}
