# FluentValidation Complete Guide for Beginners

## What You Will Learn

After completing this guide, you will understand:

* What validation is
* Why validation is important
* What FluentValidation is
* What `AbstractValidator<T>` means
* What `RuleFor()` does
* How validation works in ASP.NET Core
* Most commonly used validation rules
* How to use FluentValidation in real projects

---

# Chapter 1: What is Validation?

Imagine a user fills out a registration form.

```json
{
  "fullName": "",
  "email": "abc",
  "password": "12"
}
```

Problems:

* Name is empty
* Email format is incorrect
* Password is too short

Should we save this data to the database?

**No!**

Before saving data, we must check whether it is valid.

This process is called **Validation**.

---

# Chapter 2: Why Do We Need Validation?

Without validation:

* Invalid data enters the database
* Application crashes may occur
* Security issues can arise
* Bad user experience

With validation:

✅ Clean data

✅ Better security

✅ Better user experience

✅ Fewer bugs

---

# Chapter 3: What is FluentValidation?

FluentValidation is a .NET library used to validate data using readable rules.

Instead of writing:

```csharp
if(string.IsNullOrEmpty(user.FullName))
{
    throw new Exception("Name is required");
}
```

We write:

```csharp
RuleFor(x => x.FullName)
    .NotEmpty();
```

Much cleaner and easier to maintain.

---

# Chapter 4: Installing FluentValidation

Install packages:

```bash
dotnet add package FluentValidation
dotnet add package FluentValidation.AspNetCore
```

---

# Chapter 5: Understanding DTOs

Example:

```csharp
public class CreateUserDto
{
    public string FullName { get; set; }
    public string Email { get; set; }
}
```

DTO stands for:

**Data Transfer Object**

A DTO carries data between client and server.

---

# Chapter 6: What is AbstractValidator<T>?

This is the base class of every validator.

Example:

```csharp
public class UserValidator
    : AbstractValidator<CreateUserDto>
{
}
```

Meaning:

```text
I am creating validation rules
for CreateUserDto.
```

Think of it like:

```text
Teacher → Checks Students

Validator → Checks DTO
```

---

# Chapter 7: What is RuleFor()?

RuleFor() tells FluentValidation:

```text
Apply validation to this property.
```

Example:

```csharp
RuleFor(x => x.Email)
```

Meaning:

```text
Create rules for Email field.
```

---

# Chapter 8: Creating Your First Validator

DTO:

```csharp
public class CreateUserDto
{
    public string FullName { get; set; }
}
```

Validator:

```csharp
public class UserValidator
    : AbstractValidator<CreateUserDto>
{
    public UserValidator()
    {
        RuleFor(x => x.FullName)
            .NotEmpty()
            .WithMessage("Full name is required.");
    }
}
```

---

# Chapter 9: Registering Validators

Inside Program.cs:

```csharp
builder.Services.AddValidatorsFromAssemblyContaining<UserValidator>();

builder.Services.AddFluentValidationAutoValidation();
```

This registers all validators automatically.

---

# Chapter 10: Using Validators in Controllers

```csharp
public class UserController
{
    private readonly IValidator<CreateUserDto> _validator;

    public UserController(
        IValidator<CreateUserDto> validator)
    {
        _validator = validator;
    }
}
```

---

# Chapter 11: What is IValidator<T>?

Example:

```csharp
IValidator<CreateUserDto>
```

Meaning:

```text
A validator that validates
CreateUserDto objects.
```

ASP.NET automatically injects the correct validator.

---

# Chapter 12: What is Validate()?

```csharp
var result = validator.Validate(user);
```

Returns:

```csharp
ValidationResult
```

Check:

```csharp
if(result.IsValid)
{
    // Data is valid
}
```

---

# Chapter 13: What is ValidateAndThrow()?

```csharp
validator.ValidateAndThrow(user);
```

If valid:

```text
Continue execution
```

If invalid:

```text
Throw ValidationException
```

---

# Chapter 14: Most Important Validation Rules

## NotEmpty()

```csharp
RuleFor(x => x.FullName)
    .NotEmpty();
```

Checks:

* Not null
* Not empty
* Not whitespace

---

## EmailAddress()

```csharp
RuleFor(x => x.Email)
    .EmailAddress();
```

Checks email format.

Valid:

```text
john@gmail.com
```

Invalid:

```text
johngmail.com
```

---

## MinimumLength()

```csharp
RuleFor(x => x.Password)
    .MinimumLength(6);
```

---

## Length()

```csharp
RuleFor(x => x.Name)
    .Length(2,100);
```

---

## GreaterThan()

```csharp
RuleFor(x => x.RoleId)
    .GreaterThan(0);
```

---

## Matches()

```csharp
RuleFor(x => x.MobileNumber)
    .Matches(@"^\d{10}$");
```

Checks 10-digit phone numbers.

---

## InclusiveBetween()

```csharp
RuleFor(x => x.Score)
    .InclusiveBetween(0,100);
```

Valid:

```text
0
50
100
```

Invalid:

```text
101
```

---

## LessThanOrEqualTo()

```csharp
RuleFor(x => x.StartDate)
    .LessThanOrEqualTo(x => x.EndDate);
```

Checks date range.

---

## Must()

Custom validation.

```csharp
RuleFor(x => x.Status)
    .Must(status =>
        new[]
        {
            "Pending",
            "Approved",
            "Rejected"
        }
        .Contains(status));
```

---

# Chapter 15: Validation Flow

```text
Client Request
       ↓
DTO Created
       ↓
Validator Runs
       ↓
Validation Success?
       ↓
 YES          NO
 ↓             ↓
Service     Error
 Layer      Message
       ↓
Database
```

---

# Chapter 16: Real Project Example

User Validator:

```csharp
public class UserValidator
    : AbstractValidator<CreateUserDto>
{
    public UserValidator()
    {
        RuleFor(x => x.FullName)
            .NotEmpty()
            .Length(2,100);

        RuleFor(x => x.Email)
            .NotEmpty()
            .EmailAddress();

        RuleFor(x => x.Password)
            .MinimumLength(6);

        RuleFor(x => x.RoleId)
            .GreaterThan(0);

        RuleFor(x => x.MobileNumber)
            .Matches(@"^\d{10}$");
    }
}
```

---

# Chapter 17: Best Practices

✅ One validator per DTO

✅ Use meaningful error messages

✅ Keep validation separate from controllers

✅ Use `InclusiveBetween()` for score ranges

✅ Use `ValidateAndThrow()` in APIs

✅ Register validators using Dependency Injection

---

# Chapter 18: Interview Questions

### What is FluentValidation?

A library used to validate objects using fluent syntax.

### What is AbstractValidator<T>?

Base class used to create validators.

### What is RuleFor()?

Creates validation rules for a property.

### Difference between NotNull and NotEmpty?

NotNull checks only null.

NotEmpty checks null, empty string, whitespace, and default values.

### Difference between Validate() and ValidateAndThrow()?

Validate returns a result object.

ValidateAndThrow throws an exception when validation fails.

---

# Conclusion

FluentValidation is a powerful and clean way to validate data in ASP.NET Core applications. It keeps validation logic separate from business logic and makes applications easier to maintain, test, and scale.
