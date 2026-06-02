namespace StudentProjectManagementSystem.Application.DTOs;

public class CreateProjectAllocationDto
{
    public long ProjectId { get; set; }

    public long StudentId { get; set; }

    public long FacultyId { get; set; }

    public string AllocationStatus { get; set; }
}

public class UpdateProjectAllocationDto
{
    public string AllocationStatus { get; set; }

    public bool IsDeleted { get; set; }
}

public class ProjectAllocationResponseDto
{
    public long AllocationId { get; set; }

    public string ProjectTitle { get; set; }

    public string StudentName { get; set; }

    public string FacultyName { get; set; }

    public DateTime AssignedDate { get; set; }

    public string AllocationStatus { get; set; }
}
