using FluentValidation;
using TaskList.Core.Models;

namespace TaskList.Core.Validation
{
    /// <summary>
    /// Validator class for Project entities
    /// </summary>
    public class ProjectValidator : AbstractValidator<Project>
    {
        /// <summary>
        /// Initializes validation rules for Project entities
        /// </summary>
        public ProjectValidator()
        {
            // Validate project name
            RuleFor(x => x.Name)
                .NotEmpty().WithMessage("Project name is required")
                .MaximumLength(100).WithMessage("Project name cannot exceed 100 characters")
                .Matches("^[a-zA-Z0-9\\s-_]+$").WithMessage("Project name can only contain letters, numbers, spaces, hyphens, and underscores");

            // Validate tasks collection
            RuleFor(x => x.Tasks)
                .NotNull().WithMessage("Tasks collection cannot be null");
        }
    }

    /// <summary>
    /// Validator class for ProjectTask entities
    /// </summary>
    public class ProjectTaskValidator : AbstractValidator<ProjectTask>
    {
        /// <summary>
        /// Initializes validation rules for ProjectTask entities
        /// </summary>
        public ProjectTaskValidator()
        {
            // Validate task description
            RuleFor(x => x.Description)
                .NotEmpty().WithMessage("Task description is required")
                .MaximumLength(500).WithMessage("Task description cannot exceed 500 characters");

            // Validate task deadline
            RuleFor(x => x.Deadline)
                .Must(deadline => !deadline.HasValue || deadline.Value.Date >= DateTime.Today)
                .WithMessage("Deadline cannot be in the past");

            // Validate project ID
            RuleFor(x => x.ProjectId)
                .GreaterThan(0).WithMessage("Project ID must be greater than 0");
        }
    }
}