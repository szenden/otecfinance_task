namespace TaskList.Interfaces.Web.DTOs
{
    /// <summary>
    /// Request model for creating a new task.
    /// Used to receive task creation requests from API clients.
    /// </summary>
    public class AddTaskRequest
    {
        /// <summary>
        /// Gets or sets the name of the project to add the task to.
        /// </summary>
        public string ProjectName { get; set; }

        /// <summary>
        /// Gets or sets the description of the task to create.
        /// </summary>
        public string Description { get; set; }
    }
}