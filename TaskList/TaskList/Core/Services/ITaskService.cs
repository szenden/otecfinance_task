using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using TaskList.Core.Models;
using TaskList.Core.Models.Commands;

namespace TaskList.Core.Services
{
    public interface ITaskService
    {
        Task<Project> AddProjectAsync(string projectName);
        Task<ProjectTask> AddTaskAsync(string projectName, string description);
        Task<bool> CheckTaskAsync(long taskId, bool @checked);
        Task<bool> SetDeadlineAsync(long taskId, DateTime deadline);
        Task<IEnumerable<Project>> GetAllProjectsAsync();
        Task<IEnumerable<ProjectTask>> GetTasksByDeadlineAsync(DateTime? deadline);
        Task<IEnumerable<ProjectTask>> GetTasksForTodayAsync();
        Task<CommandResult> ExecuteCommandAsync(BaseCommand command);
    }
}