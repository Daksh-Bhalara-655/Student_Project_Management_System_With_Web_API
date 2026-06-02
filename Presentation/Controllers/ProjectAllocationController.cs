using FluentValidation;
using Microsoft.AspNetCore.Mvc;
using StudentProjectManagementSystem.Application.DTOs;
using StudentProjectManagementSystem.Application.Interfaces.Services;

namespace StudentProjectManagementSystem.Presentation.Controllers;

[Route("api/[controller]")]
public class ProjectAllocationController(
    IProjectAllocationService projectAllocationService,
    IValidator<CreateProjectAllocationDto> createValidator,
    IValidator<UpdateProjectAllocationDto> updateValidator
    ) : ApiControllerBase
{
    private readonly IProjectAllocationService _projectAllocationService = projectAllocationService;

    [HttpGet]
    public async Task<IActionResult> GetAllAllocations()
    {
        return ToActionResult(await _projectAllocationService.GetAllAsync());
    }

    [HttpGet("{id}")]
    public async Task<IActionResult> GetAllocationById([FromRoute] long id)
    {
        return ToActionResult(await _projectAllocationService.GetByIdAsync(id));
    }

    [HttpPost]
    public async Task<IActionResult> CreateAllocation(
        [FromBody] CreateProjectAllocationDto allocation,
        [FromHeader(Name = "X-Correlation-Id")] string? correlationId = null)
    {
        createValidator.ValidateAndThrow(allocation);
        return ToActionResult(await _projectAllocationService.CreateAsync(allocation));
    }

    [HttpPut("{id}")]
    public async Task<IActionResult> UpdateAllocation(
        [FromRoute] long id,
        [FromBody] UpdateProjectAllocationDto allocation,
        [FromHeader(Name = "X-Correlation-Id")] string? correlationId = null)
    {
        updateValidator.ValidateAndThrow(allocation);
        return ToActionResult(await _projectAllocationService.UpdateAsync(id, allocation));
    }

    [HttpDelete("{id}")]
    public async Task<IActionResult> DeleteAllocation(
        [FromRoute] long id,
        [FromHeader(Name = "X-Correlation-Id")] string? correlationId = null)
    {
        return ToActionResult(await _projectAllocationService.DeleteAsync(id));
    }
}
