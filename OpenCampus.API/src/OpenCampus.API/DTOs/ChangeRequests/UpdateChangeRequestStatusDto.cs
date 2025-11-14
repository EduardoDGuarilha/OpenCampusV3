using System.ComponentModel.DataAnnotations;
using OpenCampus.API.Entities;

namespace OpenCampus.API.DTOs.ChangeRequests;

public record UpdateChangeRequestStatusDto
{
    [Required]
    [EnumDataType(typeof(ChangeRequestStatus))]
    public ChangeRequestStatus Status { get; init; }

    [MaxLength(2000)]
    public string? ResolutionNotes { get; init; }
}
