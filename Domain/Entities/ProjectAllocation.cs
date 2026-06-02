namespace StudentProjectManagementSystem.Domain.Entities;

public class ProjectAllocation
{
    public long AllocationId { get; set; }

    public long ProjectId { get; set; }

    public long StudentId { get; set; }

    public long FacultyId { get; set; }

    public DateTime AssignedDate { get; set; }

    public string AllocationStatus { get; set; }

    public bool IsDeleted { get; set; }

    public DateTime CreatedAt { get; set; }

    public long? CreatedBy { get; set; }

    // Navigation Properties

    public Project Project { get; set; }

    public User Student { get; set; }

    public User Faculty { get; set; }

    public User? CreatedByUser { get; set; }
}