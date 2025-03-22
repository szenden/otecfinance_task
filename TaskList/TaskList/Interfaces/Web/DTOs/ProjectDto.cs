using System.Collections.Generic;
using TaskList.Interfaces.Web.DTOs;

namespace TaskList.Interfaces.Web.DTOs
{
    /// <summary>
    /// Data transfer object for project information.
    /// Used to transfer project data between the API and clients.
    /// </summary>
    public class ProjectDto
    {
        /// <summary>
        /// Gets or sets the name of the project.
        /// </summary>
        public string Name { get; set; }

        /// <summary>
        /// Gets or sets the list of tasks associated with the project.
        /// </summary>
        public IList<TaskDto> Tasks { get; set; }
    }
}