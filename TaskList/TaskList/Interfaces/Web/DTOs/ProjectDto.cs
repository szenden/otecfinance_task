using System.Collections.Generic;
using TaskList.Interfaces.Web.DTOs;

namespace TaskList.Interfaces.Web.DTOs
{
    /// <summary>
    /// Data transfer object for project information.
    /// Used to send project data to API clients.
    /// </summary>
    public class ProjectDto
    {
        /// <summary>
        /// Gets or sets the unique identifier of the project.
        /// </summary>
        public long Id { get; set; }

        /// <summary>
        /// Gets or sets the name of the project.
        /// </summary>
        public string Name { get; set; }

        /// <summary>
        /// Gets or sets the list of tasks in the project.
        /// </summary>
        public List<TaskDto> Tasks { get; set; }
    }
}