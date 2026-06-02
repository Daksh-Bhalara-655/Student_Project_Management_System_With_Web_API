using Microsoft.EntityFrameworkCore;
using StudentProjectManagementSystem.Application.Common;
using StudentProjectManagementSystem.Application.DTOs;
using StudentProjectManagementSystem.Application.Interfaces.Repositories;
using StudentProjectManagementSystem.Application.Interfaces.Services;
using StudentProjectManagementSystem.Domain.Entities;

namespace StudentProjectManagementSystem.Application.Services;

public class RolePermissionService : IRolePermissionService
{
    private readonly IRepository<RolePermission> _rolePermissions;
    private readonly IRepository<Role> _roles;
    private readonly IRepository<Permission> _permissions;

    public RolePermissionService(
        IRepository<RolePermission> rolePermissions,
        IRepository<Role> roles,
        IRepository<Permission> permissions)
    {
        _rolePermissions = rolePermissions;
        _roles = roles;
        _permissions = permissions;
    }

    public async Task<ServiceResult<IEnumerable<RolePermissionResponseDto>>> GetAllAsync()
    {
        var rolePermissions = await _rolePermissions.Query()
            .AsNoTracking()
            .Select(rolePermission => ToResponse(rolePermission))
            .ToListAsync();

        return ServiceResult<IEnumerable<RolePermissionResponseDto>>.Ok("Role permissions retrieved successfully", rolePermissions);
    }

    public async Task<ServiceResult<RolePermissionResponseDto>> GetByIdAsync(long id)
    {
        var rolePermission = await _rolePermissions.Query()
            .AsNoTracking()
            .Where(currentRolePermission => currentRolePermission.RolePermissionId == id)
            .Select(currentRolePermission => ToResponse(currentRolePermission))
            .FirstOrDefaultAsync();

        return rolePermission == null
            ? ServiceResult<RolePermissionResponseDto>.NotFound("Role permission not found")
            : ServiceResult<RolePermissionResponseDto>.Ok("Role permission retrieved successfully", rolePermission);
    }

    public async Task<ServiceResult<RolePermissionResponseDto>> CreateAsync(CreateRolePermissionDto rolePermission)
    {
        var validation = await ValidateRoleAndPermissionAsync(rolePermission.RoleId, rolePermission.PermissionId);
        if (validation != null)
        {
            return validation;
        }

        var duplicate = await _rolePermissions.Query().AnyAsync(currentRolePermission =>
            currentRolePermission.RoleId == rolePermission.RoleId &&
            currentRolePermission.PermissionId == rolePermission.PermissionId);

        if (duplicate)
        {
            return ServiceResult<RolePermissionResponseDto>.Conflict("This role permission already exists");
        }

        var addRolePermission = new RolePermission
        {
            RoleId = rolePermission.RoleId,
            PermissionId = rolePermission.PermissionId,
            CreatedAt = DateTime.Now
        };

        await _rolePermissions.AddAsync(addRolePermission);
        await _rolePermissions.SaveChangesAsync();

        var response = await _rolePermissions.Query()
            .AsNoTracking()
            .Where(currentRolePermission => currentRolePermission.RolePermissionId == addRolePermission.RolePermissionId)
            .Select(currentRolePermission => ToResponse(currentRolePermission))
            .FirstAsync();

        return ServiceResult<RolePermissionResponseDto>.Ok("Role permission created successfully", response);
    }

    public async Task<ServiceResult<RolePermission>> UpdateAsync(long id, UpdateRolePermissionDto rolePermission)
    {
        var updateRolePermission = await _rolePermissions.Query().FirstOrDefaultAsync(currentRolePermission => currentRolePermission.RolePermissionId == id);
        if (updateRolePermission == null)
        {
            return ServiceResult<RolePermission>.NotFound("Role permission not found");
        }

        var roleExists = await _roles.Query().AnyAsync(role => role.RoleId == rolePermission.RoleId);
        if (!roleExists)
        {
            return ServiceResult<RolePermission>.BadRequest("Invalid Role ID");
        }

        var permissionExists = await _permissions.Query().AnyAsync(permission => permission.PermissionId == rolePermission.PermissionId);
        if (!permissionExists)
        {
            return ServiceResult<RolePermission>.BadRequest("Invalid Permission ID");
        }

        var duplicate = await _rolePermissions.Query().AnyAsync(currentRolePermission =>
            currentRolePermission.RoleId == rolePermission.RoleId &&
            currentRolePermission.PermissionId == rolePermission.PermissionId &&
            currentRolePermission.RolePermissionId != id);

        if (duplicate)
        {
            return ServiceResult<RolePermission>.Conflict("This role permission already exists");
        }

        updateRolePermission.RoleId = rolePermission.RoleId;
        updateRolePermission.PermissionId = rolePermission.PermissionId;

        await _rolePermissions.SaveChangesAsync();

        return ServiceResult<RolePermission>.Ok("Role permission updated successfully", updateRolePermission);
    }

    public async Task<ServiceResult<object>> DeleteAsync(long id)
    {
        var rolePermission = await _rolePermissions.Query().FirstOrDefaultAsync(currentRolePermission => currentRolePermission.RolePermissionId == id);
        if (rolePermission == null)
        {
            return ServiceResult<object>.NotFound("Role permission not found");
        }

        _rolePermissions.Remove(rolePermission);
        await _rolePermissions.SaveChangesAsync();

        return ServiceResult<object>.Ok("Role permission deleted successfully", null);
    }

    private async Task<ServiceResult<RolePermissionResponseDto>?> ValidateRoleAndPermissionAsync(int roleId, int permissionId)
    {
        var roleExists = await _roles.Query().AnyAsync(role => role.RoleId == roleId);
        if (!roleExists)
        {
            return ServiceResult<RolePermissionResponseDto>.BadRequest("Invalid Role ID");
        }

        var permissionExists = await _permissions.Query().AnyAsync(permission => permission.PermissionId == permissionId);
        if (!permissionExists)
        {
            return ServiceResult<RolePermissionResponseDto>.BadRequest("Invalid Permission ID");
        }

        return null;
    }

    private static RolePermissionResponseDto ToResponse(RolePermission rolePermission)
    {
        return new RolePermissionResponseDto
        {
            RolePermissionId = rolePermission.RolePermissionId,
            RoleName = rolePermission.Role.RoleName,
            PermissionName = rolePermission.Permission.PermissionName,
            CreatedAt = rolePermission.CreatedAt
        };
    }
}
