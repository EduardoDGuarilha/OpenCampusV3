using System;
using System.Collections.Generic;
using OpenCampus.API.Entities.Base;

namespace OpenCampus.API.Entities;

public class Course : AuditableEntity
{
    public string Name { get; set; } = string.Empty;

    public string? Description { get; set; }

    public Guid InstitutionId { get; set; }

    public Institution Institution { get; set; } = null!;

    public ICollection<Subject> Subjects { get; set; } = new List<Subject>();

    public ICollection<Review> Reviews { get; set; } = new List<Review>();

    public ICollection<ChangeRequest> ChangeRequests { get; set; } = new List<ChangeRequest>();
}
