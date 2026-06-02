using FluentValidation;
using StudentProjectManagementSystem.Application.DTOs;

namespace StudentProjectManagementSystem.Application.Validators;

public class PermissionValidators : AbstractValidator<CreatePermissionDto>
{
    public PermissionValidators()
    {
        RuleFor(x => x.PermissionName)
            .NotEmpty().WithMessage("Name is required.")
            .Length(2, 100).WithMessage("Name must be between 2 and 100 characters.");

        RuleFor(x => x.ModuleName)
        .NotEmpty().WithMessage("Module name is required.")
        .Length(2, 100).WithMessage("Module name must be between 2 and 100 characters.");

        RuleFor(x => x.Description)
            .MaximumLength(500).WithMessage("Description must not exceed 500 characters.");
    }
}
public class UpdatePermissionValidaators : AbstractValidator<UpdatePermissionDto>
{
    public UpdatePermissionValidaators()
    {
        RuleFor(x => x.PermissionName)
            .NotEmpty().WithMessage("Name is required.")
            .Length(2, 100).WithMessage("Name must be between 2 and 100 characters.");
        RuleFor(x => x.ModuleName)
        .NotEmpty().WithMessage("Module name is required.")
        .Length(2, 100).WithMessage("Module name must be between 2 and 100 characters.");
        RuleFor(x => x.Description)
            .MaximumLength(500).WithMessage("Description must not exceed 500 characters.");
    }
}

public class ApiResponsePermissionValidator : AbstractValidator<PermissionResponseDto>
{
    public ApiResponsePermissionValidator()
    {
        RuleFor(x => x.PermissionName)
            .NotEmpty().WithMessage("Name is required.")
            .Length(2, 100).WithMessage("Name must be between 2 and 100 characters.");

        RuleFor(x => x.ModuleName)
            .NotEmpty().WithMessage("Module name is required.")
            .Length(2, 100).WithMessage("Module name must be between 2 and 100 characters.");

        RuleFor(x => x.Description)
            .MaximumLength(500).WithMessage("Description must not exceed 500 characters.");
    }
}
