namespace TaskList.Core.Models.Commands
{
    /// <summary>
    /// Command to add a new project to the TaskList application.
    /// </summary>
    public class AddProjectCommand : BaseCommand
    {
        /// <summary>
        /// Gets the name of the project to be created.
        /// </summary>
        public string ProjectName { get; }

        /// <summary>
        /// Initializes a new instance of the AddProjectCommand class.
        /// </summary>
        /// <param name="projectName">The name of the project to create.</param>
        public AddProjectCommand(string projectName) : base("add_project")
        {
            ProjectName = projectName;
        }
    }
}