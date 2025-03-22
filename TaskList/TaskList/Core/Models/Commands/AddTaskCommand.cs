namespace TaskList.Core.Models.Commands
{
    /// <summary>
    /// Command to add a new task to an existing project in the TaskList application.
    /// </summary>
    public class AddTaskCommand : BaseCommand
    {
        /// <summary>
        /// Gets the ID of the project to add the task to.
        /// </summary>
        public long ProjectId { get; }

        /// <summary>
        /// Gets the description of the task to be created.
        /// </summary>
        public string Description { get; }

        /// <summary>
        /// Initializes a new instance of the AddTaskCommand class.
        /// </summary>
        /// <param name="projectId">The ID of the project to add the task to.</param>
        /// <param name="description">The description of the task to create.</param>
        public AddTaskCommand(long projectId, string description) : base("add_task")
        {
            ProjectId = projectId;
            Description = description;
        }
    }
}