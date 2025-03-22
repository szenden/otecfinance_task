using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using TaskList.Core.Models;
using TaskList.Core.Models.Commands;

namespace TaskList.Core.Services
{
    /// <summary>
    /// Defines the contract for task management operations in the TaskList application.
    /// Provides methods for managing projects and tasks, including creation, modification, and retrieval.
    /// </summary>
    public interface ITaskService
    {
        /// <summary>
        /// Creates a new project with the specified name.
        /// </summary>
        /// <param name="projectName">The name of the project to create.</param>
        /// <returns>A task that represents the asynchronous operation. The task result contains the newly created project.</returns>
        Task<Project> AddProjectAsync(string projectName);

        /// <summary>
        /// Adds a new task to the specified project.
        /// </summary>
        /// <param name="projectName">The name of the project to add the task to.</param>
        /// <param name="description">The description of the task.</param>
        /// <returns>A task that represents the asynchronous operation. The task result contains the newly created task.</returns>
        Task<ProjectTask> AddTaskAsync(string projectName, string description);

        /// <summary>
        /// Updates the completion status of a task.
        /// </summary>
        /// <param name="taskId">The unique identifier of the task.</param>
        /// <param name="checked">The new completion status of the task.</param>
        /// <returns>A task that represents the asynchronous operation. The task result indicates whether the update was successful.</returns>
        Task<bool> CheckTaskAsync(long taskId, bool @checked);

        /// <summary>
        /// Sets a deadline for a specific task.
        /// </summary>
        /// <param name="taskId">The unique identifier of the task.</param>
        /// <param name="deadline">The deadline to set for the task.</param>
        /// <returns>A task that represents the asynchronous operation. The task result indicates whether the deadline was set successfully.</returns>
        Task<bool> SetDeadlineAsync(long taskId, DateTime deadline);

        /// <summary>
        /// Retrieves all projects in the system.
        /// </summary>
        /// <returns>A task that represents the asynchronous operation. The task result contains a collection of all projects.</returns>
        Task<IEnumerable<Project>> GetAllProjectsAsync();

        /// <summary>
        /// Retrieves tasks that have a specific deadline.
        /// </summary>
        /// <param name="deadline">The deadline to filter tasks by. If null, returns all tasks.</param>
        /// <returns>A task that represents the asynchronous operation. The task result contains a collection of tasks matching the deadline criteria.</returns>
        Task<IEnumerable<ProjectTask>> GetTasksByDeadlineAsync(DateTime? deadline);

        /// <summary>
        /// Retrieves tasks that are due today.
        /// </summary>
        /// <returns>A task that represents the asynchronous operation. The task result contains a collection of tasks due today.</returns>
        Task<IEnumerable<ProjectTask>> GetTasksForTodayAsync();

        /// <summary>
        /// Executes a command on the task service.
        /// </summary>
        /// <param name="command">The command to execute.</param>
        /// <returns>A task that represents the asynchronous operation. The task result contains the result of the command execution.</returns>
        Task<CommandResult> ExecuteCommandAsync(BaseCommand command);
    }
}