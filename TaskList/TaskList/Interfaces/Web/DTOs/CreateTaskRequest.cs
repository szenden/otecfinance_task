using System;

namespace TaskList.Interfaces.Web.DTOs
{
    /// <summary>
    /// Request model for creating a new task.
    /// Used to receive task creation requests from API clients.
    /// </summary>
    public class CreateTaskRequest
    {
        /// <summary>
        /// Gets or sets the description of the task to create.
        /// </summary>
        public string Description { get; set; }

        /// <summary>
        /// Gets or sets the deadline for the task.
        /// </summary>
        public DateTime? Deadline { get; set; }
    }
}