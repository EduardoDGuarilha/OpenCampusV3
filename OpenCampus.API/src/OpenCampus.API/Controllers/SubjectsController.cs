using System;
using System.Threading;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using OpenCampus.API.Common.Responses;
using OpenCampus.API.DTOs.Shared;
using OpenCampus.API.Services.Interfaces;

namespace OpenCampus.API.Controllers;

[ApiController]
[Route("api/[controller]")]
[AllowAnonymous]
public class SubjectsController : ControllerBase
{
    private readonly ISubjectService _subjectService;

    public SubjectsController(ISubjectService subjectService)
    {
        _subjectService = subjectService ?? throw new ArgumentNullException(nameof(subjectService));
    }

    [HttpGet]
    public async Task<IActionResult> GetSubjectsAsync([FromQuery] SearchFilterDto? filter, CancellationToken cancellationToken)
    {
        filter ??= new SearchFilterDto();
        var subjects = await _subjectService.SearchAsync(filter, cancellationToken).ConfigureAwait(false);
        return ApiResponseFactory.Success(subjects);
    }

    [HttpGet("{subjectId:guid}")]
    public async Task<IActionResult> GetByIdAsync(Guid subjectId, CancellationToken cancellationToken)
    {
        var subject = await _subjectService.GetByIdAsync(subjectId, cancellationToken).ConfigureAwait(false);
        return ApiResponseFactory.Success(subject);
    }

    [HttpGet("course/{courseId:guid}")]
    public async Task<IActionResult> GetByCourseAsync(Guid courseId, CancellationToken cancellationToken)
    {
        var subjects = await _subjectService.GetByCourseAsync(courseId, cancellationToken).ConfigureAwait(false);
        return ApiResponseFactory.Success(subjects);
    }
}
