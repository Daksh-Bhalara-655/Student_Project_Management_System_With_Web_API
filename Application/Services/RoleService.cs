using Microsoft.EntityFrameworkCore;
using StudentProjectManagementSystem.Application.Common;
using StudentProjectManagementSystem.Application.DTOs;
using StudentProjectManagementSystem.Application.Interfaces.Repositories;
using StudentProjectManagementSystem.Application.Interfaces.Services;
using StudentProjectManagementSystem.Domain.Entities;

namespace StudentProjectManagementSystem.Application.Services;

public class RoleService : IRoleService
{
    private readonly IRepository<Role> _roles;
    private readonly IRepository<User> _users;

    public RoleService(IRepository<Role> roles, IRepository<User> users)
    {
        _roles = roles;
        _users = users;
    }

    public async Task<ServiceResult<IEnumerable<RoleResponseDto>>> GetAllAsync()
    {
        var roles = await _roles.Query()
            .AsNoTracking()
            .Select(role => ToResponse(role))
            .ToListAsync();

        return ServiceResult<IEnumerable<RoleResponseDto>>.Ok("Roles retrieved successfully", roles);
    }

    public async Task<ServiceResult<RoleResponseDto>> GetByIdAsync(int id)
    {
        var role = await _roles.Query()
            .AsNoTracking()
            .Where(currentRole => currentRole.RoleId == id)
            .Select(currentRole => ToResponse(currentRole))
            .FirstOrDefaultAsync();

        return role == null
            ? ServiceResult<RoleResponseDto>.NotFound("Role not found")
            : ServiceResult<RoleResponseDto>.Ok("Role retrieved successfully", role);
    }

    public async Task<ServiceResult<RoleResponseDto>> CreateAsync(CreateRoleDto role)
    {
        var duplicateRole = await _roles.Query().AnyAsync(currentRole => currentRole.RoleName == role.RoleName);
        if (duplicateRole)
        {
            return ServiceResult<RoleResponseDto>.Conflict("A role with this name already exists");
        }

        var addRole = new Role
        {
            RoleName = role.RoleName,
            Description = role.Description
        };

        await _roles.AddAsync(addRole);
        await _roles.SaveChangesAsync();

        return ServiceResult<RoleResponseDto>.Ok("Role created successfully", ToResponse(addRole));
    }

    public async Task<ServiceResult<RoleResponseDto>> UpdateAsync(int id, UpdateRoleDto role)
    {
        var updateRole = await _roles.FindAsync(id);
        if (updateRole == null)
        {
            return ServiceResult<RoleResponseDto>.NotFound("Role not found");
        }

        var duplicateRole = await _roles.Query().AnyAsync(currentRole => currentRole.RoleName == role.RoleName && currentRole.RoleId != id);
        if (duplicateRole)
        {
            return ServiceResult<RoleResponseDto>.Conflict("A role with this name already exists");
        }

        updateRole.RoleName = role.RoleName;
        updateRole.Description = role.Description;

        await _roles.SaveChangesAsync();

        return ServiceResult<RoleResponseDto>.Ok("Role updated successfully", ToResponse(updateRole));
    }

    public async Task<ServiceResult<object>> DeleteAsync(int id)
    {
        var role = await _roles.FindAsync(id);
        if (role == null)
        {
            return ServiceResult<object>.NotFound("Role not found");
        }

        var activeUsers = await _users.Query().AnyAsync(user => user.RoleId == id && !user.IsDeleted);
        if (activeUsers)
        {
            return ServiceResult<object>.BadRequest("Cannot delete role. Active users are assigned to this role");
        }

        _roles.Remove(role);
        await _roles.SaveChangesAsync();

        return ServiceResult<object>.Ok("Role deleted successfully", null);
    }

    private static RoleResponseDto ToResponse(Role role)
    {
        return new RoleResponseDto
        {
            RoleId = role.RoleId,
            RoleName = role.RoleName,
            Description = role.Description
        };
    }
}
