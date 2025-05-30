using System;
using System.Threading.Tasks;
using TaskList.Core.Models;
using TaskList.Core.Processing;
using TaskList.Core.Services;

namespace TaskList.Interfaces.Console
{
    /// <summary>
    /// Provides a console-based user interface for the TaskList application.
    /// Handles user input, command processing, and formatted output of task and project information.
    /// </summary>
    public class ConsoleTaskList
    {
        private const string QUIT = "quit";
        /// <summary>
        /// The welcome message displayed when the application starts.
        /// </summary>
        public static readonly string StartupText = "Welcome to TaskList! Type 'help' for available commands.";

        private readonly IConsole _console;
        private readonly ICommandProcessor _commandProcessor;
        private readonly ITaskService _taskService;

        /// <summary>
        /// Initializes a new instance of the ConsoleTaskList class.
        /// </summary>
        /// <param name="console">The console interface for input and output operations.</param>
        /// <param name="commandProcessor">The processor for handling user commands.</param>
        /// <param name="taskService">The service for managing tasks and projects.</param>
        public ConsoleTaskList(IConsole console, ICommandProcessor commandProcessor, ITaskService taskService)
        {
            _console = console;
            _commandProcessor = commandProcessor;
            _taskService = taskService;
        }

        /// <summary>
        /// Runs the console-based TaskList application.
        /// Processes user commands until the quit command is entered.
        /// </summary>
        /// <returns>A task that represents the asynchronous operation.</returns>
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

        /// <summary>
        /// Displays the result of a processed command in a formatted manner.
        /// </summary>
        /// <param name="command">The command that was processed.</param>
        /// <param name="result">The result of the command processing.</param>
        /// <returns>A task that represents the asynchronous operation.</returns>
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
                    ShowTasksByDeadlineAsync(result.Data as IEnumerable<dynamic>);
                    break;
                case "help":
                    _console.WriteLine(result.Data as string);
                    break;
            }
        }

        /// <summary>
        /// Formats a task for display with its completion status, ID, description, and deadline.
        /// </summary>
        /// <param name="task">The task to format.</param>
        /// <returns>A formatted string representation of the task.</returns>
        private string FormatTaskForDisplay(ProjectTask task)
        {
            return string.Format("    [{0}] {1}: {2}{3}",
                (task.Done ? 'x' : ' '),
                task.Id,
                task.Description,
                task.Deadline.HasValue ? $", {task.Deadline.Value:dd-MM-yyyy}" : ""
            );
        }

        /// <summary>
        /// Displays all projects and their associated tasks in a formatted manner.
        /// </summary>
        /// <returns>A task that represents the asynchronous operation.</returns>
        private async Task ShowAllProjectsAsync()
        {
            var projects = await _taskService.GetAllProjectsAsync();
            foreach (var project in projects)
            {
                _console.WriteLine(project.Name);
                foreach (var task in project.Tasks)
                {
                    _console.WriteLine(FormatTaskForDisplay(task));
                }
                _console.WriteLine();
            }
        }

        /// <summary>
        /// Displays all tasks that are due today in a formatted manner.
        /// </summary>
        /// <returns>A task that represents the asynchronous operation.</returns>
        private async Task ShowTodayTasksAsync()
        {
            var tasks = await _taskService.GetTasksForTodayAsync();
            var hasTasks = false;

            foreach (var task in tasks)
            {
                hasTasks = true;
                _console.WriteLine(FormatTaskForDisplay(task));
            }

            if (!hasTasks)
            {
                _console.WriteLine("No tasks due today.");
            }
        }

        /// <summary>
        /// Displays all tasks grouped by their deadlines in a formatted manner.
        /// </summary>
        /// <param name="tasksByDeadline">The tasks grouped by deadline.</param>
        private void ShowTasksByDeadlineAsync(IEnumerable<dynamic> tasksByDeadline)
        {
            if (tasksByDeadline != null)
            {
                foreach (var deadlineGroup in tasksByDeadline)
                {
                    _console.WriteLine($"\nDeadline: {deadlineGroup.Deadline?.ToString("dd-MM-yyyy") ?? "No deadline"}");
                    foreach (var projectGroup in deadlineGroup.ProjectGroups)
                    {
                        _console.WriteLine($"  {projectGroup.ProjectName}:");
                        foreach (var task in projectGroup.Tasks)
                        {
                            _console.WriteLine(FormatTaskForDisplay((ProjectTask)task.task));
                        }
                    }
                }
            }
        }
    }
}