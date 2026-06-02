using FluentValidation;
using Microsoft.AspNetCore.Mvc;
using StudentProjectManagementSystem.Application.DTOs;
using StudentProjectManagementSystem.Application.Interfaces.Services;

namespace StudentProjectManagementSystem.Presentation.Controllers;

[Route("api/[controller]")]
public class ProjectController(
    IProjectService projectService,
    IValidator<CreateProjectDto> createValidator,
    IValidator<UpdateProjectDto> updateValidator
    ) : ApiControllerBase
{
    private readonly IProjectService _projectService = projectService;

    [HttpGet(Name = "GetAllProjects")]
    public async Task<IActionResult> GetAllProjects(
        [FromQuery] int pageNumber = 1,
        [FromQuery] int pageSize = 10,
        [FromHeader(Name = "X-Correlation-Id")] string? correlationId = null)
    {
        return ToActionResult(await _projectService.GetAllAsync(pageNumber, pageSize));
    }

    [HttpGet("{id:long}", Name = "GetProjectById")]
    public async Task<IActionResult> GetProjectById([FromRoute] long id)
    {
        return ToActionResult(await _projectService.GetByIdAsync(id));
    }

    [HttpPost(Name = "CreateProject")]
    public async Task<IActionResult> CreateProject(
        [FromForm] CreateProjectDto project,
        [FromHeader(Name = "X-Correlation-Id")] string? correlationId = null)
    {
        createValidator.ValidateAndThrow(project);
        return ToActionResult(await _projectService.CreateAsync(project));
    }

    [HttpPut("{id:long}", Name = "UpdateProject")]
    public async Task<IActionResult> UpdateProject(
        [FromRoute] long id,
        [FromForm] UpdateProjectDto project,
        [FromHeader(Name = "X-Correlation-Id")] string? correlationId = null)
    {
        updateValidator.ValidateAndThrow(project);
        return ToActionResult(await _projectService.UpdateAsync(id, project));
    }

    [HttpDelete("{id:long}", Name = "DeleteProject")]
    public async Task<IActionResult> DeleteProject(
        [FromRoute] long id,
        [FromHeader(Name = "X-Correlation-Id")] string? correlationId = null)
    {
        return ToActionResult(await _projectService.DeleteAsync(id));
    }

    [HttpGet("status/{status:alpha}", Name = "GetProjectsByStatus")]
    public async Task<IActionResult> GetProjectsByStatus([FromRoute] string status)
    {
        return ToActionResult(await _projectService.GetByStatusAsync(status));
    }

    [HttpGet("search", Name = "SearchProjects")]
    public async Task<IActionResult> SearchProjects([FromQuery] string title)
    {
        return ToActionResult(await _projectService.SearchAsync(title));
    }

    [HttpGet("{id:long}/tasks", Name = "GetProjectTasks")]
    public async Task<IActionResult> GetProjectTasks([FromRoute] long id)
    {
        return ToActionResult(await _projectService.GetProjectTasksAsync(id));
    }
}
