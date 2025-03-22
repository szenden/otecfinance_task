using System;

namespace TaskList.Interfaces.Web.DTOs
{
    /// <summary>
    /// Data transfer object for task information.
    /// Used to send task data to API clients.
    /// </summary>
    public class TaskDto
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
    }
}