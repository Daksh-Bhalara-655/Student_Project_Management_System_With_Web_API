using Microsoft.EntityFrameworkCore;
using StudentProjectManagementSystem.Application.Common;
using StudentProjectManagementSystem.Application.DTOs;
using StudentProjectManagementSystem.Application.Interfaces.Repositories;
using StudentProjectManagementSystem.Application.Interfaces.Services;
using StudentProjectManagementSystem.Domain.Entities;

namespace StudentProjectManagementSystem.Application.Services;

public class PermissionService : IPermissionService
{
    private readonly IRepository<Permission> _permissions;

    public PermissionService(IRepository<Permission> permissions)
    {
        _permissions = permissions;
    }

    public async Task<ServiceResult<IEnumerable<PermissionResponseDto>>> GetAllAsync()
    {
        var permissions = await _permissions.Query()
            .AsNoTracking()
            .Select(permission => ToResponse(permission))
            .ToListAsync();

        return ServiceResult<IEnumerable<PermissionResponseDto>>.Ok("Permissions retrieved successfully", permissions);
    }

    public async Task<ServiceResult<PermissionResponseDto>> GetByIdAsync(int id)
    {
        var permission = await _permissions.Query()
            .AsNoTracking()
            .Where(currentPermission => currentPermission.PermissionId == id)
            .Select(currentPermission => ToResponse(currentPermission))
            .FirstOrDefaultAsync();

        return permission == null
            ? ServiceResult<PermissionResponseDto>.NotFound("Permission not found")
            : ServiceResult<PermissionResponseDto>.Ok("Permission retrieved successfully", permission);
    }

    public async Task<ServiceResult<PermissionResponseDto>> CreateAsync(CreatePermissionDto permission)
    {
        var duplicatePermission = await _permissions.Query().AnyAsync(currentPermission =>
            currentPermission.PermissionName == permission.PermissionName &&
            currentPermission.ModuleName == permission.ModuleName);

        if (duplicatePermission)
        {
            return ServiceResult<PermissionResponseDto>.Conflict("This permission already exists");
        }

        var addPermission = new Permission
        {
            PermissionName = permission.PermissionName,
            ModuleName = permission.ModuleName,
            Description = permission.Description
        };

        await _permissions.AddAsync(addPermission);
        await _permissions.SaveChangesAsync();

        return ServiceResult<PermissionResponseDto>.Ok("Permission created successfully", ToResponse(addPermission));
    }

    public async Task<ServiceResult<PermissionResponseDto>> UpdateAsync(int id, UpdatePermissionDto permission)
    {
        var updatePermission = await _permissions.FindAsync(id);
        if (updatePermission == null)
        {
            return ServiceResult<PermissionResponseDto>.NotFound("Permission not found");
        }

        var duplicatePermission = await _permissions.Query().AnyAsync(currentPermission =>
            currentPermission.PermissionName == permission.PermissionName &&
            currentPermission.ModuleName == permission.ModuleName &&
            currentPermission.PermissionId != id);

        if (duplicatePermission)
        {
            return ServiceResult<PermissionResponseDto>.Conflict("This permission already exists");
        }

        updatePermission.PermissionName = permission.PermissionName;
        updatePermission.ModuleName = permission.ModuleName;
        updatePermission.Description = permission.Description;

        await _permissions.SaveChangesAsync();

        return ServiceResult<PermissionResponseDto>.Ok("Permission updated successfully", ToResponse(updatePermission));
    }

    public async Task<ServiceResult<object>> DeleteAsync(int id)
    {
        var permission = await _permissions.FindAsync(id);
        if (permission == null)
        {
            return ServiceResult<object>.NotFound("Permission not found");
        }

        _permissions.Remove(permission);
        await _permissions.SaveChangesAsync();

        return ServiceResult<object>.Ok("Permission deleted successfully", null);
    }

    private static PermissionResponseDto ToResponse(Permission permission)
    {
        return new PermissionResponseDto
        {
            PermissionId = permission.PermissionId,
            PermissionName = permission.PermissionName,
            ModuleName = permission.ModuleName,
            Description = permission.Description
        };
    }
}
