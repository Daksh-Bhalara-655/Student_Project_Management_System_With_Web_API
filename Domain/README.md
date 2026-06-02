# Domain Layer

## Purpose in Clean Architecture

The Domain layer is the center of the application. It should contain the business objects that describe the student project management problem itself: users, roles, projects, allocations, tasks, permissions, and role permissions.

In an ideal Clean Architecture design, the Domain layer should be the most independent layer. It should not know about ASP.NET Core, Entity Framework Core, controllers, database tables, HTTP requests, validation libraries, or dependency injection. Other layers can depend on Domain, but Domain should not depend on them.

Ideal Domain contents:

- Entity classes
- Value objects
- Domain enums
- Domain rules that are always true for the business
- Domain events, if the project grows
- No database code
- No controller code
- No DTOs used only for API input/output

Ideal dependency direction:

```text
Presentation -> Application -> Domain
Infrastructure -> Application -> Domain
Domain -> no project-specific outer layer
```

## Current Condition in This Project

This project currently has a `Domain` folder with an `Entities` folder inside it. The entities were moved from the old `Models` folder.

Current Domain files:

```text
Domain/
  Entities/
    Permission.cs
    Project.cs
    ProjectAllocation.cs
    ProjectTask.cs
    Role.cs
    RolePermission.cs
    User.cs
```

The current namespace is correct for this layer:

```csharp
namespace StudentProjectManagementSystem.Domain.Entities;
```

This is a good first step because controllers and EF Core infrastructure no longer use the old `StudentProjectManagementSystem.Models` namespace.

## Current Entity Example

`Project` is a Domain entity because it represents a main business concept in the system.

```csharp
public class Project
{
    public long ProjectId { get; set; }

    public string ProjectTitle { get; set; }

    public string? Description { get; set; }

    public DateOnly? StartDate { get; set; }

    public DateOnly? EndDate { get; set; }

    public string ProjectStatus { get; set; }

    public int TotalTasks { get; set; }

    public int CompletedTasks { get; set; }

    public decimal ProgressPercentage { get; set; }
}
```

This belongs in Domain because these properties describe what a project is, not how a project is stored or returned from an API.

## Navigation Properties

The entities currently include navigation properties for EF Core relationships:

```csharp
public ICollection<ProjectAllocation> ProjectAllocations { get; set; }

public ICollection<ProjectTask> Tasks { get; set; }
```

This is acceptable for a teaching project and for a single-project Clean Architecture folder structure. In a stricter enterprise Clean Architecture implementation, Domain entities often avoid EF-specific assumptions as much as possible, but navigation properties are commonly kept when Entity Framework is used directly with domain entities.

## What Is Good Right Now

- Entity classes are separated from controllers and API DTOs.
- The namespace clearly identifies the layer: `StudentProjectManagementSystem.Domain.Entities`.
- Domain does not reference controllers, services, repositories, or middleware.
- The same entities are reused by Application services and Infrastructure persistence.

## What Can Be Improved Later

The current entities use nullable-enabled C# but many non-nullable properties are not initialized:

```csharp
public string ProjectTitle { get; set; }
public string ProjectStatus { get; set; }
```

That is why the project builds with nullable warnings. Later, these can be improved with `required`, constructors, or default values:

```csharp
public required string ProjectTitle { get; set; }
public required string ProjectStatus { get; set; }
```

Another possible improvement is to move repeated audit fields into a base entity:

```csharp
public abstract class AuditableEntity
{
    public DateTime CreatedAt { get; set; }
    public long? CreatedBy { get; set; }
    public DateTime? UpdatedAt { get; set; }
    public long? UpdatedBy { get; set; }
}
```

That would reduce repeated code in `User`, `Project`, and `ProjectTask`.

## Layer Rule for This Project

For this project, keep these types in Domain:

- Real business entities
- Shared domain state
- Business concepts that should exist even if the API or database changes

Do not put these in Domain:

- Controllers
- DTOs
- Validators
- EF Core `DbContext`
- Migrations
- Repository implementations
- Middleware
