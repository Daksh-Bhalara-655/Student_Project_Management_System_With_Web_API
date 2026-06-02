using StudentProjectManagementSystem.Application.Common;
using StudentProjectManagementSystem.Application.DTOs;
using StudentProjectManagementSystem.Domain.Entities;

namespace StudentProjectManagementSystem.Application.Interfaces.Services;

public interface IProjectService
{
    Task<ServiceResult<PagedResult<ProjectResponseDto>>> GetAllAsync(int pageNumber, int pageSize);
    Task<ServiceResult<object>> GetByIdAsync(long id);
    Task<ServiceResult<ProjectResponseDto>> CreateAsync(CreateProjectDto project);
    Task<ServiceResult<Project>> UpdateAsync(long id, UpdateProjectDto project);
    Task<ServiceResult<object>> DeleteAsync(long id);
    Task<ServiceResult<IEnumerable<ProjectResponseDto>>> GetByStatusAsync(string status);
    Task<ServiceResult<IEnumerable<ProjectResponseDto>>> SearchAsync(string title);
    Task<ServiceResult<IEnumerable<TaskResponseDto>>> GetProjectTasksAsync(long id);
}
