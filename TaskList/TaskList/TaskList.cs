using System;
using System.Collections.Generic;
using System.Linq;

namespace TaskList
{
	public sealed class TaskList
	{
		private const string QUIT = "quit";
		public static readonly string startupText = "Welcome to TaskList! Type 'help' for available commands.";

		private readonly IDictionary<string, IList<Task>> tasks = new Dictionary<string, IList<Task>>();
		private readonly IConsole console;

		private long lastId = 0;

		public static void Main(string[] args)
		{
			new TaskList(new RealConsole()).Run();
		}

		public TaskList(IConsole console)
		{
			this.console = console;
		}

		public void Run()
		{
			console.WriteLine(startupText);
			while (true)
			{
				console.Write("> ");
				var command = console.ReadLine();
				if (command == QUIT)
				{
					break;
				}
				Execute(command);
			}
		}

		private void Execute(string commandLine)
		{
			var commandRest = commandLine.Split(" ".ToCharArray(), 2);
			var command = commandRest[0];
			switch (command)
			{
				case "show":
					Show();
					break;
				case "add":
					Add(commandRest[1]);
					break;
				case "check":
					Check(commandRest[1]);
					break;
				case "uncheck":
					Uncheck(commandRest[1]);
					break;
				case "help":
					Help();
					break;
				case "deadline":
					SetDeadline(commandRest[1]);
					break;
				case "today":
					ShowToday();
					break;
				case "view-by-deadline":
					ViewByDeadline();
					break;
				default:
					Error(command);
					break;
			}
		}

		private void Show()
		{
			foreach (var project in tasks)
			{
				console.WriteLine(project.Key);
				foreach (var task in project.Value)
				{
					console.WriteLine("    [{0}] {1}: {2}", (task.Done ? 'x' : ' '), task.Id, task.Description);
				}
				console.WriteLine();
			}
		}

		private void Add(string commandLine)
		{
			var subcommandRest = commandLine.Split(" ".ToCharArray(), 2);
			var subcommand = subcommandRest[0];
			if (subcommand == "project")
			{
				AddProject(subcommandRest[1]);
			}
			else if (subcommand == "task")
			{
				var projectTask = subcommandRest[1].Split(" ".ToCharArray(), 2);
				AddTask(projectTask[0], projectTask[1]);
			}
		}

		private void AddProject(string name)
		{
			tasks[name] = new List<Task>();
		}

		private void AddTask(string project, string description)
		{
			if (!tasks.TryGetValue(project, out IList<Task> projectTasks))
			{
				Console.WriteLine("Could not find a project with the name \"{0}\".", project);
				return;
			}
			projectTasks.Add(new Task { Id = NextId(), Description = description, Done = false });
		}

		private void Check(string idString)
		{
			SetDone(idString, true);
		}

		private void Uncheck(string idString)
		{
			SetDone(idString, false);
		}

		private void SetDone(string idString, bool done)
		{
			int id = int.Parse(idString);
			var identifiedTask = tasks
				.Select(project => project.Value.FirstOrDefault(task => task.Id == id))
				.Where(task => task != null)
				.FirstOrDefault();
			if (identifiedTask == null)
			{
				console.WriteLine("Could not find a task with an ID of {0}.", id);
				return;
			}

			identifiedTask.Done = done;
		}

		private void SetDeadline(string commandLine)
		{
			var parts = commandLine.Split(" ".ToCharArray(), 2);
			if (parts.Length != 2)
			{
				console.WriteLine("Invalid deadline command. Use: deadline <ID> <date>");
				return;
			}

			if (!long.TryParse(parts[0], out long id))
			{
				console.WriteLine("Invalid task ID.");
				return;
			}

			if (!DateTime.TryParseExact(parts[1], "dd-MM-yyyy", null, System.Globalization.DateTimeStyles.None, out DateTime deadline))
			{
				console.WriteLine("Invalid date format. Use DD-MM-YYYY");
				return;
			}

			var identifiedTask = tasks
				.Select(project => project.Value.FirstOrDefault(task => task.Id == id))
				.Where(task => task != null)
				.FirstOrDefault();

			if (identifiedTask == null)
			{
				console.WriteLine("Could not find a task with an ID of {0}.", id);
				return;
			}

			identifiedTask.Deadline = deadline;
			console.WriteLine("Deadline set for task {0}.", id);
		}

		private void ShowToday()
		{
			var today = DateTime.Today;
			var hasTasksForToday = false;

			foreach (var project in tasks)
			{
				var todayTasks = project.Value.Where(t => t.Deadline?.Date == today).ToList();
				if (todayTasks.Any())
				{
					hasTasksForToday = true;
					console.WriteLine(project.Key);
					foreach (var task in todayTasks)
					{
						console.WriteLine("    [{0}] {1}: {2}, ", (task.Done ? 'x' : ' '), task.Id, task.Description);
					}
					console.WriteLine();
				}
			}

			if (!hasTasksForToday)
			{
				console.WriteLine("No tasks due today.");
			}
		}

		private void Help()
		{
			console.WriteLine("Commands:");
			console.WriteLine("  show");
			console.WriteLine("  add project <project name>");
			console.WriteLine("  add task <project name> <task description>");
			console.WriteLine("  check <task ID>");
			console.WriteLine("  uncheck <task ID>");
			console.WriteLine("  deadline <task ID> <date>");
			console.WriteLine("  today");
			console.WriteLine("  view-by-deadline");
			console.WriteLine();
		}

		private void Error(string command)
		{
			console.WriteLine("I don't know what the command \"{0}\" is.", command);
		}

		private long NextId()
		{
			return ++lastId;
		}

		private void ViewByDeadline()
		{
			// Group tasks by deadline
			var tasksByDeadline = tasks
				.SelectMany(project => project.Value.Select(task => new { task, project.Key }))
				.GroupBy(x => x.task.Deadline)
				.OrderBy(g => g.Key == null) // Put null deadlines last
				.ThenBy(g => g.Key); // Order by date

			foreach (var deadlineGroup in tasksByDeadline)
			{
				if (deadlineGroup.Key == null)
				{
					console.WriteLine("No deadline:");
				}
				else
				{
					console.WriteLine($"{deadlineGroup.Key.Value:dd-MM-yyyy}:");
				}

				// Group tasks by project within each deadline
				var tasksByProject = deadlineGroup
					.GroupBy(x => x.Key)
					.OrderBy(g => g.Key);

				foreach (var projectGroup in tasksByProject)
				{
					console.WriteLine($"    {projectGroup.Key}:");
					foreach (var task in projectGroup.OrderBy(x => x.task.Id))
					{
						console.WriteLine($"        {task.task.Id}: {task.task.Description}");
					}
				}
				console.WriteLine();
			}
		}
	}
}
