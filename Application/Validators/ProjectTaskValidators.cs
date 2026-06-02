using FluentValidation;
using StudentProjectManagementSystem.Application.DTOs;

namespace StudentProjectManagementSystem.Application.Validators;

public class ProjectTaskValidators : AbstractValidator<CreateTaskDto>
{
    public ProjectTaskValidators()
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

        RuleFor(x => x.TaskTitle)
            .NotEmpty()
            .WithMessage("Task title is required.")
            .Length(2, 200)
            .WithMessage("Task title must be between 2 and 200 characters.");

        RuleFor(x => x.TaskDescription)
            .Length(2, 200).WithMessage("Task description must be between 2 and 200 characters.");

        RuleFor(x => x.TaskStatus)
            .NotEmpty()
            .WithMessage("Task status is required.")
            .Must(status => new[] { "Not Started", "In Progress", "Completed" }.Contains(status))
            .WithMessage("Task status must be 'Not Started', 'In Progress', or 'Completed'.");

        RuleFor(x => x.Priority)
            .NotEmpty()
            .WithMessage("Task priority is required.")
            .Must(priority => new[] { "Low", "Medium", "High" }.Contains(priority))
            .WithMessage("Task priority must be 'Low', 'Medium', or 'High'.");

        RuleFor(x => x.AssignedScore)
            .Must(score => new[] { new[] { 0m, 100m } }.Contains(new[] { score }))
            .WithMessage("Assigned score must be between 0 and 100.");

        RuleFor(x => x.StartDate)
        .NotEmpty()
        .WithMessage("Start date is required.")
        .LessThanOrEqualTo(x => x.DueDate)
        .Must(dueDate => new[] { dueDate }.Contains(dueDate))
        .WithMessage("Start date must be before or equal to due date.");

        RuleFor(x => x.DueDate)
            .NotEmpty()
            .WithMessage("Due date is required.")
            .GreaterThanOrEqualTo(x => x.StartDate)
            .Must(startDate => new[] { startDate }.Contains(startDate))
            .WithMessage("Due date must be after or equal to start date.");
    }


    public class UpdateProjectTaskValidators : AbstractValidator<UpdateTaskDto>
    {
        public UpdateProjectTaskValidators()
        {
            RuleFor(x => x.TaskTitle)
                .NotEmpty()
                .WithMessage("Task title is required.")
                .Length(2, 200)
                .WithMessage("Task title must be between 2 and 200 characters.");

            RuleFor(x => x.TaskDescription)
                .Length(2, 200).WithMessage("Task description must be between 2 and 200 characters.");

            RuleFor(x => x.TaskStatus)
                .NotEmpty()
                .WithMessage("Task status is required.")
                .Must(status => new[] { "Not Started", "In Progress", "Completed" }.Contains(status))
                .WithMessage("Task status must be 'Not Started', 'In Progress', or 'Completed'.");

            RuleFor(x => x.Priority)
                .NotEmpty()
                .WithMessage("Task priority is required.")
                .Must(priority => new[] { "Low", "Medium", "High" }.Contains(priority))
                .WithMessage("Task priority must be 'Low', 'Medium', or 'High'.");

            RuleFor(x => x.AssignedScore)
                .Must(score => new[] { new[] { 0m, 100m } }.Contains(new[] { score }))
                .WithMessage("Assigned score must be between 0 and 100.");

            RuleFor(x => x.EarnedScore)
                .Must(score => new[] { new[] { 0m, 100m } }.Contains(new[] { score }))
                .WithMessage("Earned score must be between 0 and 100.");

            RuleFor(x => x.ProgressPercentage)
                .Must(percentage => new[] { new[] { 0m, 100m } }.Contains(new[] { percentage }))
                .WithMessage("Progress percentage must be between 0 and 100.");

            RuleFor(x => x.StartDate)
                .LessThanOrEqualTo(x => x.DueDate)
                .Must(dueDate => new[] { dueDate }.Contains(dueDate))
                .WithMessage("Start date must be before or equal to due date.");

            RuleFor(x => x.DueDate)
                .GreaterThanOrEqualTo(x => x.StartDate)
                .Must(startDate => new[] { startDate }.Contains(startDate))
                .WithMessage("Due date must be after or equal to start date.");
        }

    }
}