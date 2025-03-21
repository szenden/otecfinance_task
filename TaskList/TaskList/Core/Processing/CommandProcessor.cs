using System;
using System.Threading.Tasks;
using TaskList.Core.Models;
using TaskList.Core.Models.Commands;
using TaskList.Core.Services;
using System.Linq;

namespace TaskList.Core.Processing
{
    public class CommandProcessor : ICommandProcessor
    {
        private readonly ITaskService _taskService;

        public CommandProcessor(ITaskService taskService)
        {
            _taskService = taskService;
        }

        public async Task<CommandResult> ProcessCommandAsync(string commandLine)
        {
            var parts = commandLine.Split(" ".ToCharArray(), 2);
            if (parts.Length == 0)
            {
                return new CommandResult { Success = false, Error = "Empty command" };
            }

            var command = parts[0].ToLower();
            var args = parts.Length > 1 ? parts[1] : string.Empty;

            try
            {
                switch (command)
                {
                    case "add":
                        return await ProcessAddCommandAsync(args);
                    case "check":
                        return await ProcessCheckCommandAsync(args, true);
                    case "uncheck":
                        return await ProcessCheckCommandAsync(args, false);
                    case "deadline":
                        return await ProcessDeadlineCommandAsync(args);
                    case "show":
                        return await ProcessShowCommandAsync();
                    case "help":
                        return ProcessHelpCommand();
                    case "today":
                        return await ProcessTodayCommandAsync();
                    case "view-by-deadline":
                        return await ProcessViewByDeadlineCommandAsync();
                    default:
                        return new CommandResult { Success = false, Error = $"Unknown command: {command}" };
                }
            }
            catch (Exception ex)
            {
                return new CommandResult { Success = false, Error = ex.Message };
            }
        }

        private async Task<CommandResult> ProcessAddCommandAsync(string args)
        {
            var parts = args.Split(" ".ToCharArray(), 2);
            if (parts.Length < 2)
            {
                return new CommandResult { Success = false, Error = "Invalid add command format" };
            }

            BaseCommand command = parts[0].ToLower() switch
            {
                "project" => new AddProjectCommand(parts[1]),
                "task" => ParseAddTaskCommand(parts[1]),
                _ => throw new ArgumentException($"Unknown add command type: {parts[0]}")
            };

            return await _taskService.ExecuteCommandAsync(command);
        }

        private async Task<CommandResult> ProcessCheckCommandAsync(string args, bool @checked)
        {
            if (!long.TryParse(args, out long taskId))
            {
                return new CommandResult { Success = false, Error = "Invalid task ID" };
            }

            var command = new CheckTaskCommand(taskId, @checked);
            return await _taskService.ExecuteCommandAsync(command);
        }

        private async Task<CommandResult> ProcessDeadlineCommandAsync(string args)
        {
            var parts = args.Split(" ".ToCharArray(), 2);
            if (parts.Length != 2)
            {
                return new CommandResult { Success = false, Error = "Invalid deadline command format" };
            }

            if (!long.TryParse(parts[0], out long taskId))
            {
                return new CommandResult { Success = false, Error = "Invalid task ID" };
            }

            if (!DateTime.TryParseExact(parts[1], "dd-MM-yyyy", null, System.Globalization.DateTimeStyles.None, out DateTime deadline))
            {
                return new CommandResult { Success = false, Error = "Invalid date format. Use DD-MM-YYYY" };
            }

            var command = new SetDeadlineCommand(taskId, deadline);
            return await _taskService.ExecuteCommandAsync(command);
        }

        private async Task<CommandResult> ProcessShowCommandAsync()
        {
            var projects = await _taskService.GetAllProjectsAsync();
            return new CommandResult { Success = true, Data = projects };
        }

        private CommandResult ProcessHelpCommand()
        {
            var helpText = @"Commands:
            show
            add project <project name>
            add task <project name> <task description>
            check <task ID>
            uncheck <task ID>
            deadline <task ID> <date>
            today
            view-by-deadline";
            return new CommandResult { Success = true, Data = helpText };
        }

        private async Task<CommandResult> ProcessTodayCommandAsync()
        {
            var tasks = await _taskService.GetTasksForTodayAsync();
            return new CommandResult { Success = true, Data = tasks };
        }

        private async Task<CommandResult> ProcessViewByDeadlineCommandAsync()
        {
            var projects = await _taskService.GetAllProjectsAsync();
            var tasksByDeadline = projects
                .SelectMany(project => project.Tasks.Select(task => new { task, project.Name }))
                .GroupBy(x => x.task.Deadline)
                .OrderBy(g => g.Key == null) // Put null deadlines last
                .ThenBy(g => g.Key) // Order by date
                .Select(g => new
                {
                    Deadline = g.Key,
                    ProjectGroups = g.GroupBy(x => x.Name)
                        .OrderBy(pg => pg.Key)
                        .Select(pg => new
                        {
                            ProjectName = pg.Key,
                            Tasks = pg.OrderBy(x => x.task.Id)
                        })
                });

            return new CommandResult { Success = true, Data = tasksByDeadline };
        }

        private BaseCommand ParseAddTaskCommand(string args)
        {
            var parts = args.Split(" ".ToCharArray(), 2);
            if (parts.Length < 2)
            {
                throw new ArgumentException("Invalid add task command format");
            }

            return new AddTaskCommand(parts[0], parts[1]);
        }
    }
}