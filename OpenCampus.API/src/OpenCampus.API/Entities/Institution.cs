using System.Collections.Generic;
using OpenCampus.API.Entities.Base;

namespace OpenCampus.API.Entities;

public class Institution : AuditableEntity
{
    public string Name { get; set; } = string.Empty;

    public string? Acronym { get; set; }

    public string? Description { get; set; }

    public string? WebsiteUrl { get; set; }

    public ICollection<Course> Courses { get; set; } = new List<Course>();

    public ICollection<Professor> Professors { get; set; } = new List<Professor>();

    public ICollection<Review> Reviews { get; set; } = new List<Review>();

    public ICollection<ChangeRequest> ChangeRequests { get; set; } = new List<ChangeRequest>();
}
