using Microsoft.EntityFrameworkCore;
using StudentProjectManagementSystem.Application.Common;
using StudentProjectManagementSystem.Application.DTOs;
using StudentProjectManagementSystem.Application.Interfaces.Repositories;
using StudentProjectManagementSystem.Application.Interfaces.Services;
using StudentProjectManagementSystem.Domain.Entities;

namespace StudentProjectManagementSystem.Application.Services;

public class ProjectService : IProjectService
{
    private readonly IRepository<Project> _projects;
    private readonly IRepository<ProjectTask> _tasks;
    private readonly IRepository<ProjectAllocation> _allocations;

    public ProjectService(
        IRepository<Project> projects,
        IRepository<ProjectTask> tasks,
        IRepository<ProjectAllocation> allocations)
    {
        _projects = projects;
        _tasks = tasks;
        _allocations = allocations;
    }

    public async Task<ServiceResult<PagedResult<ProjectResponseDto>>> GetAllAsync(int pageNumber, int pageSize)
    {
        if (pageNumber < 1 || pageSize < 1)
        {
            return ServiceResult<PagedResult<ProjectResponseDto>>.BadRequest("Page number and page size must be greater than zero");
        }

        var query = _projects.Query()
            .AsNoTracking()
            .Where(project => !project.IsDeleted);

        var totalCount = await query.CountAsync();
        var projects = await query
            .OrderByDescending(project => project.CreatedAt)
            .Skip((pageNumber - 1) * pageSize)
            .Take(pageSize)
            .Select(project => ToResponse(project))
            .ToListAsync();

        var result = new PagedResult<ProjectResponseDto>
        {
            Items = projects,
            PageNumber = pageNumber,
            PageSize = pageSize,
            TotalCount = totalCount,
            TotalPages = (int)Math.Ceiling(totalCount / (double)pageSize)
        };

        return ServiceResult<PagedResult<ProjectResponseDto>>.Ok("Projects retrieved successfully", result);
    }

    public async Task<ServiceResult<object>> GetByIdAsync(long id)
    {
        var project = await _projects.Query()
            .AsNoTracking()
            .Where(currentProject => !currentProject.IsDeleted && currentProject.ProjectId == id)
            .Select(currentProject => new
            {
                currentProject.ProjectId,
                currentProject.ProjectTitle,
                currentProject.Description,
                currentProject.StartDate,
                currentProject.EndDate,
                currentProject.ProjectStatus,
                currentProject.TotalTasks,
                currentProject.CompletedTasks,
                currentProject.ProgressPercentage,
                currentProject.CreatedAt,
                Allocations = currentProject.ProjectAllocations
                    .Where(allocation => !allocation.IsDeleted)
                    .Select(allocation => new ProjectAllocationResponseDto
                    {
                        AllocationId = allocation.AllocationId,
                        ProjectTitle = currentProject.ProjectTitle,
                        StudentName = allocation.Student.FullName,
                        FacultyName = allocation.Faculty.FullName,
                        AssignedDate = allocation.AssignedDate,
                        AllocationStatus = allocation.AllocationStatus
                    })
                    .ToList(),
                Tasks = currentProject.Tasks
                    .Where(task => !task.IsDeleted)
                    .Select(task => new TaskResponseDto
                    {
                        TaskId = task.TaskId,
                        ProjectTitle = currentProject.ProjectTitle,
                        StudentName = task.Student.FullName,
                        FacultyName = task.Faculty.FullName,
                        TaskTitle = task.TaskTitle,
                        TaskDescription = task.TaskDescription,
                        TaskStatus = task.TaskStatus,
                        Priority = task.Priority,
                        AssignedScore = task.AssignedScore,
                        EarnedScore = task.EarnedScore,
                        ProgressPercentage = task.ProgressPercentage,
                        DueDate = task.DueDate,
                        CreatedAt = task.CreatedAt
                    })
                    .ToList()
            })
            .FirstOrDefaultAsync();

        return project == null
            ? ServiceResult<object>.NotFound("Project not found")
            : ServiceResult<object>.Ok("Project retrieved successfully", project);
    }

    public async Task<ServiceResult<ProjectResponseDto>> CreateAsync(CreateProjectDto project)
    {
        if (string.IsNullOrWhiteSpace(project.ProjectTitle) || string.IsNullOrWhiteSpace(project.ProjectStatus))
        {
            return ServiceResult<ProjectResponseDto>.BadRequest("Project title and status are required");
        }

        var duplicateProject = await _projects.Query().AnyAsync(currentProject => currentProject.ProjectTitle == project.ProjectTitle && !currentProject.IsDeleted);
        if (duplicateProject)
        {
            return ServiceResult<ProjectResponseDto>.Conflict("A project with this title already exists");
        }

        var addProject = new Project
        {
            ProjectTitle = project.ProjectTitle,
            Description = project.Description,
            StartDate = project.StartDate,
            EndDate = project.EndDate,
            ProjectStatus = project.ProjectStatus,
            TotalTasks = 0,
            CompletedTasks = 0,
            ProgressPercentage = 0,
            IsDeleted = false,
            CreatedAt = DateTime.Now
        };

        await _projects.AddAsync(addProject);
        await _projects.SaveChangesAsync();

        return ServiceResult<ProjectResponseDto>.Ok("Project created successfully", ToResponse(addProject));
    }

    public async Task<ServiceResult<Project>> UpdateAsync(long id, UpdateProjectDto project)
    {
        var updateProject = await _projects.Query().FirstOrDefaultAsync(currentProject => currentProject.ProjectId == id && !currentProject.IsDeleted);
        if (updateProject == null)
        {
            return ServiceResult<Project>.NotFound("Project not found");
        }

        var duplicateProject = await _projects.Query().AnyAsync(currentProject =>
            currentProject.ProjectTitle == project.ProjectTitle &&
            currentProject.ProjectId != id &&
            !currentProject.IsDeleted);

        if (duplicateProject)
        {
            return ServiceResult<Project>.Conflict("A project with this title already exists");
        }

        updateProject.ProjectTitle = project.ProjectTitle;
        updateProject.Description = project.Description;
        updateProject.StartDate = project.StartDate;
        updateProject.EndDate = project.EndDate;
        updateProject.ProjectStatus = project.ProjectStatus;
        updateProject.UpdatedAt = DateTime.Now;

        await _projects.SaveChangesAsync();

        return ServiceResult<Project>.Ok("Project updated successfully", updateProject);
    }

    public async Task<ServiceResult<object>> DeleteAsync(long id)
    {
        var project = await _projects.Query().FirstOrDefaultAsync(currentProject => currentProject.ProjectId == id && !currentProject.IsDeleted);
        if (project == null)
        {
            return ServiceResult<object>.NotFound("Project not found");
        }

        project.IsDeleted = true;
        project.DeletedAt = DateTime.Now;

        var relatedTasks = await _tasks.Query().Where(task => task.ProjectId == id && !task.IsDeleted).ToListAsync();
        foreach (var task in relatedTasks)
        {
            task.IsDeleted = true;
            task.DeletedAt = DateTime.Now;
        }

        var relatedAllocations = await _allocations.Query().Where(allocation => allocation.ProjectId == id && !allocation.IsDeleted).ToListAsync();
        foreach (var allocation in relatedAllocations)
        {
            allocation.IsDeleted = true;
        }

        await _projects.SaveChangesAsync();

        return ServiceResult<object>.Ok("Project deleted successfully", null);
    }

    public async Task<ServiceResult<IEnumerable<ProjectResponseDto>>> GetByStatusAsync(string status)
    {
        var projects = await _projects.Query()
            .AsNoTracking()
            .Where(project => !project.IsDeleted && project.ProjectStatus == status)
            .Select(project => ToResponse(project))
            .ToListAsync();

        return ServiceResult<IEnumerable<ProjectResponseDto>>.Ok($"Projects with status '{status}' retrieved successfully", projects);
    }

    public async Task<ServiceResult<IEnumerable<ProjectResponseDto>>> SearchAsync(string title)
    {
        var projects = await _projects.Query()
            .AsNoTracking()
            .Where(project => !project.IsDeleted && project.ProjectTitle.Contains(title))
            .Select(project => ToResponse(project))
            .ToListAsync();

        return ServiceResult<IEnumerable<ProjectResponseDto>>.Ok("Projects found successfully", projects);
    }

    public async Task<ServiceResult<IEnumerable<TaskResponseDto>>> GetProjectTasksAsync(long id)
    {
        var projectExists = await _projects.Query().AnyAsync(project => project.ProjectId == id && !project.IsDeleted);
        if (!projectExists)
        {
            return ServiceResult<IEnumerable<TaskResponseDto>>.NotFound("Project not found");
        }

        var tasks = await _tasks.Query()
            .AsNoTracking()
            .Where(task => !task.IsDeleted && task.ProjectId == id)
            .Select(task => ToTaskResponse(task))
            .ToListAsync();

        return ServiceResult<IEnumerable<TaskResponseDto>>.Ok("Project tasks retrieved successfully", tasks);
    }

    private static ProjectResponseDto ToResponse(Project project)
    {
        return new ProjectResponseDto
        {
            ProjectId = project.ProjectId,
            ProjectTitle = project.ProjectTitle,
            Description = project.Description,
            StartDate = project.StartDate,
            EndDate = project.EndDate,
            ProjectStatus = project.ProjectStatus,
            TotalTasks = project.TotalTasks,
            CompletedTasks = project.CompletedTasks,
            ProgressPercentage = project.ProgressPercentage,
            CreatedAt = project.CreatedAt
        };
    }

    private static TaskResponseDto ToTaskResponse(ProjectTask task)
    {
        return new TaskResponseDto
        {
            TaskId = task.TaskId,
            ProjectTitle = task.Project.ProjectTitle,
            StudentName = task.Student.FullName,
            FacultyName = task.Faculty.FullName,
            TaskTitle = task.TaskTitle,
            TaskDescription = task.TaskDescription,
            TaskStatus = task.TaskStatus,
            Priority = task.Priority,
            AssignedScore = task.AssignedScore,
            EarnedScore = task.EarnedScore,
            ProgressPercentage = task.ProgressPercentage,
            DueDate = task.DueDate,
            CreatedAt = task.CreatedAt
        };
    }
}
