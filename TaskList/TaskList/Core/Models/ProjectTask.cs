using System;

namespace TaskList.Core.Models
{
    /// <summary>
    /// Represents a task within a project in the TaskList application.
    /// Contains task details such as description, completion status, and deadline.
    /// </summary>
    public class ProjectTask
    {
        /// <summary>
        /// Gets or sets the unique identifier of the task.
        /// </summary>
        public long Id { get; set; }

        /// <summary>
        /// Gets or sets the description of the task.
        /// </summary>
        public string Description { get; set; }

        /// <summary>
        /// Gets or sets a value indicating whether the task is completed.
        /// </summary>
        public bool Done { get; set; }

        /// <summary>
        /// Gets or sets the deadline for the task.
        /// </summary>
        public DateTime? Deadline { get; set; }

        /// <summary>
        /// Gets or sets the name of the project this task belongs to.
        /// </summary>
        public string ProjectName { get; set; }

        /// <summary>
        /// Initializes a new instance of the ProjectTask class.
        /// </summary>
        /// <param name="id">The unique identifier of the task.</param>
        /// <param name="description">The description of the task.</param>
        /// <param name="projectName">The name of the project this task belongs to.</param>
        public ProjectTask(long id, string description, string projectName)
        {
            Id = id;
            Description = description;
            ProjectName = projectName;
            Done = false;
        }

        /// <summary>
        /// Returns a string representation of the task.
        /// </summary>
        /// <returns>A formatted string containing the task's ID, description, deadline (if set), and completion status.</returns>
        public override string ToString()
        {
            var result = $"{Id}: {Description}";
            if (Deadline.HasValue)
            {
                result += $", {Deadline.Value:dd-MM-yyyy}";
            }
            if (Done)
            {
                result += " [x]";
            }
            return result;
        }
    }
}