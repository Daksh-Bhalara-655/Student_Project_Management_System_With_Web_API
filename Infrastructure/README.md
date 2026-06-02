# Infrastructure Layer

## Purpose in Clean Architecture

The Infrastructure layer contains technical details. It handles how the application talks to databases, files, external APIs, email services, authentication providers, and other outside systems.

In ideal Clean Architecture, Infrastructure depends inward on Application and Domain. Application defines interfaces, and Infrastructure implements them.

Ideal Infrastructure contents:

- EF Core `DbContext`
- EF Core migrations
- Repository implementations
- External service implementations
- Database configuration
- File storage implementations
- Email/SMS implementations
- No controllers
- No request DTO validation
- No business use-case orchestration

## Current Condition in This Project

This project currently has these Infrastructure folders:

```text
Infrastructure/
  Data/
    AppDbContext.cs
  Migrations/
    20260520060348_Initial.cs
    20260520060348_Initial.Designer.cs
    AppDbContextModelSnapshot.cs
  Repositories/
    Repository.cs
```

This is the correct place for EF Core and database-related code.

## DbContext Example

The `AppDbContext` is now inside Infrastructure:

```csharp
namespace StudentProjectManagementSystem.Infrastructure.Data;

public class AppDbContext : DbContext
{
    public DbSet<User> Users { get; set; }
    public DbSet<Role> Roles { get; set; }
    public DbSet<Permission> Permissions { get; set; }
    public DbSet<RolePermission> RolePermissions { get; set; }
    public DbSet<Project> Projects { get; set; }
    public DbSet<ProjectAllocation> ProjectAllocations { get; set; }
    public DbSet<ProjectTask> Tasks { get; set; }
}
```

This belongs in Infrastructure because `DbContext` is an EF Core detail. The Domain layer should not own database mapping.

## Mapping Configuration

The current `AppDbContext` uses Fluent API configuration:

```csharp
modelBuilder.Entity<User>(entity =>
{
    entity.HasKey(e => e.UserId);

    entity.Property(e => e.FullName)
        .IsRequired()
        .HasMaxLength(150);

    entity.HasIndex(e => e.Email)
        .IsUnique();
});
```

This is good because database rules such as max length, indexes, delete behavior, and relationships are infrastructure concerns.

## Repository Implementation Example

Application defines `IRepository<T>`, and Infrastructure implements it:

```csharp
public class Repository<T>(
    AppDbContext dbContext
) : IRepository<T> where T : class
{
    private readonly AppDbContext _dbContext = dbContext;
    private readonly DbSet<T> _dbSet = dbContext.Set<T>();

    public IQueryable<T> Query()
    {
        return _dbSet;
    }

    public async Task SaveChangesAsync()
    {
        await _dbContext.SaveChangesAsync();
    }
}
```

This follows dependency inversion. Application depends on the repository interface, while Infrastructure supplies the EF Core implementation.

## Migrations

The EF Core migration files are correctly placed in Infrastructure:

```text
Infrastructure/Migrations/
```

Migrations are not domain rules. They are database history and schema-change details, so they belong in Infrastructure.

## What Is Good Right Now

- `AppDbContext` is no longer in a top-level `Data` folder.
- EF migrations are not mixed with controllers or DTOs.
- Repository implementation is separate from repository interface.
- Infrastructure depends on Application contracts and Domain entities.
- SQL Server registration is done in the composition root through `Program.cs`.

## Current Compromise

This project is still one `.csproj`, so all folders compile into one assembly. In a stricter Clean Architecture solution, these would usually be separate projects:

```text
StudentProjectManagementSystem.Domain
StudentProjectManagementSystem.Application
StudentProjectManagementSystem.Infrastructure
StudentProjectManagementSystem.Presentation
```

With separate projects, C# project references can enforce dependency direction. In the current TA project, the folder structure and namespaces show the architecture clearly, but the compiler cannot fully prevent a wrong dependency from being added later.

## Layer Rule for This Project

For this project, keep these types in Infrastructure:

- `AppDbContext`
- EF Core configuration
- EF Core migrations
- Repository implementations
- Database-specific logic
- External technical implementations

Do not put these in Infrastructure:

- Controllers
- HTTP response formatting
- DTO validators
- Business use-case service interfaces
- Pure Domain entities
