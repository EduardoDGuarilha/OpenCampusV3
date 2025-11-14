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
public class InstitutionsController : ControllerBase
{
    private readonly IInstitutionService _institutionService;

    public InstitutionsController(IInstitutionService institutionService)
    {
        _institutionService = institutionService ?? throw new ArgumentNullException(nameof(institutionService));
    }

    [HttpGet]
    public async Task<IActionResult> GetInstitutionsAsync([FromQuery] SearchFilterDto? filter, CancellationToken cancellationToken)
    {
        filter ??= new SearchFilterDto();
        var institutions = await _institutionService.SearchAsync(filter, cancellationToken).ConfigureAwait(false);
        return ApiResponseFactory.Success(institutions);
    }

    [HttpGet("summaries")]
    public async Task<IActionResult> GetSummariesAsync(CancellationToken cancellationToken)
    {
        var summaries = await _institutionService.GetSummariesAsync(cancellationToken).ConfigureAwait(false);
        return ApiResponseFactory.Success(summaries);
    }

    [HttpGet("{institutionId:guid}")]
    public async Task<IActionResult> GetByIdAsync(Guid institutionId, CancellationToken cancellationToken)
    {
        var institution = await _institutionService.GetByIdAsync(institutionId, cancellationToken).ConfigureAwait(false);
        return ApiResponseFactory.Success(institution);
    }
}
