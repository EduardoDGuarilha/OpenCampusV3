using System;

namespace OpenCampus.API.Entities.Base;

public abstract class AuditableEntity : IdentifiableEntity
{
    public DateTime CreatedAt { get; set; }

    public DateTime? UpdatedAt { get; set; }
}
