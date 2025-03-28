using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using TaskList.Core.Models;
using TaskList.Core.Models.Exceptions;

namespace TaskList.Core.Repositories
{
    /// <summary>
    /// Repository implementation for managing projects and tasks in the TaskList application
    /// </summary>
    public class TaskListRepository : ITaskListRepository
    {
        // In-memory storage for projects and tasks
        private readonly Dictionary<long, Project> _projects = new();
        private readonly Dictionary<long, ProjectTask> _tasks = new();

        /// <summary>
        /// Retrieves a project by its ID
        /// </summary>
        /// <param name="projectId">The ID of the project to retrieve</param>
        /// <returns>The requested project</returns>
        /// <exception cref="ProjectNotFoundException">Thrown when project is not found</exception>
        public async Task<Project> GetProjectAsync(long projectId)
        {
            if (!_projects.TryGetValue(projectId, out var project))
            {
                throw new ProjectNotFoundException(projectId);
            }
            return await Task.FromResult(project);
        }

        /// <summary>
        /// Retrieves all projects
        /// </summary>
        /// <returns>Collection of all projects</returns>
        public async Task<IEnumerable<Project>> GetAllProjectsAsync()
        {
            return await Task.FromResult(_projects.Values);
        }

        /// <summary>
        /// Adds a new project
        /// </summary>
        /// <param name="project">The project to add</param>
        /// <returns>The added project</returns>
        /// <exception cref="ProjectAlreadyExistsException">Thrown when project name already exists</exception>
        public async Task<Project> AddProjectAsync(Project project)
        {
            if (_projects.Values.Any(p => p.Name == project.Name))
            {
                throw new ProjectAlreadyExistsException(project.Name);
            }
            _projects[project.Id] = project;
            return await Task.FromResult(project);
        }

        /// <summary>
        /// Updates an existing project
        /// </summary>
        /// <param name="project">The project with updated information</param>
        /// <returns>The updated project</returns>
        /// <exception cref="ProjectNotFoundException">Thrown when project is not found</exception>
        public async Task<Project> UpdateProjectAsync(Project project)
        {
            if (!_projects.ContainsKey(project.Id))
            {
                throw new ProjectNotFoundException(project.Id);
            }
            _projects[project.Id] = project;
            return await Task.FromResult(project);
        }

        /// <summary>
        /// Deletes a project by its ID
        /// </summary>
        /// <param name="projectId">The ID of the project to delete</param>
        /// <exception cref="ProjectNotFoundException">Thrown when project is not found</exception>
        public async Task DeleteProjectAsync(long projectId)
        {
            if (!_projects.ContainsKey(projectId))
            {
                throw new ProjectNotFoundException(projectId);
            }
            _projects.Remove(projectId);
            await Task.CompletedTask;
        }

        /// <summary>
        /// Retrieves a task by its ID
        /// </summary>
        /// <param name="taskId">The ID of the task to retrieve</param>
        /// <returns>The requested task</returns>
        /// <exception cref="TaskNotFoundException">Thrown when task is not found</exception>
        public async Task<ProjectTask> GetTaskAsync(long taskId)
        {
            if (!_tasks.TryGetValue(taskId, out var task))
            {
                throw new TaskNotFoundException(taskId);
            }
            return await Task.FromResult(task);
        }

        /// <summary>
        /// Retrieves all tasks for a specific project
        /// </summary>
        /// <param name="projectId">The ID of the project</param>
        /// <returns>Collection of tasks belonging to the project</returns>
        public async Task<IEnumerable<ProjectTask>> GetTasksByProjectAsync(long projectId)
        {
            return await Task.FromResult(_tasks.Values.Where(t => t.ProjectId == projectId));
        }

        /// <summary>
        /// Adds a new task
        /// </summary>
        /// <param name="task">The task to add</param>
        /// <returns>The added task</returns>
        public async Task<ProjectTask> AddTaskAsync(ProjectTask task)
        {
            _tasks[task.Id] = task;
            return await Task.FromResult(task);
        }

        /// <summary>
        /// Updates an existing task
        /// </summary>
        /// <param name="task">The task with updated information</param>
        /// <returns>The updated task</returns>
        /// <exception cref="TaskNotFoundException">Thrown when task is not found</exception>
        public async Task<ProjectTask> UpdateTaskAsync(ProjectTask task)
        {
            if (!_tasks.ContainsKey(task.Id))
            {
                throw new TaskNotFoundException(task.Id);
            }
            _tasks[task.Id] = task;
            return await Task.FromResult(task);
        }

        /// <summary>
        /// Deletes a task by its ID
        /// </summary>
        /// <param name="taskId">The ID of the task to delete</param>
        /// <exception cref="TaskNotFoundException">Thrown when task is not found</exception>
        public async Task DeleteTaskAsync(long taskId)
        {
            if (!_tasks.ContainsKey(taskId))
            {
                throw new TaskNotFoundException(taskId);
            }
            _tasks.Remove(taskId);
            await Task.CompletedTask;
        }

        /// <summary>
        /// Retrieves all tasks scheduled for today
        /// </summary>
        /// <returns>Collection of tasks for today</returns>
        public async Task<IEnumerable<ProjectTask>> GetTasksForTodayAsync()
        {
            var today = DateTime.Today;
            return await Task.FromResult(_tasks.Values.Where(t => t.Deadline?.Date == today));
        }

        /// <summary>
        /// Retrieves all tasks due by a specific deadline
        /// </summary>
        /// <param name="deadline">The deadline date to filter tasks by</param>
        /// <returns>Collection of tasks due by the specified deadline</returns>
        public async Task<IEnumerable<ProjectTask>> GetTasksByDeadlineAsync(DateTime deadline)
        {
            return await Task.FromResult(_tasks.Values.Where(t => t.Deadline?.Date == deadline.Date));
        }
    }
}