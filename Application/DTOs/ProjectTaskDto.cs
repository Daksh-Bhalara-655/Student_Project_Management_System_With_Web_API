namespace StudentProjectManagementSystem.Application.DTOs;

public class CreateTaskDto
{
    public long ProjectId { get; set; }

    public long StudentId { get; set; }

    public long FacultyId { get; set; }

    public string TaskTitle { get; set; }

    public string? TaskDescription { get; set; }

    public string TaskStatus { get; set; }

    public string Priority { get; set; }

    public decimal AssignedScore { get; set; }

    public DateTime? StartDate { get; set; }

    public DateTime? DueDate { get; set; }
}

public class UpdateTaskDto
{
    public string TaskTitle { get; set; }

    public string? TaskDescription { get; set; }

    public string TaskStatus { get; set; }

    public string Priority { get; set; }

    public decimal AssignedScore { get; set; }

    public decimal EarnedScore { get; set; }

    public decimal ProgressPercentage { get; set; }

    public DateTime? StartDate { get; set; }

    public DateTime? DueDate { get; set; }

    public DateTime? CompletedDate { get; set; }

    public string? FacultyRemarks { get; set; }

    public string? StudentRemarks { get; set; }
}

public class TaskResponseDto
{
    public long TaskId { get; set; }

    public string ProjectTitle { get; set; }

    public string StudentName { get; set; }

    public string FacultyName { get; set; }

    public string TaskTitle { get; set; }

    public string? TaskDescription { get; set; }

    public string TaskStatus { get; set; }

    public string Priority { get; set; }

    public decimal AssignedScore { get; set; }

    public decimal EarnedScore { get; set; }

    public decimal ProgressPercentage { get; set; }

    public DateTime? DueDate { get; set; }

    public DateTime CreatedAt { get; set; }
}
