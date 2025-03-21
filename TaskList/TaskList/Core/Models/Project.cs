using System.Collections.Generic;
using System.Linq;

namespace TaskList.Core.Models
{
    public class Project
    {
        public string Name { get; }
        public IList<ProjectTask> Tasks { get; }

        public Project(string name)
        {
            Name = name;
            Tasks = new List<ProjectTask>();
        }

        public void AddTask(ProjectTask task)
        {
            Tasks.Add(task);
        }

        public ProjectTask GetTaskById(long id)
        {
            return Tasks.FirstOrDefault(t => t.Id == id);
        }

        public IEnumerable<ProjectTask> GetTasksByDeadline(DateTime? deadline)
        {
            return Tasks.Where(t => t.Deadline == deadline);
        }
    }
}