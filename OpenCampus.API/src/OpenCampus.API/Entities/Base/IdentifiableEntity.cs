using System;

namespace OpenCampus.API.Entities.Base;

public abstract class IdentifiableEntity
{
    public Guid Id { get; set; }
}
