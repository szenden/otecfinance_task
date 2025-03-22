namespace TaskList.Interfaces.Web.DTOs
{
    /// <summary>
    /// Request model for creating a new project.
    /// Used to receive project creation requests from API clients.
    /// </summary>
    public class CreateProjectRequest
    {
        /// <summary>
        /// Gets or sets the name of the project to create.
        /// </summary>
        public string Name { get; set; }
    }
}