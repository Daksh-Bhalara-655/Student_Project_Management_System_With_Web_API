using StudentProjectManagementSystem.Application.Common;
using StudentProjectManagementSystem.Application.DTOs;
using StudentProjectManagementSystem.Domain.Entities;

namespace StudentProjectManagementSystem.Application.Interfaces.Services;

public interface IProjectAllocationService
{
    Task<ServiceResult<IEnumerable<ProjectAllocationResponseDto>>> GetAllAsync();
    Task<ServiceResult<ProjectAllocationResponseDto>> GetByIdAsync(long id);
    Task<ServiceResult<ProjectAllocation>> CreateAsync(CreateProjectAllocationDto allocation);
    Task<ServiceResult<ProjectAllocation>> UpdateAsync(long id, UpdateProjectAllocationDto allocation);
    Task<ServiceResult<object>> DeleteAsync(long id);
}
