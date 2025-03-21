using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using TaskList.Core.Models;
using TaskList.Core.Models.Commands;

namespace TaskList.Core.Services
{
    public class TaskService : ITaskService
    {
        private readonly IDictionary<string, Project> _projects = new Dictionary<string, Project>();
        private long _lastId = 0;

        public async Task<Project> AddProjectAsync(string projectName)
        {
            if (_projects.ContainsKey(projectName))
            {
                throw new InvalidOperationException($"Project '{projectName}' already exists.");
            }

            var project = new Project(projectName);
            _projects[projectName] = project;
            return await Task.FromResult(project);
        }

        public async Task<ProjectTask> AddTaskAsync(string projectName, string description)
        {
            if (!_projects.TryGetValue(projectName, out var project))
            {
                throw new InvalidOperationException($"Project '{projectName}' not found.");
            }

            var task = new ProjectTask(NextId(), description, projectName);
            project.AddTask(task);
            return await Task.FromResult(task);
        }

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

        public async Task<IEnumerable<Project>> GetAllProjectsAsync()
        {
            return await Task.FromResult(_projects.Values);
        }

        public async Task<IEnumerable<ProjectTask>> GetTasksByDeadlineAsync(DateTime? deadline)
        {
            var tasks = _projects.Values
                .SelectMany(p => p.Tasks)
                .Where(t => t.Deadline == deadline);
            return await Task.FromResult(tasks);
        }

        public async Task<IEnumerable<ProjectTask>> GetTasksForTodayAsync()
        {
            var today = DateTime.Today;
            var tasks = _projects.Values
                .SelectMany(p => p.Tasks)
                .Where(t => t.Deadline?.Date == today);
            return await Task.FromResult(tasks);
        }

        public async Task<CommandResult> ExecuteCommandAsync(BaseCommand command)
        {
            try
            {
                switch (command)
                {
                    case AddProjectCommand addProject:
                        await AddProjectAsync(addProject.ProjectName);
                        return new CommandResult { Success = true };

                    case AddTaskCommand addTask:
                        await AddTaskAsync(addTask.ProjectName, addTask.Description);
                        return new CommandResult { Success = true };

                    case CheckTaskCommand checkTask:
                        var checkedResult = await CheckTaskAsync(checkTask.TaskId, checkTask.Checked);
                        return new CommandResult { Success = checkedResult };

                    case SetDeadlineCommand setDeadline:
                        var deadlineResult = await SetDeadlineAsync(setDeadline.TaskId, setDeadline.Deadline);
                        return new CommandResult { Success = deadlineResult };

                    default:
                        return new CommandResult { Success = false, Error = "Unknown command type" };
                }
            }
            catch (Exception ex)
            {
                return new CommandResult { Success = false, Error = ex.Message };
            }
        }

        private ProjectTask FindTaskById(long taskId)
        {
            return _projects.Values
                .SelectMany(p => p.Tasks)
                .FirstOrDefault(t => t.Id == taskId);
        }

        private long NextId()
        {
            return ++_lastId;
        }
    }
}