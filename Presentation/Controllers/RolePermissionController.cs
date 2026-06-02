using FluentValidation;
using Microsoft.AspNetCore.Mvc;
using StudentProjectManagementSystem.Application.DTOs;
using StudentProjectManagementSystem.Application.Interfaces.Services;

namespace StudentProjectManagementSystem.Presentation.Controllers;

[Route("api/[controller]")]
public class RolePermissionController(
    IRolePermissionService rolePermissionService,
    IValidator<CreateRolePermissionDto> createValidator,
    IValidator<UpdateRolePermissionDto> updateValidator
    ) : ApiControllerBase
{
    private readonly IRolePermissionService _rolePermissionService = rolePermissionService;

    [HttpGet]
    public async Task<IActionResult> GetAllRolePermissions()
    {
        return ToActionResult(await _rolePermissionService.GetAllAsync());
    }

    [HttpGet("{id}")]
    public async Task<IActionResult> GetRolePermissionById([FromRoute] long id)
    {
        return ToActionResult(await _rolePermissionService.GetByIdAsync(id));
    }

    [HttpPost]
    public async Task<IActionResult> CreateRolePermission(
        [FromBody] CreateRolePermissionDto rolePermission,
        [FromHeader(Name = "X-Correlation-Id")] string? correlationId = null)
    {
        createValidator.ValidateAndThrow(rolePermission);
        return ToActionResult(await _rolePermissionService.CreateAsync(rolePermission));
    }

    [HttpPut("{id}")]
    public async Task<IActionResult> UpdateRolePermission(
        [FromRoute] long id,
        [FromBody] UpdateRolePermissionDto rolePermission,
        [FromHeader(Name = "X-Correlation-Id")] string? correlationId = null)
    {
        updateValidator.ValidateAndThrow(rolePermission);
        return ToActionResult(await _rolePermissionService.UpdateAsync(id, rolePermission));
    }

    [HttpDelete("{id}")]
    public async Task<IActionResult> DeleteRolePermission(
        [FromRoute] long id,
        [FromHeader(Name = "X-Correlation-Id")] string? correlationId = null)
    {
        return ToActionResult(await _rolePermissionService.DeleteAsync(id));
    }
}
