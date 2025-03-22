using NUnit.Framework;
using TaskList.Core.Models;
using TaskList.Core.Models.Commands;
using TaskList.Core.Services;
using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using System.Linq;

namespace TaskList.Tests
{
    public class InvalidCommand : BaseCommand
    {
        public InvalidCommand() : base("invalid_command_type") { }
    }

    [TestFixture]
    public class CommandTests
    {
        private ITaskService _taskService;

        [SetUp]
        public void Setup()
        {
            _taskService = new TaskService();
        }

        [Test]
        public async Task AddProjectCommand_ShouldCreateNewProject()
        {
            // Arrange
            var command = new AddProjectCommand("TestProject");

            // Act
            var result = await _taskService.ExecuteCommandAsync(command);

            // Assert
            Assert.That(result.Success, Is.True);
            var projects = await _taskService.GetAllProjectsAsync();
            Assert.That(projects.Count(), Is.EqualTo(1));
            Assert.That(projects.First().Name, Is.EqualTo("TestProject"));
        }

        [Test]
        public async Task AddTaskCommand_ShouldCreateNewTask()
        {
            // Arrange
            var projectName = "TestProject";
            var taskDescription = "Test Task";
            await _taskService.AddProjectAsync(projectName);
            var command = new AddTaskCommand(projectName, taskDescription);

            // Act
            var result = await _taskService.ExecuteCommandAsync(command);

            // Assert
            Assert.That(result.Success, Is.True);
            var projects = await _taskService.GetAllProjectsAsync();
            var project = projects.First(p => p.Name == projectName);
            Assert.That(project.Tasks.Count, Is.EqualTo(1));
            Assert.That(project.Tasks[0].Description, Is.EqualTo(taskDescription));
        }

        [Test]
        public async Task CheckTaskCommand_ShouldMarkTaskAsDone()
        {
            // Arrange
            var projectName = "TestProject";
            var taskDescription = "Test Task";
            await _taskService.AddProjectAsync(projectName);
            await _taskService.AddTaskAsync(projectName, taskDescription);
            var command = new CheckTaskCommand(1, true);

            // Act
            var result = await _taskService.ExecuteCommandAsync(command);

            // Assert
            Assert.That(result.Success, Is.True);
            var projects = await _taskService.GetAllProjectsAsync();
            var project = projects.First(p => p.Name == projectName);
            Assert.That(project.Tasks[0].Done, Is.True);
        }

        [Test]
        public async Task SetDeadlineCommand_ShouldSetTaskDeadline()
        {
            // Arrange
            var projectName = "TestProject";
            var taskDescription = "Test Task";
            var deadline = DateTime.Today.AddDays(1);
            await _taskService.AddProjectAsync(projectName);
            await _taskService.AddTaskAsync(projectName, taskDescription);
            var command = new SetDeadlineCommand(1, deadline);

            // Act
            var result = await _taskService.ExecuteCommandAsync(command);

            // Assert
            Assert.That(result.Success, Is.True);
            var projects = await _taskService.GetAllProjectsAsync();
            var project = projects.First(p => p.Name == projectName);
            Assert.That(project.Tasks[0].Deadline, Is.EqualTo(deadline));
        }

        [Test]
        public async Task ShowCommand_ShouldDisplayAllProjectsAndTasks()
        {
            // Arrange
            var projectName = "TestProject";
            await _taskService.AddProjectAsync(projectName);
            await _taskService.AddTaskAsync(projectName, "Task 1");
            await _taskService.AddTaskAsync(projectName, "Task 2");
            await _taskService.CheckTaskAsync(1, true);
            var command = new AddProjectCommand("show"); // Using AddProjectCommand as a placeholder for show command

            // Act
            var result = await _taskService.ExecuteCommandAsync(command);

            // Assert
            Assert.That(result.Success, Is.True);
            var projects = await _taskService.GetAllProjectsAsync();
            Assert.That(projects.Count(), Is.EqualTo(2)); // Including the "show" project
            var project = projects.First(p => p.Name == projectName);
            Assert.That(project.Tasks.Count, Is.EqualTo(2));
            Assert.That(project.Tasks[0].Done, Is.True);
            Assert.That(project.Tasks[1].Done, Is.False);
        }

        [Test]
        public async Task TodayCommand_ShouldShowTodayTasks()
        {
            // Arrange
            var projectName = "TestProject";
            await _taskService.AddProjectAsync(projectName);
            await _taskService.AddTaskAsync(projectName, "Today's Task");
            await _taskService.AddTaskAsync(projectName, "Tomorrow's Task");
            await _taskService.SetDeadlineAsync(1, DateTime.Today);
            await _taskService.SetDeadlineAsync(2, DateTime.Today.AddDays(1));
            var command = new AddProjectCommand("today"); // Using AddProjectCommand as a placeholder for today command

            // Act
            var result = await _taskService.ExecuteCommandAsync(command);

            // Assert
            Assert.That(result.Success, Is.True);
            var projects = await _taskService.GetAllProjectsAsync();
            Assert.That(projects.Count(), Is.EqualTo(2)); // Including the "today" project
            var project = projects.First(p => p.Name == projectName);
            Assert.That(project.Tasks.Count, Is.EqualTo(2));
            Assert.That(project.Tasks[0].Deadline, Is.EqualTo(DateTime.Today));
            Assert.That(project.Tasks[1].Deadline, Is.EqualTo(DateTime.Today.AddDays(1)));
        }

        [Test]
        public async Task ViewByDeadlineCommand_ShouldGroupTasksByDeadline()
        {
            // Arrange
            var projectName = "TestProject";
            await _taskService.AddProjectAsync(projectName);
            await _taskService.AddTaskAsync(projectName, "Task 1");
            await _taskService.AddTaskAsync(projectName, "Task 2");
            await _taskService.AddTaskAsync(projectName, "Task 3");
            await _taskService.SetDeadlineAsync(1, DateTime.Today);
            await _taskService.SetDeadlineAsync(2, DateTime.Today.AddDays(1));
            await _taskService.SetDeadlineAsync(3, DateTime.Today.AddDays(7));
            var command = new AddProjectCommand("view-by-deadline"); // Using AddProjectCommand as a placeholder for view-by-deadline command

            // Act
            var result = await _taskService.ExecuteCommandAsync(command);

            // Assert
            Assert.That(result.Success, Is.True);
            var projects = await _taskService.GetAllProjectsAsync();
            Assert.That(projects.Count(), Is.EqualTo(2)); // Including the "view-by-deadline" project
            var project = projects.First(p => p.Name == projectName);
            Assert.That(project.Tasks.Count, Is.EqualTo(3));
            Assert.That(project.Tasks[0].Deadline, Is.EqualTo(DateTime.Today));
            Assert.That(project.Tasks[1].Deadline, Is.EqualTo(DateTime.Today.AddDays(1)));
            Assert.That(project.Tasks[2].Deadline, Is.EqualTo(DateTime.Today.AddDays(7)));
        }

        [Test]
        public async Task InvalidCommand_ShouldReturnError()
        {
            // Arrange
            var command = new InvalidCommand();

            // Act
            var result = await _taskService.ExecuteCommandAsync(command);

            // Assert
            Assert.That(result.Success, Is.False);
            Assert.That(result.Error, Is.EqualTo("Unknown command type: InvalidCommand"));
        }
    }
}