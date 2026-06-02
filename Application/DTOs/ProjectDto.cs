namespace StudentProjectManagementSystem.Application.DTOs;

public class CreateProjectDto
{
    public string ProjectTitle { get; set; }

    public string? Description { get; set; }

    public DateOnly? StartDate { get; set; }

    public DateOnly? EndDate { get; set; }

    public string ProjectStatus { get; set; }
}

public class UpdateProjectDto
{
    public string ProjectTitle { get; set; }

    public string? Description { get; set; }

    public DateOnly? StartDate { get; set; }

    public DateOnly? EndDate { get; set; }

    public string ProjectStatus { get; set; }
}

public class ProjectResponseDto
{
    public long ProjectId { get; set; }

    public string ProjectTitle { get; set; }

    public string? Description { get; set; }

    public DateOnly? StartDate { get; set; }

    public DateOnly? EndDate { get; set; }

    public string ProjectStatus { get; set; }

    public int TotalTasks { get; set; }

    public int CompletedTasks { get; set; }

    public decimal ProgressPercentage { get; set; }

    public DateTime CreatedAt { get; set; }
}
