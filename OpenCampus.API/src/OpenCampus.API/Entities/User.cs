using System.Collections.Generic;
using System.Text.Json.Serialization;
using OpenCampus.API.Entities.Base;

namespace OpenCampus.API.Entities;

public class User : AuditableEntity
{
    public string FullName { get; set; } = string.Empty;

    public string Email { get; set; } = string.Empty;

    [JsonIgnore]
    public string? StudentEmail { get; set; }

    [JsonIgnore]
    public string? Cpf { get; set; }

    [JsonIgnore]
    public string PasswordHash { get; set; } = string.Empty;

    public UserRole Role { get; set; }

    public bool IsActive { get; set; } = true;

    public ICollection<Review> ReviewsAuthored { get; set; } = new List<Review>();

    public ICollection<Comment> CommentsAuthored { get; set; } = new List<Comment>();

    public ICollection<ChangeRequest> ChangeRequestsCreated { get; set; } = new List<ChangeRequest>();

    public ICollection<ChangeRequest> ChangeRequestsResolved { get; set; } = new List<ChangeRequest>();
}

public enum UserRole
{
    Student = 1,
    Professor = 2,
    Institution = 3,
    Moderator = 4
}
