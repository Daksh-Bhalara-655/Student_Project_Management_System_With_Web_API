using FluentValidation;
using StudentProjectManagementSystem.Application.DTOs;

namespace StudentProjectManagementSystem.Application.Validators;

public class UserValidators : AbstractValidator<CreateUserDto>
{
    public UserValidators() {
        RuleFor(x => x.FullName)
            .NotEmpty()
            .WithMessage("Full name is required.")
            .Length(2, 100)
            .WithMessage("Full name must be between 2 and 100 characters.");

        RuleFor(x => x.Email)
            .NotEmpty()
            .WithMessage("Email is required.")
            .EmailAddress()
            .WithMessage("Invalid email format.");

        RuleFor(x => x.Password)
            .NotEmpty()
            .WithMessage("Password is required.")
            .MinimumLength(6)
            .WithMessage("Password must be at least 6 characters long.");

            RuleFor(x => x.RoleId)
            .NotEmpty()
            .WithMessage("Role ID is required.")
            .GreaterThan(0)
            .WithMessage("Role ID must be greater than 0.");

        RuleFor(x => x.MobileNumber)
            .NotEmpty()
            .WithMessage("Mobile number is required.")
            .Matches(@"^\d{10}$")
            .WithMessage("Invalid mobile number format.");

        RuleFor(x => x.ProfilePicturePath)
            .Must(path => string.IsNullOrEmpty(path) || Uri.IsWellFormedUriString(path, UriKind.Absolute))
            .WithMessage("Profile picture path must be a valid URL.");

    }
}

public class UpdateUserValidators : AbstractValidator<UpdateUserDto>
{
    public UpdateUserValidators() {
        RuleFor(x => x.FullName)
            .NotEmpty()
            .WithMessage("Full name is required.")
            .Length(2, 100)
            .WithMessage("Full name must be between 2 and 100 characters.");
        RuleFor(x => x.RoleId)
            .NotEmpty()
            .WithMessage("Role ID is required.")
            .GreaterThan(0)
            .WithMessage("Role ID must be greater than 0.");
        RuleFor(x => x.MobileNumber)
            .NotEmpty()
            .WithMessage("Mobile number is required.")
            .Matches(@"^\d{10}$")
            .WithMessage("Invalid mobile number format.");
        RuleFor(x => x.ProfilePicturePath)
            .Must(path => string.IsNullOrEmpty(path) || Uri.IsWellFormedUriString(path, UriKind.Absolute))
            .WithMessage("Profile picture path must be a valid URL.");
    }
}
