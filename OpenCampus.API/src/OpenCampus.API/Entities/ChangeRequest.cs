using System;
using OpenCampus.API.Entities.Base;

namespace OpenCampus.API.Entities;

public class ChangeRequest : AuditableEntity
{
    public ChangeRequestTargetType TargetType { get; set; }

    public string SuggestedData { get; set; } = string.Empty;

    public ChangeRequestStatus Status { get; set; } = ChangeRequestStatus.Pending;

    public Guid CreatedById { get; set; }

    public User CreatedBy { get; set; } = null!;

    public Guid? ResolvedById { get; set; }

    public User? ResolvedBy { get; set; }

    public DateTime? ResolvedAt { get; set; }

    public string? ResolutionNotes { get; set; }

    public Guid? InstitutionId { get; set; }

    public Institution? Institution { get; set; }

    public Guid? CourseId { get; set; }

    public Course? Course { get; set; }

    public Guid? ProfessorId { get; set; }

    public Professor? Professor { get; set; }

    public Guid? SubjectId { get; set; }

    public Subject? Subject { get; set; }
}

public enum ChangeRequestTargetType
{
    Institution = 1,
    Course = 2,
    Professor = 3,
    Subject = 4
}

public enum ChangeRequestStatus
{
    Pending = 1,
    Approved = 2,
    Rejected = 3
}
