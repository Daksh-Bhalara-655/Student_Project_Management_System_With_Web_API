using FluentValidation;
using Microsoft.AspNetCore.Mvc;
using StudentProjectManagementSystem.Application.DTOs;
using StudentProjectManagementSystem.Application.Interfaces.Services;

namespace StudentProjectManagementSystem.Presentation.Controllers;

[Route("api/[controller]")]
public class TaskController(
    IProjectTaskService projectTaskService,
    IValidator<CreateTaskDto> createValidator,
    IValidator<UpdateTaskDto> updateValidator
    ) : ApiControllerBase
{
    private readonly IProjectTaskService _projectTaskService = projectTaskService;

    [HttpGet(Name = "GetAllTasks")]
    public async Task<IActionResult> GetAllTasks(
        [FromQuery] TaskFilterDto filters,
        [FromHeader(Name = "Accept-Language")] string? language = null)
    {
        return ToActionResult(await _projectTaskService.GetAllAsync(filters));
    }

    [HttpGet("{id:long}", Name = "GetTaskById")]
    public async Task<IActionResult> GetTaskById([FromRoute] long id)
    {
        return ToActionResult(await _projectTaskService.GetByIdAsync(id));
    }

    [HttpPost(Name = "CreateTask")]
    public async Task<IActionResult> CreateTask(
        [FromForm] CreateTaskDto task,
        [FromHeader(Name = "X-Correlation-Id")] string? correlationId = null)
    {
        createValidator.ValidateAndThrow(task);
        return ToActionResult(await _projectTaskService.CreateAsync(task));
    }

    [HttpPut("{id:long}", Name = "UpdateTask")]
    public async Task<IActionResult> UpdateTask(
        [FromRoute] long id,
        [FromForm] UpdateTaskDto task,
        [FromQuery] bool notifyAssignee = false,
        [FromHeader(Name = "X-Correlation-Id")] string? correlationId = null)
    {
        updateValidator.ValidateAndThrow(task);
        return ToActionResult(await _projectTaskService.UpdateAsync(id, task));
    }

    [HttpDelete("{id:long}", Name = "DeleteTask")]
    public async Task<IActionResult> DeleteTask(
        [FromRoute] long id,
        [FromHeader(Name = "X-Correlation-Id")] string? correlationId = null)
    {
        return ToActionResult(await _projectTaskService.DeleteAsync(id));
    }

    [HttpGet("priority/{priority:alpha}", Name = "GetTasksByPriority")]
    public async Task<IActionResult> GetTasksByPriority([FromRoute] string priority)
    {
        return ToActionResult(await _projectTaskService.GetByPriorityAsync(priority));
    }

    [HttpGet("project/{projectId:long:min(1)}", Name = "GetTasksByProject")]
    public async Task<IActionResult> GetTasksByProject([FromRoute] long projectId)
    {
        return ToActionResult(await _projectTaskService.GetByProjectAsync(projectId));
    }
}
