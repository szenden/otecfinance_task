namespace TaskList.Core.Models.Commands
{
    /// <summary>
    /// Command to update the completion status of a task in the TaskList application.
    /// </summary>
    public class CheckTaskCommand : BaseCommand
    {
        /// <summary>
        /// Gets the unique identifier of the task to update.
        /// </summary>
        public long TaskId { get; }

        /// <summary>
        /// Gets the new completion status of the task.
        /// </summary>
        public bool Checked { get; }

        /// <summary>
        /// Initializes a new instance of the CheckTaskCommand class.
        /// </summary>
        /// <param name="taskId">The unique identifier of the task to update.</param>
        /// <param name="checked">The new completion status of the task.</param>
        public CheckTaskCommand(long taskId, bool @checked) : base("check_task")
        {
            TaskId = taskId;
            Checked = @checked;
        }
    }
}