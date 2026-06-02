using StudentProjectManagementSystem.Application.Common;
using StudentProjectManagementSystem.Application.DTOs;

namespace StudentProjectManagementSystem.Application.Interfaces.Services;

public interface IRoleService
{
    Task<ServiceResult<IEnumerable<RoleResponseDto>>> GetAllAsync();
    Task<ServiceResult<RoleResponseDto>> GetByIdAsync(int id);
    Task<ServiceResult<RoleResponseDto>> CreateAsync(CreateRoleDto role);
    Task<ServiceResult<RoleResponseDto>> UpdateAsync(int id, UpdateRoleDto role);
    Task<ServiceResult<object>> DeleteAsync(int id);
}
