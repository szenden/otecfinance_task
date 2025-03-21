using System;
using System.Threading.Tasks;
using TaskList.Core.Models;
using TaskList.Core.Processing;
using TaskList.Core.Services;

namespace TaskList.Interfaces.Console
{
    public class ConsoleTaskList
    {
        private const string QUIT = "quit";
        public static readonly string StartupText = "Welcome to TaskList! Type 'help' for available commands.";

        private readonly IConsole _console;
        private readonly ICommandProcessor _commandProcessor;
        private readonly ITaskService _taskService;

        public ConsoleTaskList(IConsole console, ICommandProcessor commandProcessor, ITaskService taskService)
        {
            _console = console;
            _commandProcessor = commandProcessor;
            _taskService = taskService;
        }

        public async Task RunAsync()
        {
            _console.WriteLine(StartupText);
            while (true)
            {
                _console.Write("> ");
                var command = _console.ReadLine();
                if (command == QUIT)
                {
                    break;
                }

                var result = await _commandProcessor.ProcessCommandAsync(command);
                if (!result.Success)
                {
                    _console.WriteLine(result.Error);
                    continue;
                }

                await DisplayCommandResultAsync(command, result);
            }
        }

        private async Task DisplayCommandResultAsync(string command, CommandResult result)
        {
            var commandType = command.Split(" ".ToCharArray(), 2)[0].ToLower();
            switch (commandType)
            {
                case "show":
                    await ShowAllProjectsAsync();
                    break;
                case "today":
                    await ShowTodayTasksAsync();
                    break;
                case "view-by-deadline":
                    await ShowTasksByDeadlineAsync();
                    break;
                case "help":
                    ShowHelp();
                    break;
            }
        }

        private async Task ShowAllProjectsAsync()
        {
            var projects = await _taskService.GetAllProjectsAsync();
            foreach (var project in projects)
            {
                _console.WriteLine(project.Name);
                foreach (var task in project.Tasks)
                {
                    _console.WriteLine("    [{0}] {1}: {2}", (task.Done ? 'x' : ' '), task.Id, task.Description);
                }
                _console.WriteLine();
            }
        }

        private async Task ShowTodayTasksAsync()
        {
            var tasks = await _taskService.GetTasksForTodayAsync();
            var hasTasks = false;

            foreach (var task in tasks)
            {
                hasTasks = true;
                _console.WriteLine("    [{0}] {1}: {2}", (task.Done ? 'x' : ' '), task.Id, task.Description);
            }

            if (!hasTasks)
            {
                _console.WriteLine("No tasks due today.");
            }
        }

        private async Task ShowTasksByDeadlineAsync()
        {
            var result = await _commandProcessor.ProcessCommandAsync("view-by-deadline");
            if (!result.Success)
            {
                _console.WriteLine(result.Error);
                return;
            }

            var tasksByDeadline = result.Data as IEnumerable<dynamic>;
            foreach (var deadlineGroup in tasksByDeadline)
            {
                if (deadlineGroup.Deadline == null)
                {
                    _console.WriteLine("No deadline:");
                }
                else
                {
                    _console.WriteLine($"{deadlineGroup.Deadline:dd-MM-yyyy}:");
                }

                foreach (var projectGroup in deadlineGroup.ProjectGroups)
                {
                    _console.WriteLine($"    {projectGroup.ProjectName}:");
                    foreach (var task in projectGroup.Tasks)
                    {
                        _console.WriteLine($"        {task.task.Id}: {task.task.Description}");
                    }
                }
                _console.WriteLine();
            }
        }

        private void ShowHelp()
        {
            _console.WriteLine("Commands:");
            _console.WriteLine("  show");
            _console.WriteLine("  add project <project name>");
            _console.WriteLine("  add task <project name> <task description>");
            _console.WriteLine("  check <task ID>");
            _console.WriteLine("  uncheck <task ID>");
            _console.WriteLine("  deadline <task ID> <date>");
            _console.WriteLine("  today");
            _console.WriteLine("  view-by-deadline");
            _console.WriteLine();
        }
    }
}