namespace StudentProjectManagementSystem.Application.DTOs;

public class CreateRolePermissionDto
{
    public int RoleId { get; set; }

    public int PermissionId { get; set; }
}

public class UpdateRolePermissionDto
{
    public int RoleId { get; set; }

    public int PermissionId { get; set; }
}

public class RolePermissionResponseDto
{
    public long RolePermissionId { get; set; }

    public string RoleName { get; set; }

    public string PermissionName { get; set; }

    public DateTime CreatedAt { get; set; }
}
