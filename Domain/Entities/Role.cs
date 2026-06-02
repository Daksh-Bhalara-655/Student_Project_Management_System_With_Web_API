namespace StudentProjectManagementSystem.Domain.Entities;

public class Role
{
    public int RoleId { get; set; }

    public string RoleName { get; set; }

    public string? Description { get; set; }

    // Navigation Properties

    public ICollection<User> Users { get; set; }

    public ICollection<RolePermission> RolePermissions { get; set; }
}