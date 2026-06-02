namespace StudentProjectManagementSystem.Application.DTOs;

public class TaskFilterDto
{
    public int PageNumber { get; set; } = 1;

    public int PageSize { get; set; } = 10;

    public string? TaskStatus { get; set; }

    public string? ProjectStatus { get; set; }

    public decimal? MinAssignedScore { get; set; }

    public decimal? MaxAssignedScore { get; set; }

    public decimal? MinEarnedScore { get; set; }

    public decimal? MaxEarnedScore { get; set; }

    public int? RoleId { get; set; }

    public string? RoleScope { get; set; }

    public string? SortBy { get; set; } = "CreatedAt";

    public string? SortDirection { get; set; } = "desc";
}

public class UserFilterDto
{
    public int PageNumber { get; set; } = 1;

    public int PageSize { get; set; } = 10;

    public bool? IsActive { get; set; }

    public int? RoleId { get; set; }

    public string? RoleName { get; set; }

    public string? SortBy { get; set; } = "CreatedAt";

    public string? SortDirection { get; set; } = "desc";
}