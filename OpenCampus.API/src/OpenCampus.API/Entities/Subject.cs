using System;
using System.Collections.Generic;
using OpenCampus.API.Entities.Base;

namespace OpenCampus.API.Entities;

public class Subject : AuditableEntity
{
    public string Name { get; set; } = string.Empty;

    public string? Description { get; set; }

    public Guid CourseId { get; set; }

    public Course Course { get; set; } = null!;

    public ICollection<Review> Reviews { get; set; } = new List<Review>();

    public ICollection<ChangeRequest> ChangeRequests { get; set; } = new List<ChangeRequest>();
}
