namespace StudentProjectManagementSystem.Application.DTOs;

public class CreateUserDto
{
    public string FullName { get; set; }

    public string Email { get; set; }

    public string Password { get; set; }

    public string? MobileNumber { get; set; }

    public string? ProfilePicturePath { get; set; }

    public int RoleId { get; set; }
}

public class UpdateUserDto
{
    public string FullName { get; set; }

    public string? MobileNumber { get; set; }

    public string? ProfilePicturePath { get; set; }

    public bool IsActive { get; set; }

    public int RoleId { get; set; }
}

public class UserResponseDto
{
    public long UserId { get; set; }

    public string FullName { get; set; }

    public string Email { get; set; }

    public string? MobileNumber { get; set; }

    public string? ProfilePicturePath { get; set; }

    public bool IsActive { get; set; }

    public string RoleName { get; set; }

    public DateTime CreatedAt { get; set; }
}
