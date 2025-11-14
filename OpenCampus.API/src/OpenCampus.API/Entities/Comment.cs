using System;
using OpenCampus.API.Entities.Base;

namespace OpenCampus.API.Entities;

public class Comment : AuditableEntity
{
    public Guid ReviewId { get; set; }

    public Review Review { get; set; } = null!;

    public Guid AuthorId { get; set; }

    public User Author { get; set; } = null!;

    public string Text { get; set; } = string.Empty;

    public bool IsOfficial { get; set; }
}
