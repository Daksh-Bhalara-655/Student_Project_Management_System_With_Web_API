using FluentValidation;
using StudentProjectManagementSystem.Application.DTOs;

namespace StudentProjectManagementSystem.Application.Validators;

public class ProjectValidators : AbstractValidator<CreateProjectDto>
{
    public ProjectValidators()
    {
        RuleFor(x => x.ProjectTitle)
            .NotEmpty().WithMessage("Project title is required.")
            .Length(2, 200).WithMessage("Project title must be between 2 and 200 characters.");
        RuleFor(x => x.Description)
            .MaximumLength(1000).WithMessage("Description must not exceed 1000 characters.");
        RuleFor(x => x.StartDate)
            .LessThanOrEqualTo(x => x.EndDate).WithMessage("Start date must be before or equal to end date.");
        RuleFor(x => x.ProjectStatus)
            .NotEmpty().WithMessage("Project status is required.")
            .Must(status => new[] { "Not Started", "In Progress", "Completed" }.Contains(status))
            .WithMessage("Project status must be 'Not Started', 'In Progress', or 'Completed'.");
    }
}

public class UpdateProjectValidators : AbstractValidator<UpdateProjectDto>
{
    public UpdateProjectValidators()
    {
        RuleFor(x => x.ProjectTitle)
            .NotEmpty().WithMessage("Project title is required.")
            .Length(2, 200).WithMessage("Project title must be between 2 and 200 characters.");
        RuleFor(x => x.Description)
            .MaximumLength(1000).WithMessage("Description must not exceed 1000 characters.");
        RuleFor(x => x.StartDate)
            .LessThanOrEqualTo(x => x.EndDate).WithMessage("Start date must be before or equal to end date.");
        RuleFor(x => x.ProjectStatus)
            .NotEmpty().WithMessage("Project status is required.")
            .Must(status => new[] { "Not Started", "In Progress", "Completed" }.Contains(status))
            .WithMessage("Project status must be 'Not Started', 'In Progress', or 'Completed'.");
    }
}
