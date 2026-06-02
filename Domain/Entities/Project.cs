namespace StudentProjectManagementSystem.Domain.Entities;

public class Project
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

    public bool IsDeleted { get; set; }

    public long? DeletedBy { get; set; }

    public DateTime? DeletedAt { get; set; }

    public DateTime CreatedAt { get; set; }

    public long? CreatedBy { get; set; }

    public DateTime? UpdatedAt { get; set; }

    public long? UpdatedBy { get; set; }

    // Navigation Properties

    public User? CreatedByUser { get; set; }

    public User? UpdatedByUser { get; set; }

    public User? DeletedByUser { get; set; }

    public ICollection<ProjectAllocation> ProjectAllocations { get; set; }

    public ICollection<ProjectTask> Tasks { get; set; }
}