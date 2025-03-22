using System;

namespace TaskList.Core.Models.Commands
{
    /// <summary>
    /// Command to set or update the deadline of a task in the TaskList application.
    /// </summary>
    public class SetDeadlineCommand : BaseCommand
    {
        /// <summary>
        /// Gets the unique identifier of the task to update.
        /// </summary>
        public long TaskId { get; }

        /// <summary>
        /// Gets the new deadline for the task.
        /// </summary>
        public DateTime Deadline { get; }

        /// <summary>
        /// Initializes a new instance of the SetDeadlineCommand class.
        /// </summary>
        /// <param name="taskId">The unique identifier of the task to update.</param>
        /// <param name="deadline">The new deadline for the task.</param>
        public SetDeadlineCommand(long taskId, DateTime deadline) : base("set_deadline")
        {
            TaskId = taskId;
            Deadline = deadline;
        }
    }
}