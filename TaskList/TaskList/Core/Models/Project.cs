using System;
using System.Collections.Generic;
using System.Linq;

namespace TaskList.Core.Models
{
    /// <summary>
    /// Represents a project in the TaskList application.
    /// Contains a collection of tasks and provides methods for task management.
    /// </summary>
    public class Project
    {
        /// <summary>
        /// Gets or sets the unique identifier of the project.
        /// </summary>
        public long Id { get; set; }

        /// <summary>
        /// Gets the name of the project.
        /// </summary>
        public string Name { get; }

        /// <summary>
        /// Gets the collection of tasks associated with the project.
        /// </summary>
        public IList<ProjectTask> Tasks { get; }

        /// <summary>
        /// Initializes a new instance of the Project class.
        /// </summary>
        /// <param name="name">The name of the project.</param>
        public Project(string name)
        {
            Name = name;
            Tasks = new List<ProjectTask>();
        }

        /// <summary>
        /// Adds a task to the project.
        /// </summary>
        /// <param name="task">The task to add to the project.</param>
        public void AddTask(ProjectTask task)
        {
            Tasks.Add(task);
        }

        /// <summary>
        /// Retrieves a task by its unique identifier.
        /// </summary>
        /// <param name="id">The unique identifier of the task to find.</param>
        /// <returns>The task with the specified ID, or null if not found.</returns>
        public ProjectTask GetTaskById(long id)
        {
            return Tasks.FirstOrDefault(t => t.Id == id);
        }

        /// <summary>
        /// Retrieves all tasks that have a specific deadline.
        /// </summary>
        /// <param name="deadline">The deadline to filter tasks by. If null, returns tasks without deadlines.</param>
        /// <returns>A collection of tasks matching the deadline criteria.</returns>
        public IEnumerable<ProjectTask> GetTasksByDeadline(DateTime? deadline)
        {
            return Tasks.Where(t => t.Deadline == deadline);
        }
    }
}