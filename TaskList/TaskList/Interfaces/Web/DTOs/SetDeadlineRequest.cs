using System;

namespace TaskList.Interfaces.Web.DTOs
{
    /// <summary>
    /// Request model for setting a task's deadline.
    /// Used to receive deadline update requests from API clients.
    /// </summary>
    public class SetDeadlineRequest
    {
        /// <summary>
        /// Gets or sets the new deadline for the task.
        /// </summary>
        public DateTime Deadline { get; set; }
    }
}