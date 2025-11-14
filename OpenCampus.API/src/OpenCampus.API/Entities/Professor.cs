using System;
using System.Collections.Generic;
using OpenCampus.API.Entities.Base;

namespace OpenCampus.API.Entities;

public class Professor : AuditableEntity
{
    public string FullName { get; set; } = string.Empty;

    public string? Bio { get; set; }

    public Guid InstitutionId { get; set; }

    public Institution Institution { get; set; } = null!;

    public ICollection<Review> Reviews { get; set; } = new List<Review>();

    public ICollection<ChangeRequest> ChangeRequests { get; set; } = new List<ChangeRequest>();
}
