using TaskList;

namespace Tasks
{
	[TestFixture]
	public sealed class ApplicationTest
	{
		public const string PROMPT = "> ";

		private FakeConsole console;
		private System.Threading.Thread applicationThread;

		[SetUp]
		public void StartTheApplication()
		{
			this.console = new FakeConsole();
			var taskList = new TaskList.TaskList(console);
			this.applicationThread = new System.Threading.Thread(() => taskList.Run());
			applicationThread.Start();
			ReadLines(TaskList.TaskList.startupText);
		}

		[TearDown]
		public void KillTheApplication()
		{
			if (applicationThread == null || !applicationThread.IsAlive)
			{
				return;
			}

			applicationThread.Abort();
			throw new Exception("The application is still running.");
		}

		[Test, Timeout(1000)]
		public void ItWorks()
		{
			Execute("show");

			Execute("add project secrets");
			Execute("add task secrets Eat more donuts.");
			Execute("add task secrets Destroy all humans.");

			Execute("show");
			ReadLines(
				"secrets",
				"    [ ] 1: Eat more donuts.",
				"    [ ] 2: Destroy all humans.",
				""
			);

			Execute("add project training");
			Execute("add task training Four Elements of Simple Design");
			Execute("add task training SOLID");
			Execute("add task training Coupling and Cohesion");
			Execute("add task training Primitive Obsession");
			Execute("add task training Outside-In TDD");
			Execute("add task training Interaction-Driven Design");

			Execute("check 1");
			Execute("check 3");
			Execute("check 5");
			Execute("check 6");

			Execute("show");
			ReadLines(
				"secrets",
				"    [x] 1: Eat more donuts.",
				"    [ ] 2: Destroy all humans.",
				"",
				"training",
				"    [x] 3: Four Elements of Simple Design",
				"    [ ] 4: SOLID",
				"    [x] 5: Coupling and Cohesion",
				"    [x] 6: Primitive Obsession",
				"    [ ] 7: Outside-In TDD",
				"    [ ] 8: Interaction-Driven Design",
				""
			);

			Execute("quit");
		}

		[Test, Timeout(1000)]
		public void DeadlineCommandsWork()
		{
			Execute("add project project1");
			Execute("add task project1 Task 1");
			Execute("add task project1 Task 2");
			Execute("add project project2");
			Execute("add task project2 Task 3");

			// Set deadline for Task 1 to today
			Execute("deadline 1 " + DateTime.Today.ToString("dd-MM-yyyy"));
			ReadLines("Deadline set for task 1.");

			// Set deadline for Task 2 to tomorrow
			Execute("deadline 2 " + DateTime.Today.AddDays(1).ToString("dd-MM-yyyy"));
			ReadLines("Deadline set for task 2.");

			// Set deadline for Task 3 to today
			Execute("deadline 3 " + DateTime.Today.ToString("dd-MM-yyyy"));
			ReadLines("Deadline set for task 3.");


			// Test invalid deadline command
			Execute("deadline invalid");
			ReadLines("Invalid deadline command. Use: deadline <ID> <date>");

			// Test invalid date format
			Execute("deadline 1 2024-11-25");
			ReadLines("Invalid date format. Use DD-MM-YYYY");

			// Test non-existent task
			Execute("deadline 999 25-11-2024");
			ReadLines("Could not find a task with an ID of 999.");

			Execute("quit");
		}

		[Test, Timeout(1000)]
		public void TodayCommandsWork()
		{
			Execute("add project project1");
			Execute("add task project1 Task 1");
			Execute("add task project1 Task 2");
			Execute("add project project2");
			Execute("add task project2 Task 3");

			// Set deadline for Task 1 to today
			Execute("deadline 1 " + DateTime.Today.ToString("dd-MM-yyyy"));
			ReadLines("Deadline set for task 1.");

			// Set deadline for Task 2 to tomorrow
			Execute("deadline 2 " + DateTime.Today.AddDays(1).ToString("dd-MM-yyyy"));
			ReadLines("Deadline set for task 2.");

			// Set deadline for Task 3 to today
			Execute("deadline 3 " + DateTime.Today.ToString("dd-MM-yyyy"));
			ReadLines("Deadline set for task 3.");

			// Check today's tasks
			Execute("today");
			ReadLines(
				"project1",
				"    [ ] 1: Task 1, ",
				"",
				"project2", 
				"    [ ] 3: Task 3, ",
				""
			);

			Execute("quit");
		}

		private void Execute(string command)
		{
			Read(PROMPT);
			Write(command);
		}

		private void Read(string expectedOutput)
		{
			var length = expectedOutput.Length;
			var actualOutput = console.RetrieveOutput(expectedOutput.Length);
			Assert.AreEqual(expectedOutput, actualOutput);
		}

		private void ReadLines(params string[] expectedOutput)
		{
			foreach (var line in expectedOutput)
			{
				Read(line + Environment.NewLine);
			}
		}

		private void Write(string input)
		{
			console.SendInput(input + Environment.NewLine);
		}
	}
}
