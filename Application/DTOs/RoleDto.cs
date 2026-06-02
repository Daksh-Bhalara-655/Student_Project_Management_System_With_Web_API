namespace StudentProjectManagementSystem.Application.DTOs;

public class CreateRoleDto
{
    public string RoleName { get; set; }

    public string? Description { get; set; }
}

public class UpdateRoleDto
{
    public string RoleName { get; set; }

    public string? Description { get; set; }
}

public class RoleResponseDto
{
    public int RoleId { get; set; }

    public string RoleName { get; set; }

    public string? Description { get; set; }
}
