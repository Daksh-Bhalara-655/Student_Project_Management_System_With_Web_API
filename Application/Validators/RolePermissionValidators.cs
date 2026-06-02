using FluentValidation;
using StudentProjectManagementSystem.Application.DTOs;

namespace StudentProjectManagementSystem.Application.Validators;

public class RolePermissionValidators :AbstractValidator<CreateRolePermissionDto>
{
    public RolePermissionValidators() { 
    RuleFor(x => x.RoleId)
        .GreaterThan(0)
        .WithMessage("Role ID is required.");


    RuleFor(x => x.PermissionId)
        .GreaterThan(0)
        .WithMessage("Permission ID is required.");


    }
}

public class UpdateRolePermissionValidators : AbstractValidator<UpdateRolePermissionDto>
{
    public UpdateRolePermissionValidators()
    {
        RuleFor(x => x.RoleId)
            .GreaterThan(0)
            .WithMessage("Role ID is required.");
        RuleFor(x => x.PermissionId)
            .GreaterThan(0)
            .WithMessage("Permission ID is required.");
    }
}
