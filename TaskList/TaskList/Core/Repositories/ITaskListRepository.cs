using System.Collections.Generic;
using System.Threading.Tasks;
using TaskList.Core.Models;

namespace TaskList.Core.Repositories
{
    /// <summary>
    /// Repository interface for managing projects and tasks in the TaskList application
    /// </summary>
    public interface ITaskListRepository
    {
        /// <summary>
        /// Retrieves a project by its ID
        /// </summary>
        /// <param name="projectId">The ID of the project to retrieve</param>
        /// <returns>The requested project</returns>
        Task<Project> GetProjectAsync(long projectId);

        /// <summary>
        /// Retrieves all projects
        /// </summary>
        /// <returns>Collection of all projects</returns>
        Task<IEnumerable<Project>> GetAllProjectsAsync();

        /// <summary>
        /// Adds a new project
        /// </summary>
        /// <param name="project">The project to add</param>
        /// <returns>The added project</returns>
        Task<Project> AddProjectAsync(Project project);

        /// <summary>
        /// Updates an existing project
        /// </summary>
        /// <param name="project">The project with updated information</param>
        /// <returns>The updated project</returns>
        Task<Project> UpdateProjectAsync(Project project);

        /// <summary>
        /// Deletes a project by its ID
        /// </summary>
        /// <param name="projectId">The ID of the project to delete</param>
        Task DeleteProjectAsync(long projectId);

        /// <summary>
        /// Retrieves a task by its ID
        /// </summary>
        /// <param name="taskId">The ID of the task to retrieve</param>
        /// <returns>The requested task</returns>
        Task<ProjectTask> GetTaskAsync(long taskId);

        /// <summary>
        /// Retrieves all tasks for a specific project
        /// </summary>
        /// <param name="projectId">The ID of the project</param>
        /// <returns>Collection of tasks belonging to the project</returns>
        Task<IEnumerable<ProjectTask>> GetTasksByProjectAsync(long projectId);

        /// <summary>
        /// Adds a new task
        /// </summary>
        /// <param name="task">The task to add</param>
        /// <returns>The added task</returns>
        Task<ProjectTask> AddTaskAsync(ProjectTask task);

        /// <summary>
        /// Updates an existing task
        /// </summary>
        /// <param name="task">The task with updated information</param>
        /// <returns>The updated task</returns>
        Task<ProjectTask> UpdateTaskAsync(ProjectTask task);

        /// <summary>
        /// Deletes a task by its ID
        /// </summary>
        /// <param name="taskId">The ID of the task to delete</param>
        Task DeleteTaskAsync(long taskId);

        /// <summary>
        /// Retrieves all tasks scheduled for today
        /// </summary>
        /// <returns>Collection of tasks for today</returns>
        Task<IEnumerable<ProjectTask>> GetTasksForTodayAsync();

        /// <summary>
        /// Retrieves all tasks due by a specific deadline
        /// </summary>
        /// <param name="deadline">The deadline date to filter tasks by</param>
        /// <returns>Collection of tasks due by the specified deadline</returns>
        Task<IEnumerable<ProjectTask>> GetTasksByDeadlineAsync(DateTime deadline);
    }
}