using System;

namespace TaskList.Core.Models
{
    public class ProjectTask
    {
        public long Id { get; set; }
        public string Description { get; set; }
        public bool Done { get; set; }
        public DateTime? Deadline { get; set; }
        public string ProjectName { get; set; }

        public ProjectTask(long id, string description, string projectName)
        {
            Id = id;
            Description = description;
            ProjectName = projectName;
            Done = false;
        }
    }
}