namespace StudentProjectManagementSystem.Domain.Entities;

public class ProjectTask
{
    public long TaskId { get; set; }

    public long ProjectId { get; set; }

    public long StudentId { get; set; }

    public long FacultyId { get; set; }

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

    public bool IsDeleted { get; set; }

    public long? DeletedBy { get; set; }

    public DateTime? DeletedAt { get; set; }

    public DateTime CreatedAt { get; set; }

    public long? CreatedBy { get; set; }

    public DateTime? UpdatedAt { get; set; }

    public long? UpdatedBy { get; set; }

    // Navigation Properties

    public Project Project { get; set; }

    public User Student { get; set; }

    public User Faculty { get; set; }

    public User? CreatedByUser { get; set; }

    public User? UpdatedByUser { get; set; }

    public User? DeletedByUser { get; set; }
}