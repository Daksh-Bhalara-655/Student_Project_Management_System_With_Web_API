using FluentValidation;
using StudentProjectManagementSystem.Application.DTOs;

namespace StudentProjectManagementSystem.Application.Validators;

public class ProjectAllocationValidators
    : AbstractValidator<CreateProjectAllocationDto>
{
    public ProjectAllocationValidators()
    {
        RuleFor(x => x.ProjectId)
            .GreaterThan(0)
            .WithMessage("Project ID is required.");

        RuleFor(x => x.StudentId)
            .GreaterThan(0)
            .WithMessage("Student ID is required.");

        RuleFor(x => x.FacultyId)
            .GreaterThan(0)
            .WithMessage("Faculty ID is required.");

        RuleFor(x => x.AllocationStatus)
            .NotEmpty()
            .WithMessage("Allocation Status is required.")
            .Must(status => new[] { "Pending", "Approved", "Rejected" }
            .Contains(status))
            .WithMessage("Allocation status must be either 'Pending', 'Approved', or 'Rejected'.");
    }
}

public class UpdateProjectAllocationValidators
    : AbstractValidator<UpdateProjectAllocationDto>
{
    public UpdateProjectAllocationValidators()
    {
        RuleFor(x => x.AllocationStatus)
            .NotEmpty()
            .WithMessage("Allocation Status is required.")
            .Must(status => new[] { "Pending", "Approved", "Rejected" }
            .Contains(status))
            .WithMessage("Allocation status must be either 'Pending', 'Approved', or 'Rejected'.");
    }
}