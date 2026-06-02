# Application Layer

## Purpose in Clean Architecture

The Application layer contains the use cases of the system. It decides what the application can do, such as creating a user, assigning a student to a project, updating task progress, deleting a project, or reading paged data.

In ideal Clean Architecture, Application depends on Domain but does not depend on Infrastructure or Presentation. That means it should not know how HTTP works and should not know the exact database technology. It should use interfaces for repositories and services so the outer layers can provide implementations.

Ideal Application contents:

- Service interfaces
- Service implementations
- Repository interfaces
- DTOs
- Validators
- Use case logic
- Application result models
- Pagination models
- No EF Core `DbContext` implementation
- No controllers
- No ASP.NET middleware

## Current Condition in This Project

This project currently has these Application folders:

```text
Application/
  Common/
    PagedResult.cs
    ServiceResult.cs
  DTOs/
    ApiResponse.cs
    PermissionDto.cs
    ProjectAllocationDto.cs
    ProjectDto.cs
    ProjectTaskDto.cs
    QueryFiltersDto.cs
    RoleDto.cs
    RolePermissionDto.cs
    UserDto.cs
  Interfaces/
    Repositories/
      IRepository.cs
    Services/
      IPermissionService.cs
      IProjectAllocationService.cs
      IProjectService.cs
      IProjectTaskService.cs
      IRolePermissionService.cs
      IRoleService.cs
      IUserService.cs
  Services/
    PermissionService.cs
    ProjectAllocationService.cs
    ProjectService.cs
    ProjectTaskService.cs
    RolePermissionService.cs
    RoleService.cs
    UserService.cs
  Validators/
    PermissionValidators.cs
    ProjectAllocationValidators.cs
    ProjectTaskValidators.cs
    ProjectValidators.cs
    RolePermissionValidators.cs
    RoleValidators.cs
    UserValidators.cs
```

This is the layer where most controller logic was moved. The controllers now call services instead of directly using `AppDbContext`.

## Service Interface Example

Application exposes service contracts like this:

```csharp
public interface IUserService
{
    Task<ServiceResult<PagedResult<UserResponseDto>>> GetAllAsync(UserFilterDto filters);
    Task<ServiceResult<UserResponseDto>> GetByIdAsync(long id);
    Task<ServiceResult<UserResponseDto>> CreateAsync(CreateUserDto user);
    Task<ServiceResult<User>> UpdateAsync(long id, UpdateUserDto user);
    Task<ServiceResult<object>> DeleteAsync(long id);
}
```

This is useful because the Presentation layer only needs to know the service contract. It does not need to know how users are stored.

## Repository Interface Example

The repository abstraction is also in Application:

```csharp
public interface IRepository<T> where T : class
{
    IQueryable<T> Query();

    Task<T?> FindAsync(params object[] keyValues);

    Task AddAsync(T entity);

    void Remove(T entity);

    Task SaveChangesAsync();
}
```

The interface belongs in Application because the use cases need a persistence contract, but they should not depend directly on Infrastructure.

## Service Implementation Example

The service contains application rules that used to be inside controllers:

```csharp
var duplicateEmail = await _users.Query()
    .AnyAsync(currentUser => currentUser.Email == user.Email && !currentUser.IsDeleted);

if (duplicateEmail)
{
    return ServiceResult<UserResponseDto>.Conflict("A user with this email already exists");
}
```

This is application logic because it decides whether the create-user use case is allowed to continue.

## Result Wrapper

Services return `ServiceResult<T>`:

```csharp
public class ServiceResult<T>
{
    public bool Success => StatusCode >= 200 && StatusCode < 300;

    public string Message { get; set; } = string.Empty;

    public int StatusCode { get; set; }

    public T? Data { get; set; }
}
```

This keeps HTTP-like result information out of the service method signatures while still making controllers simple. In a stricter architecture, Application would use more neutral names such as `ResultStatus` instead of numeric HTTP status codes. For this TA project, the current approach is practical and easy to understand.

## Validators

FluentValidation validators are currently in Application:

```text
Application/Validators/UserValidators.cs
Application/Validators/ProjectValidators.cs
Application/Validators/ProjectTaskValidators.cs
```

This is appropriate because validation belongs close to request/use-case models. These validators check DTO input before the service performs business actions.

## What Is Good Right Now

- Controllers no longer directly contain EF Core queries.
- Service interfaces exist for each major feature area.
- Repository abstraction exists.
- DTOs are separated from Domain entities.
- Validation is grouped with Application use cases.
- Business workflow logic is now easier to test than when it was inside controllers.

## Current Compromise

The Application services currently use `IQueryable<T>` from the repository:

```csharp
var query = _users.Query()
    .AsNoTracking()
    .Where(user => !user.IsDeleted);
```

This is practical, but it leaks some query-building responsibility into Application. In a stricter Clean Architecture design, repository interfaces would expose more specific methods:

```csharp
Task<User?> GetActiveUserByEmailAsync(string email);
Task<IReadOnlyList<User>> SearchActiveUsersByNameAsync(string name);
```

That stricter version hides EF-style querying from Application. The current generic repository is acceptable for this project because it clearly introduces repository pattern and dependency inversion without creating too many files.

## Layer Rule for This Project

For this project, keep these types in Application:

- DTOs
- FluentValidation validators
- Service interfaces
- Service implementations
- Repository interfaces
- Result and pagination helper classes
- Use-case logic

Do not put these in Application:

- Controllers
- Middleware
- `DbContext`
- EF migrations
- Repository implementations
- SQL Server configuration
