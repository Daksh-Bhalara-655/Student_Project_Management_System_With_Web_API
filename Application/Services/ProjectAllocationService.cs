using Microsoft.EntityFrameworkCore;
using StudentProjectManagementSystem.Application.Common;
using StudentProjectManagementSystem.Application.DTOs;
using StudentProjectManagementSystem.Application.Interfaces.Repositories;
using StudentProjectManagementSystem.Application.Interfaces.Services;
using StudentProjectManagementSystem.Domain.Entities;

namespace StudentProjectManagementSystem.Application.Services;

public class ProjectAllocationService : IProjectAllocationService
{
    private readonly IRepository<ProjectAllocation> _allocations;
    private readonly IRepository<Project> _projects;
    private readonly IRepository<User> _users;

    public ProjectAllocationService(
        IRepository<ProjectAllocation> allocations,
        IRepository<Project> projects,
        IRepository<User> users)
    {
        _allocations = allocations;
        _projects = projects;
        _users = users;
    }

    public async Task<ServiceResult<IEnumerable<ProjectAllocationResponseDto>>> GetAllAsync()
    {
        var allocations = await _allocations.Query()
            .AsNoTracking()
            .Where(allocation => !allocation.IsDeleted)
            .Select(allocation => ToResponse(allocation))
            .ToListAsync();

        return ServiceResult<IEnumerable<ProjectAllocationResponseDto>>.Ok("Allocations retrieved successfully", allocations);
    }

    public async Task<ServiceResult<ProjectAllocationResponseDto>> GetByIdAsync(long id)
    {
        var allocation = await _allocations.Query()
            .AsNoTracking()
            .Where(currentAllocation => !currentAllocation.IsDeleted && currentAllocation.AllocationId == id)
            .Select(currentAllocation => ToResponse(currentAllocation))
            .FirstOrDefaultAsync();

        return allocation == null
            ? ServiceResult<ProjectAllocationResponseDto>.NotFound("Allocation not found")
            : ServiceResult<ProjectAllocationResponseDto>.Ok("Allocation retrieved successfully", allocation);
    }

    public async Task<ServiceResult<ProjectAllocation>> CreateAsync(CreateProjectAllocationDto allocation)
    {
        var projectExists = await _projects.Query().AnyAsync(project => project.ProjectId == allocation.ProjectId && !project.IsDeleted);
        if (!projectExists)
        {
            return ServiceResult<ProjectAllocation>.BadRequest("Project not found or is deleted");
        }

        var studentExists = await _users.Query().AnyAsync(user => user.UserId == allocation.StudentId && !user.IsDeleted);
        if (!studentExists)
        {
            return ServiceResult<ProjectAllocation>.BadRequest("Student not found or is deleted");
        }

        var facultyExists = await _users.Query().AnyAsync(user => user.UserId == allocation.FacultyId && !user.IsDeleted);
        if (!facultyExists)
        {
            return ServiceResult<ProjectAllocation>.BadRequest("Faculty not found or is deleted");
        }

        var duplicateAllocation = await _allocations.Query().AnyAsync(currentAllocation =>
            currentAllocation.ProjectId == allocation.ProjectId &&
            currentAllocation.StudentId == allocation.StudentId &&
            currentAllocation.FacultyId == allocation.FacultyId &&
            !currentAllocation.IsDeleted);

        if (duplicateAllocation)
        {
            return ServiceResult<ProjectAllocation>.Conflict("This allocation already exists");
        }

        var addAllocation = new ProjectAllocation
        {
            ProjectId = allocation.ProjectId,
            StudentId = allocation.StudentId,
            FacultyId = allocation.FacultyId,
            AssignedDate = DateTime.Now,
            AllocationStatus = allocation.AllocationStatus,
            IsDeleted = false,
            CreatedAt = DateTime.Now
        };

        await _allocations.AddAsync(addAllocation);
        await _allocations.SaveChangesAsync();

        return ServiceResult<ProjectAllocation>.Ok("Allocation created successfully", addAllocation);
    }

    public async Task<ServiceResult<ProjectAllocation>> UpdateAsync(long id, UpdateProjectAllocationDto allocation)
    {
        var updateAllocation = await _allocations.Query().FirstOrDefaultAsync(currentAllocation => currentAllocation.AllocationId == id && !currentAllocation.IsDeleted);
        if (updateAllocation == null)
        {
            return ServiceResult<ProjectAllocation>.NotFound("Allocation not found");
        }

        if (allocation.IsDeleted)
        {
            updateAllocation.IsDeleted = true;
        }
        else
        {
            updateAllocation.AllocationStatus = allocation.AllocationStatus;
        }

        await _allocations.SaveChangesAsync();

        return ServiceResult<ProjectAllocation>.Ok("Allocation updated successfully", updateAllocation);
    }

    public async Task<ServiceResult<object>> DeleteAsync(long id)
    {
        var allocation = await _allocations.Query().FirstOrDefaultAsync(currentAllocation => currentAllocation.AllocationId == id && !currentAllocation.IsDeleted);
        if (allocation == null)
        {
            return ServiceResult<object>.NotFound("Allocation not found");
        }

        allocation.IsDeleted = true;
        await _allocations.SaveChangesAsync();

        return ServiceResult<object>.Ok("Allocation deleted successfully", null);
    }

    private static ProjectAllocationResponseDto ToResponse(ProjectAllocation allocation)
    {
        return new ProjectAllocationResponseDto
        {
            AllocationId = allocation.AllocationId,
            ProjectTitle = allocation.Project.ProjectTitle,
            StudentName = allocation.Student.FullName,
            FacultyName = allocation.Faculty.FullName,
            AssignedDate = allocation.AssignedDate,
            AllocationStatus = allocation.AllocationStatus
        };
    }
}
