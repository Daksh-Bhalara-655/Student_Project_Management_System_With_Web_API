using StudentProjectManagementSystem.Application.Common;
using StudentProjectManagementSystem.Application.DTOs;

namespace StudentProjectManagementSystem.Application.Interfaces.Services;

public interface IPermissionService
{
    Task<ServiceResult<IEnumerable<PermissionResponseDto>>> GetAllAsync();
    Task<ServiceResult<PermissionResponseDto>> GetByIdAsync(int id);
    Task<ServiceResult<PermissionResponseDto>> CreateAsync(CreatePermissionDto permission);
    Task<ServiceResult<PermissionResponseDto>> UpdateAsync(int id, UpdatePermissionDto permission);
    Task<ServiceResult<object>> DeleteAsync(int id);
}
