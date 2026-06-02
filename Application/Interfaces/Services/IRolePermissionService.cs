using StudentProjectManagementSystem.Application.Common;
using StudentProjectManagementSystem.Application.DTOs;
using StudentProjectManagementSystem.Domain.Entities;

namespace StudentProjectManagementSystem.Application.Interfaces.Services;

public interface IRolePermissionService
{
    Task<ServiceResult<IEnumerable<RolePermissionResponseDto>>> GetAllAsync();
    Task<ServiceResult<RolePermissionResponseDto>> GetByIdAsync(long id);
    Task<ServiceResult<RolePermissionResponseDto>> CreateAsync(CreateRolePermissionDto rolePermission);
    Task<ServiceResult<RolePermission>> UpdateAsync(long id, UpdateRolePermissionDto rolePermission);
    Task<ServiceResult<object>> DeleteAsync(long id);
}
