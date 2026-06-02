using Microsoft.EntityFrameworkCore;
using StudentProjectManagementSystem.Application.Common;
using StudentProjectManagementSystem.Application.DTOs;
using StudentProjectManagementSystem.Application.Interfaces.Repositories;
using StudentProjectManagementSystem.Application.Interfaces.Services;
using StudentProjectManagementSystem.Domain.Entities;

namespace StudentProjectManagementSystem.Application.Services;

public class UserService : IUserService
{
    private readonly IRepository<User> _users;
    private readonly IRepository<Role> _roles;

    public UserService(IRepository<User> users, IRepository<Role> roles)
    {
        _users = users;
        _roles = roles;
    }

    public async Task<ServiceResult<PagedResult<UserResponseDto>>> GetAllAsync(UserFilterDto filters)
    {
        if (filters.PageNumber < 1 || filters.PageSize < 1)
        {
            return ServiceResult<PagedResult<UserResponseDto>>.BadRequest("Page number and page size must be greater than zero");
        }

        var query = _users.Query()
            .AsNoTracking()
            .Where(user => !user.IsDeleted);

        if (filters.IsActive.HasValue)
        {
            query = query.Where(user => user.IsActive == filters.IsActive.Value);
        }

        if (filters.RoleId.HasValue)
        {
            query = query.Where(user => user.RoleId == filters.RoleId.Value);
        }

        if (!string.IsNullOrWhiteSpace(filters.RoleName))
        {
            query = query.Where(user => user.Role.RoleName == filters.RoleName);
        }

        query = ApplySorting(query, filters);

        var totalCount = await query.CountAsync();
        var users = await query
            .Skip((filters.PageNumber - 1) * filters.PageSize)
            .Take(filters.PageSize)
            .Select(user => ToResponse(user))
            .ToListAsync();

        var result = new PagedResult<UserResponseDto>
        {
            Items = users,
            PageNumber = filters.PageNumber,
            PageSize = filters.PageSize,
            TotalCount = totalCount,
            TotalPages = (int)Math.Ceiling(totalCount / (double)filters.PageSize)
        };

        return ServiceResult<PagedResult<UserResponseDto>>.Ok("Users retrieved successfully", result);
    }

    public async Task<ServiceResult<UserResponseDto>> GetByIdAsync(long id)
    {
        var user = await _users.Query()
            .AsNoTracking()
            .Where(currentUser => !currentUser.IsDeleted && currentUser.UserId == id)
            .Select(currentUser => ToResponse(currentUser))
            .FirstOrDefaultAsync();

        return user == null
            ? ServiceResult<UserResponseDto>.NotFound("User not found")
            : ServiceResult<UserResponseDto>.Ok("User retrieved successfully", user);
    }

    public async Task<ServiceResult<UserResponseDto>> CreateAsync(CreateUserDto user)
    {
        if (string.IsNullOrWhiteSpace(user.FullName) || string.IsNullOrWhiteSpace(user.Email) || string.IsNullOrWhiteSpace(user.Password))
        {
            return ServiceResult<UserResponseDto>.BadRequest("Full name, email, and password are required");
        }

        var roleExists = await _roles.Query().AnyAsync(role => role.RoleId == user.RoleId);
        if (!roleExists)
        {
            return ServiceResult<UserResponseDto>.BadRequest("Invalid Role ID");
        }

        var duplicateEmail = await _users.Query().AnyAsync(currentUser => currentUser.Email == user.Email && !currentUser.IsDeleted);
        if (duplicateEmail)
        {
            return ServiceResult<UserResponseDto>.Conflict("A user with this email already exists");
        }

        var addUser = new User
        {
            FullName = user.FullName,
            Email = user.Email,
            PasswordHash = BCrypt.Net.BCrypt.HashPassword(user.Password),
            MobileNumber = user.MobileNumber,
            ProfilePicturePath = user.ProfilePicturePath,
            RoleId = user.RoleId,
            IsActive = true,
            IsDeleted = false,
            CreatedAt = DateTime.Now
        };

        await _users.AddAsync(addUser);
        await _users.SaveChangesAsync();

        var response = await _users.Query()
            .AsNoTracking()
            .Where(currentUser => currentUser.UserId == addUser.UserId)
            .Select(currentUser => ToResponse(currentUser))
            .FirstAsync();

        return ServiceResult<UserResponseDto>.Ok("User created successfully", response);
    }

    public async Task<ServiceResult<User>> UpdateAsync(long id, UpdateUserDto user)
    {
        var updateUser = await _users.Query().FirstOrDefaultAsync(currentUser => currentUser.UserId == id && !currentUser.IsDeleted);
        if (updateUser == null)
        {
            return ServiceResult<User>.NotFound("User not found");
        }

        var roleExists = await _roles.Query().AnyAsync(role => role.RoleId == user.RoleId);
        if (!roleExists)
        {
            return ServiceResult<User>.BadRequest("Invalid Role ID");
        }

        updateUser.FullName = user.FullName;
        updateUser.MobileNumber = user.MobileNumber;
        updateUser.ProfilePicturePath = user.ProfilePicturePath;
        updateUser.IsActive = user.IsActive;
        updateUser.RoleId = user.RoleId;
        updateUser.UpdatedAt = DateTime.Now;

        await _users.SaveChangesAsync();

        return ServiceResult<User>.Ok("User updated successfully", updateUser);
    }

    public async Task<ServiceResult<object>> DeleteAsync(long id)
    {
        var user = await _users.Query().FirstOrDefaultAsync(currentUser => currentUser.UserId == id && !currentUser.IsDeleted);
        if (user == null)
        {
            return ServiceResult<object>.NotFound("User not found");
        }

        user.IsDeleted = true;
        user.IsActive = false;
        user.DeletedAt = DateTime.Now;

        await _users.SaveChangesAsync();

        return ServiceResult<object>.Ok("User deleted successfully", null);
    }

    public async Task<ServiceResult<IEnumerable<UserResponseDto>>> SearchByNameAsync(string name)
    {
        var users = await _users.Query()
            .AsNoTracking()
            .Where(user => !user.IsDeleted && user.FullName.Contains(name))
            .Select(user => ToResponse(user))
            .ToListAsync();

        return ServiceResult<IEnumerable<UserResponseDto>>.Ok("Users found successfully", users);
    }

    public async Task<ServiceResult<IEnumerable<UserResponseDto>>> GetActiveAsync()
    {
        var users = await _users.Query()
            .AsNoTracking()
            .Where(user => !user.IsDeleted && user.IsActive)
            .Select(user => ToResponse(user))
            .ToListAsync();

        return ServiceResult<IEnumerable<UserResponseDto>>.Ok("Active users retrieved successfully", users);
    }

    private static IQueryable<User> ApplySorting(IQueryable<User> query, UserFilterDto filters)
    {
        var sortBy = filters.SortBy?.Trim().ToLowerInvariant() ?? "createdat";
        var isAscending = string.Equals(filters.SortDirection?.Trim(), "asc", StringComparison.OrdinalIgnoreCase);

        return sortBy switch
        {
            "fullname" => isAscending ? query.OrderBy(user => user.FullName) : query.OrderByDescending(user => user.FullName),
            "email" => isAscending ? query.OrderBy(user => user.Email) : query.OrderByDescending(user => user.Email),
            "rolename" => isAscending ? query.OrderBy(user => user.Role.RoleName) : query.OrderByDescending(user => user.Role.RoleName),
            "isactive" => isAscending ? query.OrderBy(user => user.IsActive) : query.OrderByDescending(user => user.IsActive),
            _ => isAscending ? query.OrderBy(user => user.CreatedAt) : query.OrderByDescending(user => user.CreatedAt)
        };
    }

    private static UserResponseDto ToResponse(User user)
    {
        return new UserResponseDto
        {
            UserId = user.UserId,
            FullName = user.FullName,
            Email = user.Email,
            MobileNumber = user.MobileNumber,
            ProfilePicturePath = user.ProfilePicturePath,
            IsActive = user.IsActive,
            RoleName = user.Role.RoleName,
            CreatedAt = user.CreatedAt
        };
    }
}
