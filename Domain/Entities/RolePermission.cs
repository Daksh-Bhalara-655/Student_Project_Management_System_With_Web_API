namespace StudentProjectManagementSystem.Domain.Entities;

public class RolePermission
{
    public long RolePermissionId { get; set; }

    public int RoleId { get; set; }

    public int PermissionId { get; set; }

    public DateTime CreatedAt { get; set; }

    // Navigation Properties

    public Role Role { get; set; }

    public Permission Permission { get; set; }
}