using StudentProjectManagementSystem.Application.Common;
using StudentProjectManagementSystem.Application.DTOs;
using StudentProjectManagementSystem.Domain.Entities;

namespace StudentProjectManagementSystem.Application.Interfaces.Services;

public interface IProjectTaskService
{
    Task<ServiceResult<PagedResult<TaskResponseDto>>> GetAllAsync(TaskFilterDto filters);
    Task<ServiceResult<TaskResponseDto>> GetByIdAsync(long id);
    Task<ServiceResult<ProjectTask>> CreateAsync(CreateTaskDto task);
    Task<ServiceResult<ProjectTask>> UpdateAsync(long id, UpdateTaskDto task);
    Task<ServiceResult<object>> DeleteAsync(long id);
    Task<ServiceResult<IEnumerable<TaskResponseDto>>> GetByPriorityAsync(string priority);
    Task<ServiceResult<IEnumerable<TaskResponseDto>>> GetByProjectAsync(long projectId);
}
