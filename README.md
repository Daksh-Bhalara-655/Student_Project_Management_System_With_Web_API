# FluentValidation in ASP.NET Core Web API

## Introduction

FluentValidation is a popular .NET library that helps you validate objects using a clean, readable, and maintainable syntax.

Instead of writing validation logic inside controllers or DTOs, FluentValidation keeps validation rules in separate classes.

### Benefits

* Clean and readable code
* Separation of concerns
* Reusable validation rules
* Easy integration with ASP.NET Core
* Better error handling

---

# Step 1: Install FluentValidation

Install the required NuGet packages.

```bash
dotnet add package FluentValidation
dotnet add package FluentValidation.AspNetCore
```

Or using Package Manager:

```powershell
Install-Package FluentValidation
Install-Package FluentValidation.AspNetCore
```

---

# Step 2: Create DTO Classes

Example:

```csharp
public class CreateUserDto
{
    public string FullName { get; set; } = string.Empty;
    public string Email { get; set; } = string.Empty;
    public string Password { get; set; } = string.Empty;
    public int RoleId { get; set; }
    public string MobileNumber { get; set; } = string.Empty;
    public string? ProfilePicturePath { get; set; }
}
```

DTOs only contain data.

Validation rules should not be written inside DTOs.

---

# Step 3: Create Validator Class

Create a validator that inherits from:

```csharp
AbstractValidator<T>
```

Example:

```csharp
using FluentValidation;

public class UserValidators : AbstractValidator<CreateUserDto>
{
    public UserValidators()
    {
        RuleFor(x => x.FullName)
            .NotEmpty()
            .WithMessage("Full name is required.")
            .Length(2, 100)
            .WithMessage("Full name must be between 2 and 100 characters.");

        RuleFor(x => x.Email)
            .NotEmpty()
            .EmailAddress()
            .WithMessage("Invalid email format.");

        RuleFor(x => x.Password)
            .NotEmpty()
            .MinimumLength(6)
            .WithMessage("Password must be at least 6 characters.");

        RuleFor(x => x.RoleId)
            .GreaterThan(0)
            .WithMessage("Role ID must be greater than 0.");

        RuleFor(x => x.MobileNumber)
            .Matches(@"^\d{10}$")
            .WithMessage("Mobile number must contain exactly 10 digits.");
    }
}
```

---

# Step 4: Register FluentValidation

In `Program.cs`

```csharp
using FluentValidation;
using FluentValidation.AspNetCore;

builder.Services.AddControllers();

builder.Services.AddValidatorsFromAssemblyContaining<UserValidators>();

builder.Services.AddFluentValidationAutoValidation();
```

This automatically discovers all validator classes.

---

# Step 5: Inject Validator into Controller

Example:

```csharp
public class PermissionController(
    IPermissionService permissionService,
    IValidator<CreatePermissionDto> createValidator,
    IValidator<UpdatePermissionDto> updateValidator)
{
}
```

FluentValidation validators are injected through Dependency Injection.

---

# Step 6: Validate Request Data

Inside Controller:

```csharp
[HttpPost]
public async Task<IActionResult> CreatePermission(
    [FromBody] CreatePermissionDto permission)
{
    createValidator.ValidateAndThrow(permission);

    return Ok();
}
```

If validation fails, FluentValidation throws a `ValidationException`.

---

# Step 7: Common Validation Rules

## NotEmpty

```csharp
RuleFor(x => x.Name)
    .NotEmpty();
```

Checks that the value is not null or empty.

---

## Length

```csharp
RuleFor(x => x.Name)
    .Length(2, 100);
```

Checks minimum and maximum length.

---

## MinimumLength

```csharp
RuleFor(x => x.Password)
    .MinimumLength(6);
```

---

## MaximumLength

```csharp
RuleFor(x => x.Description)
    .MaximumLength(500);
```

---

## EmailAddress

```csharp
RuleFor(x => x.Email)
    .EmailAddress();
```

Validates email format.

---

## GreaterThan

```csharp
RuleFor(x => x.RoleId)
    .GreaterThan(0);
```

Validates numeric values.

---

## Matches

```csharp
RuleFor(x => x.MobileNumber)
    .Matches(@"^\d{10}$");
```

Validates using Regular Expressions.

---

## Must

Custom validation rule.

```csharp
RuleFor(x => x.ProjectStatus)
    .Must(status =>
        new[] { "Not Started", "In Progress", "Completed" }
        .Contains(status));
```

---

# Step 8: Date Validation

Example:

```csharp
RuleFor(x => x.StartDate)
    .LessThanOrEqualTo(x => x.EndDate)
    .WithMessage("Start date must be before end date.");
```

---

# Step 9: URL Validation

Example:

```csharp
RuleFor(x => x.ProfilePicturePath)
    .Must(path =>
        string.IsNullOrEmpty(path) ||
        Uri.IsWellFormedUriString(path, UriKind.Absolute))
    .WithMessage("Invalid URL.");
```

---

# Step 10: Custom Error Messages

```csharp
RuleFor(x => x.Email)
    .EmailAddress()
    .WithMessage("Please enter a valid email address.");
```

---

# Step 11: Example Validation Result

Request:

```json
{
  "fullName": "",
  "email": "abc",
  "password": "123"
}
```

Response:

```json
{
  "errors": {
    "FullName": [
      "Full name is required."
    ],
    "Email": [
      "Invalid email format."
    ],
    "Password": [
      "Password must be at least 6 characters."
    ]
  }
}
```

---

# Step 12: Project Validators Overview

## UserValidators

Validates:

* FullName
* Email
* Password
* RoleId
* MobileNumber
* ProfilePicturePath

---

## RoleValidators

Validates:

* RoleName
* Description

---

## PermissionValidators

Validates:

* PermissionName
* ModuleName
* Description

---

## RolePermissionValidators

Validates:

* RoleId
* PermissionId

---

## ProjectValidators

Validates:

* ProjectTitle
* Description
* StartDate
* EndDate
* ProjectStatus

---

## ProjectTaskValidators

Validates:

* ProjectId
* StudentId
* FacultyId
* TaskTitle
* TaskDescription
* TaskStatus
* Priority
* AssignedScore
* EarnedScore
* ProgressPercentage
* StartDate
* DueDate

---

## ProjectAllocationValidators

Validates:

* ProjectId
* StudentId
* FacultyId
* AllocationStatus

---

# Best Practices

✅ Keep validation logic separate from DTOs

✅ Create one validator per DTO

✅ Use meaningful error messages

✅ Reuse validation rules where possible

✅ Register validators using dependency injection

✅ Validate data before calling service methods

---

# Conclusion

FluentValidation provides a clean and maintainable way to validate incoming data in ASP.NET Core applications. By keeping validation logic inside dedicated validator classes, controllers remain clean, business logic stays focused, and validation becomes easier to manage as the application grows.
