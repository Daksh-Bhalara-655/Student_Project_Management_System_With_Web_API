using Microsoft.EntityFrameworkCore;
using StudentProjectManagementSystem.Application.Common;
using StudentProjectManagementSystem.Application.DTOs;
using StudentProjectManagementSystem.Application.Interfaces.Repositories;
using StudentProjectManagementSystem.Application.Interfaces.Services;
using StudentProjectManagementSystem.Domain.Entities;

namespace StudentProjectManagementSystem.Application.Services;

public class ProjectTaskService : IProjectTaskService
{
    private readonly IRepository<ProjectTask> _tasks;
    private readonly IRepository<Project> _projects;
    private readonly IRepository<ProjectAllocation> _allocations;

    public ProjectTaskService(
        IRepository<ProjectTask> tasks,
        IRepository<Project> projects,
        IRepository<ProjectAllocation> allocations)
    {
        _tasks = tasks;
        _projects = projects;
        _allocations = allocations;
    }

    public async Task<ServiceResult<PagedResult<TaskResponseDto>>> GetAllAsync(TaskFilterDto filters)
    {
        if (filters.PageNumber < 1 || filters.PageSize < 1)
        {
            return ServiceResult<PagedResult<TaskResponseDto>>.BadRequest("Page number and page size must be greater than zero");
        }

        if (filters.MinAssignedScore.HasValue && filters.MaxAssignedScore.HasValue && filters.MinAssignedScore > filters.MaxAssignedScore)
        {
            return ServiceResult<PagedResult<TaskResponseDto>>.BadRequest("Min assigned score cannot be greater than max assigned score");
        }

        if (filters.MinEarnedScore.HasValue && filters.MaxEarnedScore.HasValue && filters.MinEarnedScore > filters.MaxEarnedScore)
        {
            return ServiceResult<PagedResult<TaskResponseDto>>.BadRequest("Min earned score cannot be greater than max earned score");
        }

        var query = ApplyFilters(_tasks.Query().AsNoTracking().Where(task => !task.IsDeleted), filters);
        query = ApplySorting(query, filters);

        var totalCount = await query.CountAsync();
        var tasks = await query
            .Skip((filters.PageNumber - 1) * filters.PageSize)
            .Take(filters.PageSize)
            .Select(task => ToResponse(task))
            .ToListAsync();

        var result = new PagedResult<TaskResponseDto>
        {
            Items = tasks,
            PageNumber = filters.PageNumber,
            PageSize = filters.PageSize,
            TotalCount = totalCount,
            TotalPages = (int)Math.Ceiling(totalCount / (double)filters.PageSize)
        };

        return ServiceResult<PagedResult<TaskResponseDto>>.Ok("Tasks retrieved successfully", result);
    }

    public async Task<ServiceResult<TaskResponseDto>> GetByIdAsync(long id)
    {
        var task = await _tasks.Query()
            .AsNoTracking()
            .Where(currentTask => !currentTask.IsDeleted && currentTask.TaskId == id)
            .Select(currentTask => ToResponse(currentTask))
            .FirstOrDefaultAsync();

        return task == null
            ? ServiceResult<TaskResponseDto>.NotFound("Task not found")
            : ServiceResult<TaskResponseDto>.Ok("Task retrieved successfully", task);
    }

    public async Task<ServiceResult<ProjectTask>> CreateAsync(CreateTaskDto task)
    {
        if (string.IsNullOrWhiteSpace(task.TaskTitle) || string.IsNullOrWhiteSpace(task.TaskStatus) || string.IsNullOrWhiteSpace(task.Priority))
        {
            return ServiceResult<ProjectTask>.BadRequest("Task title, status, and priority are required");
        }

        var projectExists = await _projects.Query().AnyAsync(project => project.ProjectId == task.ProjectId && !project.IsDeleted);
        if (!projectExists)
        {
            return ServiceResult<ProjectTask>.BadRequest("Project not found or is deleted.");
        }

        var isAllocated = await _allocations.Query().AnyAsync(allocation =>
            allocation.ProjectId == task.ProjectId &&
            allocation.StudentId == task.StudentId &&
            allocation.FacultyId == task.FacultyId &&
            !allocation.IsDeleted);

        if (!isAllocated)
        {
            return ServiceResult<ProjectTask>.BadRequest("Access Denied: Faculty is not allocated to this Student on the specified Project.");
        }

        var addTask = new ProjectTask
        {
            ProjectId = task.ProjectId,
            StudentId = task.StudentId,
            FacultyId = task.FacultyId,
            TaskTitle = task.TaskTitle,
            TaskDescription = task.TaskDescription,
            TaskStatus = task.TaskStatus,
            Priority = task.Priority,
            AssignedScore = task.AssignedScore,
            EarnedScore = 0,
            ProgressPercentage = 0,
            StartDate = task.StartDate ?? DateTime.Now,
            DueDate = task.DueDate,
            IsDeleted = false,
            CreatedAt = DateTime.Now
        };

        await _tasks.AddAsync(addTask);
        await _tasks.SaveChangesAsync();
        await UpdateProjectProgressAsync(task.ProjectId);

        return ServiceResult<ProjectTask>.Ok("Task created successfully", addTask);
    }

    public async Task<ServiceResult<ProjectTask>> UpdateAsync(long id, UpdateTaskDto task)
    {
        var updateTask = await _tasks.Query().FirstOrDefaultAsync(currentTask => currentTask.TaskId == id && !currentTask.IsDeleted);
        if (updateTask == null)
        {
            return ServiceResult<ProjectTask>.NotFound("Task not found");
        }

        var isAllocated = await _allocations.Query().AnyAsync(allocation =>
            allocation.ProjectId == updateTask.ProjectId &&
            allocation.StudentId == updateTask.StudentId &&
            allocation.FacultyId == updateTask.FacultyId &&
            !allocation.IsDeleted);

        if (!isAllocated)
        {
            return ServiceResult<ProjectTask>.BadRequest("Access Denied: Allocation for this project/student/faculty is inactive or deleted.");
        }

        updateTask.TaskTitle = task.TaskTitle;
        updateTask.TaskDescription = task.TaskDescription;
        updateTask.TaskStatus = task.TaskStatus;
        updateTask.Priority = task.Priority;
        updateTask.AssignedScore = task.AssignedScore;
        updateTask.EarnedScore = task.EarnedScore;
        updateTask.ProgressPercentage = task.AssignedScore > 0 ? Math.Round((task.EarnedScore / task.AssignedScore) * 100, 2) : 0;
        updateTask.StartDate = task.StartDate;
        updateTask.DueDate = task.DueDate;
        updateTask.CompletedDate = task.CompletedDate;
        updateTask.FacultyRemarks = task.FacultyRemarks;
        updateTask.StudentRemarks = task.StudentRemarks;
        updateTask.UpdatedAt = DateTime.Now;

        if (task.TaskStatus.Equals("Completed", StringComparison.OrdinalIgnoreCase) && !updateTask.CompletedDate.HasValue)
        {
            updateTask.CompletedDate = DateTime.Now;
        }

        await _tasks.SaveChangesAsync();
        await UpdateProjectProgressAsync(updateTask.ProjectId);

        return ServiceResult<ProjectTask>.Ok("Task updated successfully", updateTask);
    }

    public async Task<ServiceResult<object>> DeleteAsync(long id)
    {
        var task = await _tasks.Query().FirstOrDefaultAsync(currentTask => currentTask.TaskId == id && !currentTask.IsDeleted);
        if (task == null)
        {
            return ServiceResult<object>.NotFound("Task not found");
        }

        task.IsDeleted = true;
        task.DeletedAt = DateTime.Now;

        await _tasks.SaveChangesAsync();
        await UpdateProjectProgressAsync(task.ProjectId);

        return ServiceResult<object>.Ok("Task deleted successfully", null);
    }

    public async Task<ServiceResult<IEnumerable<TaskResponseDto>>> GetByPriorityAsync(string priority)
    {
        var tasks = await _tasks.Query()
            .AsNoTracking()
            .Where(task => !task.IsDeleted && task.Priority == priority)
            .Select(task => ToResponse(task))
            .ToListAsync();

        return ServiceResult<IEnumerable<TaskResponseDto>>.Ok($"Tasks with priority '{priority}' retrieved successfully", tasks);
    }

    public async Task<ServiceResult<IEnumerable<TaskResponseDto>>> GetByProjectAsync(long projectId)
    {
        var tasks = await _tasks.Query()
            .AsNoTracking()
            .Where(task => !task.IsDeleted && task.ProjectId == projectId)
            .Select(task => ToResponse(task))
            .ToListAsync();

        return ServiceResult<IEnumerable<TaskResponseDto>>.Ok("Tasks for the project retrieved successfully", tasks);
    }

    private async Task UpdateProjectProgressAsync(long projectId)
    {
        var project = await _projects.Query()
            .Include(currentProject => currentProject.Tasks)
            .FirstOrDefaultAsync(currentProject => currentProject.ProjectId == projectId);

        if (project == null)
        {
            return;
        }

        var activeTasks = project.Tasks.Where(task => !task.IsDeleted).ToList();
        project.TotalTasks = activeTasks.Count;
        project.CompletedTasks = activeTasks.Count(task => task.TaskStatus.Equals("Completed", StringComparison.OrdinalIgnoreCase));

        decimal totalAssignedScore = activeTasks.Sum(task => task.AssignedScore);
        if (totalAssignedScore > 0)
        {
            decimal weightedProgressSum = activeTasks.Sum(task => (task.ProgressPercentage / 100) * task.AssignedScore);
            project.ProgressPercentage = Math.Round((weightedProgressSum / totalAssignedScore) * 100, 2);
        }
        else
        {
            project.ProgressPercentage = activeTasks.Count > 0
                ? Math.Round((decimal)project.CompletedTasks / activeTasks.Count * 100, 2)
                : 0;
        }

        await _projects.SaveChangesAsync();
    }

    private static IQueryable<ProjectTask> ApplyFilters(IQueryable<ProjectTask> query, TaskFilterDto filters)
    {
        if (!string.IsNullOrWhiteSpace(filters.TaskStatus))
        {
            query = query.Where(task => task.TaskStatus == filters.TaskStatus);
        }

        if (!string.IsNullOrWhiteSpace(filters.ProjectStatus))
        {
            query = query.Where(task => task.Project.ProjectStatus == filters.ProjectStatus);
        }

        if (filters.MinAssignedScore.HasValue)
        {
            query = query.Where(task => task.AssignedScore >= filters.MinAssignedScore.Value);
        }

        if (filters.MaxAssignedScore.HasValue)
        {
            query = query.Where(task => task.AssignedScore <= filters.MaxAssignedScore.Value);
        }

        if (filters.MinEarnedScore.HasValue)
        {
            query = query.Where(task => task.EarnedScore >= filters.MinEarnedScore.Value);
        }

        if (filters.MaxEarnedScore.HasValue)
        {
            query = query.Where(task => task.EarnedScore <= filters.MaxEarnedScore.Value);
        }

        if (filters.RoleId.HasValue)
        {
            var roleScope = string.IsNullOrWhiteSpace(filters.RoleScope) ? "both" : filters.RoleScope.Trim();

            query = roleScope.ToLowerInvariant() switch
            {
                "student" => query.Where(task => task.Student.RoleId == filters.RoleId.Value),
                "faculty" => query.Where(task => task.Faculty.RoleId == filters.RoleId.Value),
                _ => query.Where(task => task.Student.RoleId == filters.RoleId.Value || task.Faculty.RoleId == filters.RoleId.Value)
            };
        }

        return query;
    }

    private static IQueryable<ProjectTask> ApplySorting(IQueryable<ProjectTask> query, TaskFilterDto filters)
    {
        var sortBy = filters.SortBy?.Trim().ToLowerInvariant() ?? "createdat";
        var isAscending = string.Equals(filters.SortDirection?.Trim(), "asc", StringComparison.OrdinalIgnoreCase);

        return sortBy switch
        {
            "duedate" => isAscending ? query.OrderBy(task => task.DueDate) : query.OrderByDescending(task => task.DueDate),
            "tasktitle" => isAscending ? query.OrderBy(task => task.TaskTitle) : query.OrderByDescending(task => task.TaskTitle),
            "taskstatus" => isAscending ? query.OrderBy(task => task.TaskStatus) : query.OrderByDescending(task => task.TaskStatus),
            "projectstatus" => isAscending ? query.OrderBy(task => task.Project.ProjectStatus) : query.OrderByDescending(task => task.Project.ProjectStatus),
            "projecttitle" => isAscending ? query.OrderBy(task => task.Project.ProjectTitle) : query.OrderByDescending(task => task.Project.ProjectTitle),
            "studentname" => isAscending ? query.OrderBy(task => task.Student.FullName) : query.OrderByDescending(task => task.Student.FullName),
            "facultyname" => isAscending ? query.OrderBy(task => task.Faculty.FullName) : query.OrderByDescending(task => task.Faculty.FullName),
            "assignedscore" => isAscending ? query.OrderBy(task => task.AssignedScore) : query.OrderByDescending(task => task.AssignedScore),
            "earnedscore" => isAscending ? query.OrderBy(task => task.EarnedScore) : query.OrderByDescending(task => task.EarnedScore),
            "progresspercentage" => isAscending ? query.OrderBy(task => task.ProgressPercentage) : query.OrderByDescending(task => task.ProgressPercentage),
            _ => isAscending ? query.OrderBy(task => task.CreatedAt) : query.OrderByDescending(task => task.CreatedAt)
        };
    }

    private static TaskResponseDto ToResponse(ProjectTask task)
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
