using FluentValidation;
using Microsoft.AspNetCore.Mvc;
using StudentProjectManagementSystem.Application.DTOs;
using StudentProjectManagementSystem.Application.Interfaces.Services;

namespace StudentProjectManagementSystem.Presentation.Controllers;

[Route("api/[controller]")]
public class RoleController(
    IRoleService roleService,
    IValidator<CreateRoleDto> createValidator,
    IValidator<UpdateRoleDto> updateValidator
) : ApiControllerBase
{
    private readonly IRoleService _roleService = roleService;

    [HttpGet]
    public async Task<IActionResult> GetAllRoles()
    {
        return ToActionResult(await _roleService.GetAllAsync());
    }

    [HttpGet("{id}")]
    public async Task<IActionResult> GetRoleById([FromRoute] int id)
    {
        return ToActionResult(await _roleService.GetByIdAsync(id));
    }

    [HttpPost]
    public async Task<IActionResult> CreateRole(
        [FromBody] CreateRoleDto role,
        [FromHeader(Name = "X-Correlation-Id")] string? correlationId = null)
    {
        createValidator.ValidateAndThrow(role);
        return ToActionResult(await _roleService.CreateAsync(role));
    }

    [HttpPut("{id}")]
    public async Task<IActionResult> UpdateRole(
        [FromRoute] int id,
        [FromBody] UpdateRoleDto role,
        [FromHeader(Name = "X-Correlation-Id")] string? correlationId = null)
    {
        updateValidator.ValidateAndThrow(role);
        return ToActionResult(await _roleService.UpdateAsync(id, role));
    }

    [HttpDelete("{id}")]
    public async Task<IActionResult> DeleteRole(
        [FromRoute] int id,
        [FromHeader(Name = "X-Correlation-Id")] string? correlationId = null)
    {
        return ToActionResult(await _roleService.DeleteAsync(id));
    }
}
