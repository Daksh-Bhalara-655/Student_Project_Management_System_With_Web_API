using FluentValidation;
using StudentProjectManagementSystem.Application.DTOs;

namespace StudentProjectManagementSystem.Application.Validators;

public class RoleValidators : AbstractValidator<CreateRoleDto>
{
    public RoleValidators() { 
        RuleFor(x => x.RoleName)
            .NotEmpty()
            .WithMessage("Role name is required.")
            .Length(2, 100)
            .WithMessage("Role name must be between 2 and 100 characters.");

            RuleFor(x => x.Description)
            .Length(10,100)
            .WithMessage("Description must be between 10 and 100 characters.");
    }
}
public class UpdateRoleValidators : AbstractValidator<UpdateRoleDto>
{
    public UpdateRoleValidators()
    {
        RuleFor(x => x.RoleName)
            .NotEmpty()
            .WithMessage("Role name is required.")
            .Length(2, 100)
            .WithMessage("Role name must be between 2 and 100 characters.");
        RuleFor(x => x.Description)
            .Length(10, 100)
            .WithMessage("Description must be between 10 and 100 characters.");
    }
}
