using NUnit.Framework;
using TaskList.Core.Models;
using TaskList.Core.Services;
using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using System.Linq;

namespace TaskList.Tests
{
    [TestFixture]
    public class TaskServiceTests
    {
        private ITaskService _taskService;

        [SetUp]
        public void Setup()
        {
            _taskService = new TaskService();
        }

        [Test]
        public async Task AddProject_ShouldCreateNewProject()
        {
            // Arrange
            var projectName = "TestProject";

            // Act
            var project = await _taskService.AddProjectAsync(projectName);

            // Assert
            Assert.That(project.Name, Is.EqualTo(projectName));
            var projects = await _taskService.GetAllProjectsAsync();
            Assert.That(projects.Count(), Is.EqualTo(1));
        }

        [Test]
        public async Task AddProject_WithExistingName_ShouldFail()
        {
            // Arrange
            var projectName = "TestProject";
            await _taskService.AddProjectAsync(projectName);

            // Act & Assert
            Assert.ThrowsAsync<InvalidOperationException>(() =>
                _taskService.AddProjectAsync(projectName));
        }

        [Test]
        public async Task AddTask_ToExistingProject_ShouldCreateTask()
        {
            // Arrange
            var projectName = "TestProject";
            var taskDescription = "Test Task";
            await _taskService.AddProjectAsync(projectName);

            // Act
            var task = await _taskService.AddTaskAsync(projectName, taskDescription);

            // Assert
            Assert.That(task.Description, Is.EqualTo(taskDescription));
            var projects = await _taskService.GetAllProjectsAsync();
            var project = projects.First(p => p.Name == projectName);
            Assert.That(project.Tasks, Has.Count.EqualTo(1));
        }

        [Test]
        public async Task AddTask_ToNonExistentProject_ShouldFail()
        {
            // Arrange
            var projectName = "NonExistentProject";
            var taskDescription = "Test Task";

            // Act & Assert
            Assert.ThrowsAsync<InvalidOperationException>(() =>
                _taskService.AddTaskAsync(projectName, taskDescription));
        }

        [Test]
        public async Task CheckTask_WithValidId_ShouldMarkTaskAsDone()
        {
            // Arrange
            var projectName = "TestProject";
            var taskDescription = "Test Task";
            await _taskService.AddProjectAsync(projectName);
            await _taskService.AddTaskAsync(projectName, taskDescription);

            // Act
            var result = await _taskService.CheckTaskAsync(1, true);

            // Assert
            Assert.That(result, Is.True);
            var projects = await _taskService.GetAllProjectsAsync();
            var project = projects.First(p => p.Name == projectName);
            Assert.That(project.Tasks[0].Done, Is.True);
        }

        [Test]
        public async Task CheckTask_WithInvalidId_ShouldFail()
        {
            // Arrange
            var invalidId = 999;

            // Act
            var result = await _taskService.CheckTaskAsync(invalidId, true);

            // Assert
            Assert.That(result, Is.False);
        }

        [Test]
        public async Task SetDeadline_WithValidDate_ShouldSetDeadline()
        {
            // Arrange
            var projectName = "TestProject";
            var taskDescription = "Test Task";
            var deadline = DateTime.Today.AddDays(1);
            await _taskService.AddProjectAsync(projectName);
            await _taskService.AddTaskAsync(projectName, taskDescription);

            // Act
            var result = await _taskService.SetDeadlineAsync(1, deadline);

            // Assert
            Assert.That(result, Is.True);
            var projects = await _taskService.GetAllProjectsAsync();
            var project = projects.First(p => p.Name == projectName);
            Assert.That(project.Tasks[0].Deadline, Is.EqualTo(deadline));
        }

        [Test]
        public async Task SetDeadline_WithInvalidId_ShouldFail()
        {
            // Arrange
            var invalidId = 999;
            var deadline = DateTime.Today.AddDays(1);

            // Act
            var result = await _taskService.SetDeadlineAsync(invalidId, deadline);

            // Assert
            Assert.That(result, Is.False);
        }

        [Test]
        public async Task GetTasksByDeadline_ShouldGroupTasksCorrectly()
        {
            // Arrange
            var projectName = "TestProject";
            await _taskService.AddProjectAsync(projectName);
            await _taskService.AddTaskAsync(projectName, "Task 1");
            await _taskService.AddTaskAsync(projectName, "Task 2");
            await _taskService.AddTaskAsync(projectName, "Task 3");

            var today = DateTime.Today;
            var tomorrow = today.AddDays(1);
            var nextWeek = today.AddDays(7);

            await _taskService.SetDeadlineAsync(1, today);
            await _taskService.SetDeadlineAsync(2, tomorrow);
            await _taskService.SetDeadlineAsync(3, nextWeek);

            // Act
            var todayTasks = await _taskService.GetTasksByDeadlineAsync(today);
            var tomorrowTasks = await _taskService.GetTasksByDeadlineAsync(tomorrow);
            var nextWeekTasks = await _taskService.GetTasksByDeadlineAsync(nextWeek);

            // Assert
            Assert.That(todayTasks.Count(), Is.EqualTo(1));
            Assert.That(tomorrowTasks.Count(), Is.EqualTo(1));
            Assert.That(nextWeekTasks.Count(), Is.EqualTo(1));
        }

        [Test]
        public async Task GetTasksForToday_ShouldReturnOnlyTodayTasks()
        {
            // Arrange
            var projectName = "TestProject";
            await _taskService.AddProjectAsync(projectName);
            await _taskService.AddTaskAsync(projectName, "Today's Task");
            await _taskService.AddTaskAsync(projectName, "Tomorrow's Task");
            await _taskService.AddTaskAsync(projectName, "Next Week's Task");

            var today = DateTime.Today;
            var tomorrow = today.AddDays(1);
            var nextWeek = today.AddDays(7);

            await _taskService.SetDeadlineAsync(1, today);
            await _taskService.SetDeadlineAsync(2, tomorrow);
            await _taskService.SetDeadlineAsync(3, nextWeek);

            // Act
            var todayTasks = await _taskService.GetTasksForTodayAsync();

            // Assert
            Assert.That(todayTasks.Count(), Is.EqualTo(1));
            Assert.That(todayTasks.First().Description, Is.EqualTo("Today's Task"));
        }
    }
}