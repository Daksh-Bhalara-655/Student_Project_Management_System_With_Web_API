# FluentValidation Complete Guide for Beginners

## What You Will Learn

- What validation is
- Why validation is important
- Different types of validation
- Validation flow in ASP.NET Core
- What FluentValidation is
- What AbstractValidator<T> means
- What RuleFor() does
- Most commonly used validation rules
- Real project examples

---

#  1: What is Validation?

Validation is the process of checking whether user input is correct before processing or saving it.

## Example Registration Form
<br>

<html lang="en">
<head>
    <form method="post">
        <label for="name">Name:</label>
        <input type="text" id="name" placeholder=""name="name"><br><br>
        <label for="email">Email:</label>
        <input type="text" id="email" 
        placeholder="abc" name="email"><br><br>
        <label for="password">Password:</label>
        <input type="password" id="password" 
        placeholder="12" name="password">
        <br><br>
<input type="submit" value="Submit"><br>
    </form>
</body>
</html>
<br>

Problems:

- Name is empty
- Email format is invalid
- Password is too short

Such data should not be stored in the database.

Example:

```json
{
  "fullName": "",
  "email": "abc",
  "password": "12"
}
```

Before saving data, we must verify that it follows the required rules.

This process is called Validation.

---

#  2: Why Do We Need Validation?

Without validation:

- Invalid data enters the database
- Security issues can occur
- Application errors increase
- User experience becomes poor

With validation:

- Clean data
- Better security
- Better user experience
- Fewer bugs

---

#  3: Types of Validation

## 1. Required Field Validation

```csharp
RuleFor(x => x.Name).NotEmpty();
```

## 2. Format Validation

```csharp
RuleFor(x => x.Email).EmailAddress();
```

## 3. Range Validation

```csharp
RuleFor(x => x.Age).InclusiveBetween(18,60);
```

## 4. Static List Validation

```csharp
RuleFor(x => x.Status)
    .Must(status =>
        new[] { "Pending", "Approved", "Rejected" }
        .Contains(status));
```

## 5. Database Validation

Check whether data already exists.

```csharp
RuleFor(x => x.Email)
    .MustAsync(async (email, cancellation) =>
    {
        return !await _dbContext.Users
            .AnyAsync(x => x.Email == email);
    });
```

## 6. Business Rule Validation

```csharp
RuleFor(x => x.Age)
    .GreaterThanOrEqualTo(18);
```

## 7. Cross Field Validation

```csharp
RuleFor(x => x.EndDate)
    .GreaterThan(x => x.StartDate);
```

## 8. File Validation

```csharp
RuleFor(x => x.File.Length)
    .LessThan(5 * 1024 * 1024);
```

## 9. API / External Validation

Examples:

- GST Number Validation
- Pincode Validation
- Address Validation

---

#  4: Validation Flow

## How Validation Works

```text
User Fills Form
        ↓
Request Sent
        ↓
DTO Created
        ↓
Validation Rules Run
        ↓
Any Error?
     /      \
   Yes      No
    |        |
Error     Service
Message     Layer
    |        |
 Stop    Database
```

### Explanation

1. User sends a request.
2. ASP.NET Core creates a DTO object.
3. FluentValidation runs validation rules.
4. If validation fails, an error response is returned.
5. If validation succeeds, the request goes to the Service Layer.
6. Finally, data is stored in the database.

---

#  5: What is FluentValidation?

FluentValidation is a .NET library that provides a clean and readable way to validate data.

```csharp
RuleFor(x => x.FullName)
    .NotEmpty();
```

---

#  6: Installing FluentValidation

```bash
dotnet add package FluentValidation
dotnet add package FluentValidation.AspNetCore
```

---

#  7: Understanding DTOs

```csharp
public class CreateUserDto
{
    public string FullName { get; set; }
    public string Email { get; set; }
}
```

DTO = Data Transfer Object

---

#  8: What is AbstractValidator<T>?

AbstractValidator<T> is the base class provided by FluentValidation that is used to create validation rules for a specific class (DTO).

```csharp
public class UserValidator
    : AbstractValidator<CreateUserDto>
{
}
```



Used to define validation rules for a specific DTO.

---

#  9: What is RuleFor()?

```csharp
RuleFor(x => x.Email)
```

Used to define validation rules for a property.

---

#  10: Creating Your First Validator

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

#  11: Registering Validators

```csharp
builder.Services.AddValidatorsFromAssemblyContaining<UserValidator>();

builder.Services.AddFluentValidationAutoValidation();
```

---

#  12: Using Validators in Controllers

```csharp
private readonly IValidator<CreateUserDto> _validator;
```

---

#  13: Validate() vs ValidateAndThrow()

```csharp
var result = validator.Validate(user);
```

```csharp
validator.ValidateAndThrow(user);
```

---

#  14: Most Important Validation Rules

| Rule Name | Checks | Sample Code |
|------------|---------|-------------|
| NotEmpty() | Null, Empty, Whitespace | RuleFor(x => x.Name).NotEmpty(); |
| NotNull() | Null value | RuleFor(x => x.Name).NotNull(); |
| EmailAddress() | Email format | RuleFor(x => x.Email).EmailAddress(); |
| MinimumLength() | Minimum characters | RuleFor(x => x.Password).MinimumLength(6); |
| MaximumLength() | Maximum characters | RuleFor(x => x.Name).MaximumLength(100); |
| Length() | Min & Max length | RuleFor(x => x.Name).Length(2,100); |
| GreaterThan() | Greater value | RuleFor(x => x.Age).GreaterThan(18); |
| GreaterThanOrEqualTo() | Greater or Equal | RuleFor(x => x.Age).GreaterThanOrEqualTo(18); |
| LessThan() | Less value | RuleFor(x => x.Age).LessThan(60); |
| LessThanOrEqualTo() | Less or Equal | RuleFor(x => x.StartDate).LessThanOrEqualTo(x => x.EndDate); |
| InclusiveBetween() | Range Validation | RuleFor(x => x.Score).InclusiveBetween(0,100); |
| Matches() | Regex Validation | RuleFor(x => x.Mobile).Matches(@"^\d{10}$"); |
| Must() | Custom Validation | RuleFor(x => x.Status).Must(...); |

---

#  15: Real Project Example

```csharp
public class UserValidator : AbstractValidator<CreateUserDto>
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

