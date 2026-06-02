using FluentValidation;
using Microsoft.AspNetCore.Mvc;
using StudentProjectManagementSystem.Application.DTOs;
using StudentProjectManagementSystem.Application.Interfaces.Services;

namespace StudentProjectManagementSystem.Presentation.Controllers;

[Route("api/[controller]")]
public class PermissionController(
    IPermissionService permissionService,
    IValidator<CreatePermissionDto> createValidator,
    IValidator<UpdatePermissionDto> updateValidator
) : ApiControllerBase
{
    private readonly IPermissionService _permissionService = permissionService;

    [HttpGet]
    public async Task<IActionResult> GetAllPermissions()
    {
        return ToActionResult(await _permissionService.GetAllAsync());
    }

    [HttpGet("{id}")]
    public async Task<IActionResult> GetPermissionById([FromRoute] int id)
    {
        return ToActionResult(await _permissionService.GetByIdAsync(id));
    }

    [HttpPost]
    public async Task<IActionResult> CreatePermission(
        [FromBody] CreatePermissionDto permission,
        [FromHeader(Name = "X-Correlation-Id")] string? correlationId = null)
    {
        createValidator.ValidateAndThrow(permission);
        return ToActionResult(await _permissionService.CreateAsync(permission));
    }

    [HttpPut("{id}")]
    public async Task<IActionResult> UpdatePermission(
        [FromRoute] int id,
        [FromBody] UpdatePermissionDto permission,
        [FromHeader(Name = "X-Correlation-Id")] string? correlationId = null)
    {
        updateValidator.ValidateAndThrow(permission);
        return ToActionResult(await _permissionService.UpdateAsync(id, permission));
    }

    [HttpDelete("{id}")]
    public async Task<IActionResult> DeletePermission(
        [FromRoute] int id,
        [FromHeader(Name = "X-Correlation-Id")] string? correlationId = null)
    {
        return ToActionResult(await _permissionService.DeleteAsync(id));
    }
}
