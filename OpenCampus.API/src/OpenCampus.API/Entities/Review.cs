using System;
using System.Collections.Generic;
using OpenCampus.API.Entities.Base;

namespace OpenCampus.API.Entities;

public class Review : AuditableEntity
{
    public Guid AuthorId { get; set; }

    public User Author { get; set; } = null!;

    public ReviewTargetType TargetType { get; set; }

    public int ScoreClarity { get; set; }

    public int ScoreRelevance { get; set; }

    public int ScoreSupport { get; set; }

    public int ScoreInfrastructure { get; set; }

    public string Text { get; set; } = string.Empty;

    public bool Approved { get; set; }

    public Guid? InstitutionId { get; set; }

    public Institution? Institution { get; set; }

    public Guid? CourseId { get; set; }

    public Course? Course { get; set; }

    public Guid? ProfessorId { get; set; }

    public Professor? Professor { get; set; }

    public Guid? SubjectId { get; set; }

    public Subject? Subject { get; set; }

    public ICollection<Comment> Comments { get; set; } = new List<Comment>();
}

public enum ReviewTargetType
{
    Institution = 1,
    Course = 2,
    Professor = 3,
    Subject = 4
}
