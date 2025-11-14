using System;

namespace OpenCampus.API.DTOs.Subjects;

public record SubjectSummaryDto
{
    public Guid Id { get; init; }

    public string Name { get; init; } = string.Empty;

    public Guid CourseId { get; init; }

    public string CourseName { get; init; } = string.Empty;
}
