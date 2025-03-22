using Microsoft.AspNetCore.Mvc;
using Moq;
using NUnit.Framework;
using TaskList.Core.Models;
using TaskList.Core.Models.Commands;
using TaskList.Core.Services;
using TaskList.Interfaces.Web.Controllers;
using TaskList.Interfaces.Web.DTOs;
using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using System.Linq;

namespace TaskList.Tests
{
    [TestFixture]
    public class ProjectControllerTests
    {
        private Mock<ITaskService> _taskServiceMock;
        private ProjectController _controller;

        [SetUp]
        public void Setup()
        {
            _taskServiceMock = new Mock<ITaskService>();
            _controller = new ProjectController(_taskServiceMock.Object);
        }

        [Test]
        public async Task GetProjects_ShouldReturnAllProjects()
        {
            // Arrange
            var projects = new List<Project>
            {
                new Project("Project 1") { Id = 1 },
                new Project("Project 2") { Id = 2 }
            };
            _taskServiceMock.Setup(x => x.GetAllProjectsAsync())
                .ReturnsAsync(projects);

            // Act
            var result = await _controller.GetProjects();

            // Assert
            Assert.That(result, Is.InstanceOf<OkObjectResult>());
            var okResult = result as OkObjectResult;
            Assert.That(okResult, Is.Not.Null);
            var returnedProjects = okResult.Value as IEnumerable<ProjectDto>;
            Assert.That(returnedProjects, Is.Not.Null);
            Assert.That(returnedProjects.Count(), Is.EqualTo(2));
            Assert.That(returnedProjects.First().Name, Is.EqualTo("Project 1"));
        }

        [Test]
        public void GetProject_WhenProjectExists_ShouldReturnProject()
        {
            // Arrange
            var project = new Project("Test Project") { Id = 1 };
            _taskServiceMock.Setup(x => x.GetProject(1))
                .Returns(project);

            // Act
            var result = _controller.GetProject(1);

            // Assert
            Assert.That(result, Is.InstanceOf<OkObjectResult>());
            var okResult = result as OkObjectResult;
            Assert.That(okResult, Is.Not.Null);
            var returnedProject = okResult.Value as ProjectDto;
            Assert.That(returnedProject, Is.Not.Null);
            Assert.That(returnedProject.Id, Is.EqualTo(1));
            Assert.That(returnedProject.Name, Is.EqualTo("Test Project"));
        }

        [Test]
        public void GetProject_WhenProjectDoesNotExist_ShouldReturnNotFound()
        {
            // Arrange
            _taskServiceMock.Setup(x => x.GetProject(1))
                .Throws(new InvalidOperationException($"Project with ID 1 not found"));

            // Act
            var result = _controller.GetProject(1);

            // Assert
            Assert.That(result, Is.InstanceOf<NotFoundObjectResult>());
            var notFoundResult = result as NotFoundObjectResult;
            Assert.That(notFoundResult, Is.Not.Null);
            Assert.That(notFoundResult.Value, Is.EqualTo("Project with ID 1 not found"));
        }

        [Test]
        public async Task CreateProject_ShouldCreateNewProject()
        {
            // Arrange
            var request = new CreateProjectRequest { Name = "New Project" };
            var project = new Project("New Project") { Id = 1 };
            var commandResult = new CommandResult { Success = true, Data = project };

            _taskServiceMock.Setup(x => x.ExecuteCommandAsync(It.IsAny<AddProjectCommand>()))
                .ReturnsAsync(commandResult);

            // Act
            var result = await _controller.CreateProject(request);

            // Assert
            Assert.That(result, Is.InstanceOf<CreatedAtActionResult>());
            var createdResult = result as CreatedAtActionResult;
            Assert.That(createdResult, Is.Not.Null);
            var returnedProject = createdResult.Value as ProjectDto;
            Assert.That(returnedProject, Is.Not.Null);
            Assert.That(returnedProject.Id, Is.EqualTo(1));
            Assert.That(returnedProject.Name, Is.EqualTo("New Project"));
        }

        [Test]
        public async Task CreateProject_WhenCommandFails_ShouldReturnBadRequest()
        {
            // Arrange
            var request = new CreateProjectRequest { Name = "New Project" };
            var commandResult = new CommandResult { Success = false, Error = "Project already exists" };

            _taskServiceMock.Setup(x => x.ExecuteCommandAsync(It.IsAny<AddProjectCommand>()))
                .ReturnsAsync(commandResult);

            // Act
            var result = await _controller.CreateProject(request);

            // Assert
            Assert.That(result, Is.InstanceOf<BadRequestObjectResult>());
            var badRequestResult = result as BadRequestObjectResult;
            Assert.That(badRequestResult, Is.Not.Null);
            Assert.That(badRequestResult.Value, Is.EqualTo("Project already exists"));
        }

        [Test]
        public async Task CreateTask_ShouldCreateNewTask()
        {
            // Arrange
            var project = new Project("Test Project") { Id = 1 };
            var task = new ProjectTask(1, "New Task", "Test Project") { Id = 1 };
            var request = new CreateTaskRequest { Description = "New Task", Deadline = DateTime.Today };

            _taskServiceMock.Setup(x => x.GetProject(1))
                .Returns(project);
            _taskServiceMock.Setup(x => x.ExecuteCommandAsync(It.IsAny<AddTaskCommand>()))
                .ReturnsAsync(new CommandResult { Success = true, Data = task });
            _taskServiceMock.Setup(x => x.ExecuteCommandAsync(It.IsAny<SetDeadlineCommand>()))
                .ReturnsAsync(new CommandResult { Success = true });

            // Act
            var result = await _controller.CreateTask(1, request);

            // Assert
            Assert.That(result, Is.InstanceOf<CreatedAtActionResult>());
            var createdResult = result as CreatedAtActionResult;
            Assert.That(createdResult, Is.Not.Null);
            var returnedTask = createdResult.Value as TaskDto;
            Assert.That(returnedTask, Is.Not.Null);
            Assert.That(returnedTask.Id, Is.EqualTo(1));
            Assert.That(returnedTask.Description, Is.EqualTo("New Task"));
        }

        [Test]
        public async Task CreateTask_WhenProjectDoesNotExist_ShouldReturnNotFound()
        {
            // Arrange
            var request = new CreateTaskRequest { Description = "New Task" };
            _taskServiceMock.Setup(x => x.GetProject(1))
                .Throws(new InvalidOperationException($"Project with ID 1 not found"));

            // Act
            var result = await _controller.CreateTask(1, request);

            // Assert
            Assert.That(result, Is.InstanceOf<NotFoundObjectResult>());
            var notFoundResult = result as NotFoundObjectResult;
            Assert.That(notFoundResult, Is.Not.Null);
            Assert.That(notFoundResult.Value, Is.EqualTo("Project with ID 1 not found"));
        }

        [Test]
        public async Task UpdateTaskDeadline_ShouldUpdateDeadline()
        {
            // Arrange
            var project = new Project("Test Project") { Id = 1 };
            var task = new ProjectTask(1, "Test Task", "Test Project") { Id = 1, Deadline = DateTime.Today.AddDays(1) };
            project.AddTask(task);
            var deadline = DateTime.Today.AddDays(1);

            _taskServiceMock.Setup(x => x.GetProject(1))
                .Returns(project);
            _taskServiceMock.Setup(x => x.ExecuteCommandAsync(It.IsAny<SetDeadlineCommand>()))
                .ReturnsAsync(new CommandResult { Success = true });

            // Act
            var result = await _controller.UpdateTaskDeadline(1, 1, deadline);

            // Assert
            Assert.That(result, Is.InstanceOf<OkObjectResult>());
            var okResult = result as OkObjectResult;
            Assert.That(okResult, Is.Not.Null);
            var returnedTask = okResult.Value as TaskDto;
            Assert.That(returnedTask, Is.Not.Null);
            Assert.That(returnedTask.Id, Is.EqualTo(1));
            Assert.That(returnedTask.Deadline, Is.EqualTo(deadline));
        }

        [Test]
        public async Task CheckTask_ShouldUpdateStatus()
        {
            // Arrange
            var project = new Project("Test Project") { Id = 1 };
            var task = new ProjectTask(1, "Test Task", "Test Project") { Id = 1 };
            project.AddTask(task);

            _taskServiceMock.Setup(x => x.GetProject(1))
                .Returns(project);
            _taskServiceMock.Setup(x => x.ExecuteCommandAsync(It.IsAny<BaseCommand>()))
                .ReturnsAsync(new CommandResult { Success = true })
                .Callback<BaseCommand>(cmd =>
                {
                    if (cmd is CheckTaskCommand checkCmd)
                    {
                        task.Done = checkCmd.Checked;
                    }
                });

            // Act
            var result = await _controller.CheckTask(1, 1, true);

            // Assert
            Assert.That(result, Is.InstanceOf<OkObjectResult>());
            var okResult = result as OkObjectResult;
            Assert.That(okResult, Is.Not.Null);
            var returnedTask = okResult.Value as TaskDto;
            Assert.That(returnedTask, Is.Not.Null);
            Assert.That(returnedTask.Id, Is.EqualTo(1));
            Assert.That(returnedTask.Done, Is.True);
        }

        [Test]
        public async Task GetTasksByDeadline_ShouldGroupTasksByDeadline()
        {
            // Arrange
            var project1 = new Project("Project 1") { Id = 1 };
            var project2 = new Project("Project 2") { Id = 2 };
            var today = DateTime.Today;
            var tomorrow = today.AddDays(1);

            project1.AddTask(new ProjectTask(1, "Task 1", "Project 1") { Id = 1, Deadline = today });
            project1.AddTask(new ProjectTask(2, "Task 2", "Project 1") { Id = 2, Deadline = tomorrow });
            project2.AddTask(new ProjectTask(3, "Task 3", "Project 2") { Id = 3, Deadline = today });

            var projects = new List<Project> { project1, project2 };
            _taskServiceMock.Setup(x => x.GetAllProjectsAsync())
                .ReturnsAsync(projects);

            // Act
            var result = await _controller.GetTasksByDeadline();

            // Assert
            Assert.That(result, Is.InstanceOf<OkObjectResult>());
            var okResult = result as OkObjectResult;
            Assert.That(okResult, Is.Not.Null);
            var groups = okResult.Value as IEnumerable<object>;
            Assert.That(groups, Is.Not.Null);
            Assert.That(groups.Count(), Is.EqualTo(2));

            // Convert to list for easier access
            var groupsList = groups.ToList();
            var todayGroup = groupsList[0];
            var tomorrowGroup = groupsList[1];

            // Use reflection to access properties
            var deadlineProperty = todayGroup.GetType().GetProperty("Deadline");
            var tasksProperty = todayGroup.GetType().GetProperty("Tasks");
            Assert.That(deadlineProperty, Is.Not.Null);
            Assert.That(tasksProperty, Is.Not.Null);

            Assert.That(deadlineProperty.GetValue(todayGroup), Is.EqualTo(today));
            var tasks = tasksProperty.GetValue(todayGroup) as IEnumerable<TaskDto>;
            Assert.That(tasks, Is.Not.Null);
            Assert.That(tasks.Count(), Is.EqualTo(2));
        }

        [Test]
        public async Task GetTasksBySpecificDeadline_ShouldFilterTasksByDeadline()
        {
            // Arrange
            var project1 = new Project("Project 1") { Id = 1 };
            var project2 = new Project("Project 2") { Id = 2 };
            var today = DateTime.Today;
            var tomorrow = today.AddDays(1);

            project1.AddTask(new ProjectTask(1, "Task 1", "Project 1") { Id = 1, Deadline = today });
            project1.AddTask(new ProjectTask(2, "Task 2", "Project 1") { Id = 2, Deadline = tomorrow });
            project2.AddTask(new ProjectTask(3, "Task 3", "Project 2") { Id = 3, Deadline = today });

            var projects = new List<Project> { project1, project2 };
            _taskServiceMock.Setup(x => x.GetAllProjectsAsync())
                .ReturnsAsync(projects);

            // Act
            var result = await _controller.GetTasksBySpecificDeadline(today);

            // Assert
            Assert.That(result, Is.InstanceOf<OkObjectResult>());
            var okResult = result as OkObjectResult;
            Assert.That(okResult, Is.Not.Null);
            var tasks = okResult.Value as IEnumerable<TaskDto>;
            Assert.That(tasks, Is.Not.Null);
            Assert.That(tasks.Count(), Is.EqualTo(2));
            Assert.That(tasks.All(t => t.Deadline == today), Is.True);
        }

        [Test]
        public void GetTask_WhenTaskDoesNotExist_ShouldReturnNotFound()
        {
            // Arrange
            var project = new Project("Test Project") { Id = 1 };
            project.AddTask(new ProjectTask(1, "Other Task", "Test Project") { Id = 1 });
            _taskServiceMock.Setup(x => x.GetProject(1))
                .Returns(project);

            // Act
            var result = _controller.GetTask(1, 999);

            // Assert
            Assert.That(result, Is.InstanceOf<NotFoundObjectResult>());
            var notFoundResult = result as NotFoundObjectResult;
            Assert.That(notFoundResult, Is.Not.Null);
            Assert.That(notFoundResult.Value, Is.Not.Null);
            Assert.That(notFoundResult.Value.ToString()!, Does.Contain("Task with ID 999 not found in project 1"));
        }

        [Test]
        public async Task CreateTask_WhenCommandFails_ShouldReturnBadRequest()
        {
            // Arrange
            var project = new Project("Test Project") { Id = 1 };
            var request = new CreateTaskRequest { Description = "New Task" };
            var commandResult = new CommandResult { Success = false, Error = "Failed to create task" };

            _taskServiceMock.Setup(x => x.GetProject(1))
                .Returns(project);
            _taskServiceMock.Setup(x => x.ExecuteCommandAsync(It.IsAny<AddTaskCommand>()))
                .ReturnsAsync(commandResult);

            // Act
            var result = await _controller.CreateTask(1, request);

            // Assert
            Assert.That(result, Is.InstanceOf<BadRequestObjectResult>());
            var badRequestResult = result as BadRequestObjectResult;
            Assert.That(badRequestResult, Is.Not.Null);
            Assert.That(badRequestResult.Value, Is.EqualTo("Failed to create task"));
        }

        [Test]
        public async Task CreateTask_WhenDeadlineSettingFails_ShouldReturnBadRequest()
        {
            // Arrange
            var project = new Project("Test Project") { Id = 1 };
            var task = new ProjectTask(1, "New Task", "Test Project") { Id = 1 };
            var request = new CreateTaskRequest
            {
                Description = "New Task",
                Deadline = DateTime.Today
            };

            _taskServiceMock.Setup(x => x.GetProject(1))
                .Returns(project);
            _taskServiceMock.Setup(x => x.ExecuteCommandAsync(It.IsAny<AddTaskCommand>()))
                .ReturnsAsync(new CommandResult { Success = true, Data = task });
            _taskServiceMock.Setup(x => x.ExecuteCommandAsync(It.IsAny<SetDeadlineCommand>()))
                .ReturnsAsync(new CommandResult { Success = false, Error = "Invalid deadline" });

            // Act
            var result = await _controller.CreateTask(1, request);

            // Assert
            Assert.That(result, Is.InstanceOf<BadRequestObjectResult>());
            var badRequestResult = result as BadRequestObjectResult;
            Assert.That(badRequestResult, Is.Not.Null);
            Assert.That(badRequestResult.Value, Is.EqualTo("Failed to set task deadline"));
        }

        [Test]
        public async Task UpdateTaskDeadline_WhenCommandFails_ShouldReturnBadRequest()
        {
            // Arrange
            var project = new Project("Test Project") { Id = 1 };
            var task = new ProjectTask(1, "Test Task", "Test Project") { Id = 1 };
            project.AddTask(task);
            var deadline = DateTime.Today.AddDays(1);

            _taskServiceMock.Setup(x => x.GetProject(1))
                .Returns(project);
            _taskServiceMock.Setup(x => x.ExecuteCommandAsync(It.IsAny<SetDeadlineCommand>()))
                .ReturnsAsync(new CommandResult { Success = false, Error = "Invalid deadline" });

            // Act
            var result = await _controller.UpdateTaskDeadline(1, 1, deadline);

            // Assert
            Assert.That(result, Is.InstanceOf<BadRequestObjectResult>());
            var badRequestResult = result as BadRequestObjectResult;
            Assert.That(badRequestResult, Is.Not.Null);
            Assert.That(badRequestResult.Value, Is.EqualTo("Invalid deadline"));
        }

        [Test]
        public async Task UpdateTaskDeadline_WhenTaskDoesNotExist_ShouldReturnNotFound()
        {
            // Arrange
            var project = new Project("Test Project") { Id = 1 };
            var deadline = DateTime.Today.AddDays(1);
            project.AddTask(new ProjectTask(1, "Other Task", "Test Project") { Id = 1 });

            _taskServiceMock.Setup(x => x.GetProject(1))
                .Returns(project);

            // Act
            var result = await _controller.UpdateTaskDeadline(1, 999, deadline);

            // Assert
            Assert.That(result, Is.InstanceOf<NotFoundObjectResult>());
            var notFoundResult = result as NotFoundObjectResult;
            Assert.That(notFoundResult, Is.Not.Null);
            Assert.That(notFoundResult.Value, Is.Not.Null);
            Assert.That(notFoundResult.Value.ToString()!, Does.Contain("Task with ID 999 not found in project 1"));
        }

        [Test]
        public async Task CheckTask_WhenCommandFails_ShouldReturnBadRequest()
        {
            // Arrange
            var project = new Project("Test Project") { Id = 1 };
            var task = new ProjectTask(1, "Test Task", "Test Project") { Id = 1 };
            project.AddTask(task);

            _taskServiceMock.Setup(x => x.GetProject(1))
                .Returns(project);
            _taskServiceMock.Setup(x => x.ExecuteCommandAsync(It.IsAny<CheckTaskCommand>()))
                .ReturnsAsync(new CommandResult { Success = false, Error = "Failed to update task status" });

            // Act
            var result = await _controller.CheckTask(1, 1, true);

            // Assert
            Assert.That(result, Is.InstanceOf<BadRequestObjectResult>());
            var badRequestResult = result as BadRequestObjectResult;
            Assert.That(badRequestResult, Is.Not.Null);
            Assert.That(badRequestResult.Value, Is.EqualTo("Failed to update task status"));
        }

        [Test]
        public async Task CheckTask_WhenTaskDoesNotExist_ShouldReturnNotFound()
        {
            // Arrange
            var project = new Project("Test Project") { Id = 1 };
            project.AddTask(new ProjectTask(1, "Other Task", "Test Project") { Id = 1 });

            _taskServiceMock.Setup(x => x.GetProject(1))
                .Returns(project);

            // Act
            var result = await _controller.CheckTask(1, 999, true);

            // Assert
            Assert.That(result, Is.InstanceOf<NotFoundObjectResult>());
            var notFoundResult = result as NotFoundObjectResult;
            Assert.That(notFoundResult, Is.Not.Null);
            Assert.That(notFoundResult.Value, Is.Not.Null);
            Assert.That(notFoundResult.Value.ToString()!, Does.Contain("Task with ID 999 not found in project 1"));
        }

        [Test]
        public async Task GetTasksBySpecificDeadline_WhenNoTasksExist_ShouldReturnEmptyList()
        {
            // Arrange
            var project = new Project("Test Project") { Id = 1 };
            var projects = new List<Project> { project };
            _taskServiceMock.Setup(x => x.GetAllProjectsAsync())
                .ReturnsAsync(projects);

            // Act
            var result = await _controller.GetTasksBySpecificDeadline(DateTime.Today);

            // Assert
            Assert.That(result, Is.InstanceOf<OkObjectResult>());
            var okResult = result as OkObjectResult;
            Assert.That(okResult, Is.Not.Null);
            var tasks = okResult.Value as IEnumerable<TaskDto>;
            Assert.That(tasks, Is.Not.Null);
            Assert.That(tasks, Is.Empty);
        }
    }
}