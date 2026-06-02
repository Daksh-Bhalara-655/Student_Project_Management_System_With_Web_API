# Presentation Layer

## Purpose in Clean Architecture

The Presentation layer is the outer layer that receives requests and returns responses. In this project, Presentation is the ASP.NET Core Web API layer.

In ideal Clean Architecture, controllers should be thin. They should not contain business rules, database queries, duplicate checks, EF Core includes, password hashing, or project-progress calculations. Their job is to accept HTTP input, call Application services, and return HTTP output.

Ideal Presentation contents:

- API controllers
- Middleware
- API filters
- Request binding attributes
- Route definitions
- HTTP response conversion
- Swagger/OpenAPI setup when kept near API startup
- No EF Core queries
- No direct `DbContext` usage
- No business workflow logic

## Current Condition in This Project

This project currently has these Presentation folders:

```text
Presentation/
  Controllers/
    ApiControllerBase.cs
    PermissionController.cs
    ProjectAllocationController.cs
    ProjectController.cs
    RoleController.cs
    RolePermissionController.cs
    TaskController.cs
    UserController.cs
  Middlewares/
    ExceptionHandlingMiddleware.cs
```

The controllers were moved from the old top-level `Controllers` folder. The middleware was moved from the old top-level `Middlewares` folder.

## Thin Controller Example

Controllers now depend on Application service interfaces:

```csharp
[Route("api/[controller]")]
public class UserController : ApiControllerBase
{
    private readonly IUserService _userService;

    public UserController(IUserService userService)
    {
        _userService = userService;
    }
}
```

This is better than injecting `AppDbContext` directly into the controller because the controller no longer owns data access.

## Endpoint Example

Current controller actions are small:

```csharp
[HttpGet("{id:long}", Name = "GetUserById")]
public async Task<IActionResult> GetUserById([FromRoute] long id)
{
    return ToActionResult(await _userService.GetByIdAsync(id));
}
```

This is the correct Presentation responsibility:

1. Read route input.
2. Call the Application service.
3. Convert the service result into an HTTP response.

## Shared Controller Response Conversion

`ApiControllerBase` converts Application results to API responses:

```csharp
protected IActionResult ToActionResult<T>(ServiceResult<T> result)
{
    return StatusCode(result.StatusCode, new ApiResponse<T?>
    {
        Success = result.Success,
        Message = result.Message,
        StatusCode = result.StatusCode,
        Data = result.Data
    });
}
```

This prevents every controller from repeating `Ok`, `BadRequest`, `NotFound`, and `Conflict` response logic.

## Middleware

The exception middleware is also part of Presentation because it handles HTTP pipeline behavior:

```csharp
public async Task InvokeAsync(HttpContext httpContext)
{
    try
    {
        await _next(httpContext);
    }
    catch (Exception ex)
    {
        _logger.LogError($"Something went wrong: {ex}");
        await HandleExceptionAsync(httpContext, ex);
    }
}
```

Middleware is not a business rule. It is HTTP request pipeline infrastructure, so it belongs at the API boundary.

## Program.cs and Dependency Injection

`Program.cs` is the composition root. It connects interfaces to implementations:

```csharp
builder.Services.AddScoped(typeof(IRepository<>), typeof(Repository<>));
builder.Services.AddScoped<IUserService, UserService>();
builder.Services.AddScoped<IProjectService, ProjectService>();
builder.Services.AddScoped<IProjectTaskService, ProjectTaskService>();
```

This is how dependency injection connects Presentation, Application, and Infrastructure.

In a stricter multi-project architecture, `Program.cs` usually stays in the Web API project and calls extension methods such as:

```csharp
builder.Services.AddApplication();
builder.Services.AddInfrastructure(builder.Configuration);
```

For this single-project TA version, direct registration in `Program.cs` is simple and acceptable.

## What Is Good Right Now

- Controllers no longer directly use `AppDbContext`.
- Routes and binding attributes are preserved.
- Controllers depend on service interfaces, not concrete service classes.
- Middleware is separated under `Presentation/Middlewares`.
- Shared response conversion is handled in `ApiControllerBase`.

## Current Compromise

Some HTTP-style details still exist in Application result objects, such as numeric status codes. That makes controller code very simple, but a stricter Clean Architecture design might keep HTTP status codes completely inside Presentation.

Current practical version:

```csharp
return ToActionResult(await _userService.GetByIdAsync(id));
```

Stricter version might look like:

```csharp
var result = await _userService.GetByIdAsync(id);

return result.Status switch
{
    ResultStatus.NotFound => NotFound(...),
    ResultStatus.Success => Ok(...),
    _ => BadRequest(...)
};
```

The current version is easier for this project and still clearly separates controllers from business and database logic.

## Layer Rule for This Project

For this project, keep these types in Presentation:

- Controllers
- Middleware
- API-specific response conversion
- Route attributes
- HTTP request binding

Do not put these in Presentation:

- EF Core queries
- `DbContext`
- Repository implementations
- Entity relationship configuration
- Business use-case logic
- Domain entities
