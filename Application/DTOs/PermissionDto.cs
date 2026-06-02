namespace StudentProjectManagementSystem.Application.DTOs;

public class CreatePermissionDto
{
    public string PermissionName { get; set; }

    public string ModuleName { get; set; }

    public string? Description { get; set; }
}

public class UpdatePermissionDto
{
    public string PermissionName { get; set; }

    public string ModuleName { get; set; }

    public string? Description { get; set; }
}

public class PermissionResponseDto
{
    public int PermissionId { get; set; }

    public string PermissionName { get; set; }

    public string ModuleName { get; set; }

    public string? Description { get; set; }
}
