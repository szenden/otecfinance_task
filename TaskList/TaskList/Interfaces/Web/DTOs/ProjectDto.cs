using System.Collections.Generic;
using TaskList.Interfaces.Web.DTOs;

namespace TaskList.Interfaces.Web.DTOs
{
    public class ProjectDto
    {
        public string Name { get; set; }
        public IList<TaskDto> Tasks { get; set; }
    }
}