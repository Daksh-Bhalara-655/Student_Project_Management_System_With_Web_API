using StudentProjectManagementSystem.Application.Common;
using StudentProjectManagementSystem.Application.DTOs;
using StudentProjectManagementSystem.Domain.Entities;

namespace StudentProjectManagementSystem.Application.Interfaces.Services;

public interface IUserService
{
    Task<ServiceResult<PagedResult<UserResponseDto>>> GetAllAsync(UserFilterDto filters);
    Task<ServiceResult<UserResponseDto>> GetByIdAsync(long id);
    Task<ServiceResult<UserResponseDto>> CreateAsync(CreateUserDto user);
    Task<ServiceResult<User>> UpdateAsync(long id, UpdateUserDto user);
    Task<ServiceResult<object>> DeleteAsync(long id);
    Task<ServiceResult<IEnumerable<UserResponseDto>>> SearchByNameAsync(string name);
    Task<ServiceResult<IEnumerable<UserResponseDto>>> GetActiveAsync();
}
