using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using TaskList.Core.Models;
using TaskList.Core.Models.Commands;

namespace TaskList.Core.Services
{
    /// <summary>
    /// Implementation of the ITaskService interface that provides task management functionality.
    /// Manages projects and tasks in memory using a dictionary-based storage system.
    /// </summary>
    public class TaskService : ITaskService
    {
        private readonly IDictionary<long, Project> _projects = new Dictionary<long, Project>();
        private long _lastProjectId = 0;
        private long _lastTaskId = 0;

        /// <summary>
        /// Creates a new project with the specified name.
        /// </summary>
        /// <param name="projectName">The name of the project to create.</param>
        /// <returns>A task that represents the asynchronous operation. The task result contains the newly created project.</returns>
        /// <exception cref="InvalidOperationException">Thrown when a project with the same name already exists.</exception>
        public async Task<Project> AddProjectAsync(string projectName)
        {
            if (_projects.Values.Any(p => p.Name == projectName))
            {
                throw new InvalidOperationException($"Project '{projectName}' already exists.");
            }

            var project = new Project(projectName);
            project.Id = NextProjectId();
            _projects[project.Id] = project;
            return await Task.FromResult(project);
        }

        /// <summary>
        /// Adds a new project to the system.
        /// </summary>
        /// <param name="project">The project to add.</param>
        /// <returns>A task that represents the asynchronous operation.</returns>
        public async Task AddProject(Project project)
        {
            if (_projects.ContainsKey(project.Id))
            {
                throw new InvalidOperationException($"Project with ID {project.Id} already exists.");
            }

            _projects[project.Id] = project;
            await Task.CompletedTask;
        }

        /// <summary>
        /// Retrieves a project by its unique identifier.
        /// </summary>
        /// <param name="projectId">The unique identifier of the project.</param>
        /// <returns>The project with the specified ID, or null if not found.</returns>
        public Project GetProject(long projectId)
        {
            return _projects.TryGetValue(projectId, out var project) ? project : throw new InvalidOperationException($"Project with ID {projectId} not found.");
        }

        /// <summary>
        /// Adds a new task to the specified project.
        /// </summary>
        /// <param name="projectId">The ID of the project to add the task to.</param>
        /// <param name="description">The description of the task.</param>
        /// <returns>A task that represents the asynchronous operation. The task result contains the newly created task.</returns>
        /// <exception cref="InvalidOperationException">Thrown when the specified project does not exist.</exception>
        public async Task<ProjectTask> AddTaskAsync(long projectId, string description)
        {
            if (!_projects.TryGetValue(projectId, out var project))
            {
                throw new InvalidOperationException($"Project with ID {projectId} not found.");
            }

            var task = new ProjectTask(NextTaskId(), description, project.Name);
            project.AddTask(task);
            return await Task.FromResult(task);
        }

        /// <summary>
        /// Updates the completion status of a task.
        /// </summary>
        /// <param name="taskId">The unique identifier of the task.</param>
        /// <param name="checked">The new completion status of the task.</param>
        /// <returns>A task that represents the asynchronous operation. The task result indicates whether the update was successful.</returns>
        public async Task<bool> CheckTaskAsync(long taskId, bool @checked)
        {
            var task = FindTaskById(taskId);
            if (task == null)
            {
                return await Task.FromResult(false);
            }

            task.Done = @checked;
            return await Task.FromResult(true);
        }

        /// <summary>
        /// Sets a deadline for a specific task.
        /// </summary>
        /// <param name="taskId">The unique identifier of the task.</param>
        /// <param name="deadline">The deadline to set for the task.</param>
        /// <returns>A task that represents the asynchronous operation. The task result indicates whether the deadline was set successfully.</returns>
        public async Task<bool> SetDeadlineAsync(long taskId, DateTime deadline)
        {
            var task = FindTaskById(taskId);
            if (task == null)
            {
                return await Task.FromResult(false);
            }

            task.Deadline = deadline;
            return await Task.FromResult(true);
        }

        /// <summary>
        /// Retrieves all projects in the system.
        /// </summary>
        /// <returns>A task that represents the asynchronous operation. The task result contains a collection of all projects.</returns>
        public async Task<IEnumerable<Project>> GetAllProjectsAsync()
        {
            return await Task.FromResult(_projects.Values);
        }

        /// <summary>
        /// Retrieves tasks that have a specific deadline.
        /// </summary>
        /// <param name="deadline">The deadline to filter tasks by. If null, returns all tasks.</param>
        /// <returns>A task that represents the asynchronous operation. The task result contains a collection of tasks matching the deadline criteria.</returns>
        public async Task<IEnumerable<ProjectTask>> GetTasksByDeadlineAsync(DateTime? deadline)
        {
            var tasks = _projects.Values
                .SelectMany(p => p.Tasks)
                .Where(t => t.Deadline == deadline);
            return await Task.FromResult(tasks);
        }

        /// <summary>
        /// Retrieves tasks that are due today.
        /// </summary>
        /// <returns>A task that represents the asynchronous operation. The task result contains a collection of tasks due today.</returns>
        public async Task<IEnumerable<ProjectTask>> GetTasksForTodayAsync()
        {
            var today = DateTime.Today;
            var tasks = _projects.Values
                .SelectMany(p => p.Tasks)
                .Where(t => t.Deadline?.Date == today);
            return await Task.FromResult(tasks);
        }

        /// <summary>
        /// Executes a command on the task service.
        /// </summary>
        /// <param name="command">The command to execute.</param>
        /// <returns>A task that represents the asynchronous operation. The task result contains the result of the command execution.</returns>
        /// <exception cref="Exception">Thrown when command execution fails.</exception>
        public async Task<CommandResult> ExecuteCommandAsync(BaseCommand command)
        {
            try
            {
                switch (command)
                {
                    case AddProjectCommand addProject:
                        var project = await AddProjectAsync(addProject.ProjectName);
                        return new CommandResult { Success = true, Data = project };

                    case AddTaskCommand addTask:
                        var task = await AddTaskAsync(addTask.ProjectId, addTask.Description);
                        return new CommandResult { Success = true, Data = task };

                    case CheckTaskCommand checkTask:
                        var checkedResult = await CheckTaskAsync(checkTask.TaskId, checkTask.Checked);
                        return new CommandResult { Success = checkedResult };

                    case SetDeadlineCommand setDeadline:
                        var deadlineResult = await SetDeadlineAsync(setDeadline.TaskId, setDeadline.Deadline);
                        return new CommandResult { Success = deadlineResult };

                    default:
                        return new CommandResult { Success = false, Error = "Unknown command type: " + command.GetType().Name };
                }
            }
            catch (Exception ex)
            {
                return new CommandResult { Success = false, Error = ex.Message };
            }
        }

        /// <summary>
        /// Finds a task by its unique identifier across all projects.
        /// </summary>
        /// <param name="taskId">The unique identifier of the task to find.</param>
        /// <returns>The found task, or null if no task with the specified ID exists.</returns>
        private ProjectTask FindTaskById(long taskId)
        {
            return _projects.Values
                .SelectMany(p => p.Tasks)
                .FirstOrDefault(t => t.Id == taskId) ?? throw new InvalidOperationException($"Task with ID {taskId} not found.");
        }

        /// <summary>
        /// Generates the next available project identifier.
        /// </summary>
        /// <returns>The next available project identifier.</returns>
        private long NextProjectId()
        {
            return ++_lastProjectId;
        }

        /// <summary>
        /// Generates the next available task identifier.
        /// </summary>
        /// <returns>The next available task identifier.</returns>
        private long NextTaskId()
        {
            return ++_lastTaskId;
        }
    }
}