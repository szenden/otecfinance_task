using NUnit.Framework;
using TaskList.Core.Models;
using System;

namespace TaskList.Tests
{
    [TestFixture]
    public class ModelTests
    {
        [Test]
        public void Project_Constructor_ShouldInitializeProperties()
        {
            // Arrange
            var projectName = "TestProject";

            // Act
            var project = new Project(projectName);

            // Assert
            Assert.That(project.Name, Is.EqualTo(projectName));
            Assert.That(project.Tasks, Is.Empty);
        }

        [Test]
        public void Project_AddTask_ShouldAddTaskToList()
        {
            // Arrange
            var project = new Project("TestProject");
            var taskDescription = "Test Task";
            var task = new ProjectTask(1, taskDescription, project.Name);

            // Act
            project.AddTask(task);

            // Assert
            Assert.That(project.Tasks.Count, Is.EqualTo(1));
            Assert.That(project.Tasks[0].Description, Is.EqualTo(taskDescription));
            Assert.That(project.Tasks[0].Id, Is.EqualTo(1));
        }

        [Test]
        public void Project_AddTask_ShouldIncrementTaskIds()
        {
            // Arrange
            var project = new Project("TestProject");

            // Act
            project.AddTask(new ProjectTask(1, "Task 1", project.Name));
            project.AddTask(new ProjectTask(2, "Task 2", project.Name));
            project.AddTask(new ProjectTask(3, "Task 3", project.Name));

            // Assert
            Assert.That(project.Tasks[0].Id, Is.EqualTo(1));
            Assert.That(project.Tasks[1].Id, Is.EqualTo(2));
            Assert.That(project.Tasks[2].Id, Is.EqualTo(3));
        }

        [Test]
        public void ProjectTask_Constructor_ShouldInitializeProperties()
        {
            // Arrange
            var id = 1;
            var description = "Test Task";
            var projectName = "TestProject";

            // Act
            var task = new ProjectTask(id, description, projectName);

            // Assert
            Assert.That(task.Id, Is.EqualTo(id));
            Assert.That(task.Description, Is.EqualTo(description));
            Assert.That(task.Done, Is.False);
            Assert.That(task.Deadline, Is.Null);
        }

        [Test]
        public void ProjectTask_SetDeadline_ShouldUpdateDeadline()
        {
            // Arrange
            var task = new ProjectTask(1, "Test Task", "TestProject");
            var deadline = DateTime.Today.AddDays(1);

            // Act
            task.Deadline = deadline;

            // Assert
            Assert.That(task.Deadline, Is.EqualTo(deadline));
        }

        [Test]
        public void ProjectTask_Check_ShouldMarkTaskAsDone()
        {
            // Arrange
            var task = new ProjectTask(1, "Test Task", "TestProject");

            // Act
            task.Done = true;

            // Assert
            Assert.That(task.Done, Is.True);
        }

        [Test]
        public void ProjectTask_Uncheck_ShouldMarkTaskAsNotDone()
        {
            // Arrange
            var task = new ProjectTask(1, "Test Task", "TestProject");
            task.Done = true;

            // Act
            task.Done = false;

            // Assert
            Assert.That(task.Done, Is.False);
        }

        [Test]
        public void ProjectTask_ToString_ShouldFormatCorrectly()
        {
            // Arrange
            var task = new ProjectTask(1, "Test Task", "TestProject");
            task.Deadline = new DateTime(2024, 1, 1);

            // Act
            var result = task.ToString();

            // Assert
            Assert.That(result, Is.EqualTo("1: Test Task, 01-01-2024"));
        }

        [Test]
        public void ProjectTask_ToString_WithoutDeadline_ShouldFormatCorrectly()
        {
            // Arrange
            var task = new ProjectTask(1, "Test Task", "TestProject");

            // Act
            var result = task.ToString();

            // Assert
            var expected = $"{task.Id}: {task.Description}";
            Assert.That(result, Is.EqualTo(expected));
        }

        [Test]
        public void ProjectTask_ToString_WhenDone_ShouldIncludeCheckmark()
        {
            // Arrange
            var task = new ProjectTask(1, "Test Task", "TestProject");
            task.Done = true;

            // Act
            var result = task.ToString();

            // Assert
            Assert.That(result, Is.EqualTo("1: Test Task [x]"));
        }
    }
}