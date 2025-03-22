using Microsoft.AspNetCore.Mvc;
using TaskList.Core.Models;
using TaskList.Core.Models.Commands;
using TaskList.Core.Services;
using TaskList.Interfaces.Web.DTOs;

namespace TaskList.Interfaces.Web.Controllers
{
    /// <summary>
    /// Controller for managing tasks and projects through a RESTful API.
    /// Provides endpoints for creating, updating, and retrieving tasks and projects.
    /// </summary>
    [ApiController]
    [Route("api/[controller]")]
    public class TaskController : ControllerBase
    {
        private readonly ITaskService _taskService;

        /// <summary>
        /// Initializes a new instance of the TaskController class.
        /// </summary>
        /// <param name="taskService">The service for managing tasks and projects.</param>
        public TaskController(ITaskService taskService)
        {
            _taskService = taskService;
        }

        /// <summary>
        /// Retrieves all projects and their associated tasks.
        /// </summary>
        /// <returns>A collection of projects with their tasks.</returns>
        [HttpGet("projects")]
        public async Task<ActionResult<IEnumerable<ProjectDto>>> GetProjects()
        {
            var projects = await _taskService.GetAllProjectsAsync();
            return Ok(MapToProjectDtos(projects));
        }

        /// <summary>
        /// Creates a new project.
        /// </summary>
        /// <param name="request">The project creation request containing the project name.</param>
        /// <returns>The newly created project.</returns>
        /// <response code="200">Returns the newly created project.</response>
        /// <response code="400">If the project creation fails.</response>
        [HttpPost("projects")]
        public async Task<ActionResult<ProjectDto>> AddProject([FromBody] AddProjectRequest request)
        {
            var command = new AddProjectCommand(request.ProjectName);
            var result = await _taskService.ExecuteCommandAsync(command);

            if (!result.Success)
            {
                return BadRequest(result.Error);
            }

            var project = await _taskService.GetAllProjectsAsync();
            return Ok(MapToProjectDto(project.First(p => p.Name == request.ProjectName)));
        }

        /// <summary>
        /// Creates a new task in the specified project.
        /// </summary>
        /// <param name="request">The task creation request containing project ID and task description.</param>
        /// <returns>The newly created task.</returns>
        /// <response code="200">Returns the newly created task.</response>
        /// <response code="400">If the task creation fails.</response>
        [HttpPost("tasks")]
        public async Task<ActionResult<TaskDto>> AddTask([FromBody] AddTaskRequest request)
        {
            var project = _taskService.GetProject(request.ProjectId);
            if (project == null)
            {
                return BadRequest($"Project with ID {request.ProjectId} not found");
            }

            var command = new AddTaskCommand(request.ProjectId, request.Description);
            var result = await _taskService.ExecuteCommandAsync(command);

            if (!result.Success)
            {
                return BadRequest(result.Error);
            }

            var updatedProject = _taskService.GetProject(request.ProjectId);
            var newTask = updatedProject.Tasks.Last();
            return Ok(MapToTaskDto(newTask));
        }

        /// <summary>
        /// Marks a task as completed.
        /// </summary>
        /// <param name="id">The unique identifier of the task.</param>
        /// <returns>No content on success.</returns>
        /// <response code="200">If the task was successfully marked as completed.</response>
        /// <response code="400">If the operation fails.</response>
        [HttpPut("tasks/{id}/check")]
        public async Task<IActionResult> CheckTask(long id)
        {
            var command = new CheckTaskCommand(id, true);
            var result = await _taskService.ExecuteCommandAsync(command);

            if (!result.Success)
            {
                return BadRequest(result.Error);
            }

            return Ok();
        }

        /// <summary>
        /// Marks a task as not completed.
        /// </summary>
        /// <param name="id">The unique identifier of the task.</param>
        /// <returns>No content on success.</returns>
        /// <response code="200">If the task was successfully marked as not completed.</response>
        /// <response code="400">If the operation fails.</response>
        [HttpPut("tasks/{id}/uncheck")]
        public async Task<IActionResult> UncheckTask(long id)
        {
            var command = new CheckTaskCommand(id, false);
            var result = await _taskService.ExecuteCommandAsync(command);

            if (!result.Success)
            {
                return BadRequest(result.Error);
            }

            return Ok();
        }

        /// <summary>
        /// Sets a deadline for a task.
        /// </summary>
        /// <param name="id">The unique identifier of the task.</param>
        /// <param name="request">The request containing the new deadline.</param>
        /// <returns>No content on success.</returns>
        /// <response code="200">If the deadline was successfully set.</response>
        /// <response code="400">If the operation fails.</response>
        [HttpPut("tasks/{id}/deadline")]
        public async Task<IActionResult> SetDeadline(long id, [FromBody] SetDeadlineRequest request)
        {
            var command = new SetDeadlineCommand(id, request.Deadline);
            var result = await _taskService.ExecuteCommandAsync(command);

            if (!result.Success)
            {
                return BadRequest(result.Error);
            }

            return Ok();
        }

        /// <summary>
        /// Retrieves all tasks that are due today.
        /// </summary>
        /// <returns>A collection of tasks due today.</returns>
        [HttpGet("tasks/today")]
        public async Task<ActionResult<IEnumerable<TaskDto>>> GetTodayTasks()
        {
            var tasks = await _taskService.GetTasksForTodayAsync();
            return Ok(MapToTaskDtos(tasks));
        }

        /// <summary>
        /// Retrieves tasks filtered by their deadline.
        /// </summary>
        /// <param name="deadline">The deadline to filter tasks by. If null, returns all tasks.</param>
        /// <returns>A collection of tasks matching the deadline criteria.</returns>
        [HttpGet("tasks/by-deadline")]
        public async Task<ActionResult<IEnumerable<TaskDto>>> GetTasksByDeadline([FromQuery] DateTime? deadline)
        {
            var tasks = await _taskService.GetTasksByDeadlineAsync(deadline);
            return Ok(MapToTaskDtos(tasks));
        }

        /// <summary>
        /// Maps a collection of projects to project DTOs.
        /// </summary>
        /// <param name="projects">The projects to map.</param>
        /// <returns>A collection of project DTOs.</returns>
        private static IEnumerable<ProjectDto> MapToProjectDtos(IEnumerable<Project> projects)
        {
            return projects.Select(MapToProjectDto);
        }

        /// <summary>
        /// Maps a project to a project DTO.
        /// </summary>
        /// <param name="project">The project to map.</param>
        /// <returns>A project DTO.</returns>
        private static ProjectDto MapToProjectDto(Project project)
        {
            return new ProjectDto
            {
                Name = project.Name,
                Tasks = project.Tasks.Select(MapToTaskDto).ToList()
            };
        }

        /// <summary>
        /// Maps a collection of tasks to task DTOs.
        /// </summary>
        /// <param name="tasks">The tasks to map.</param>
        /// <returns>A collection of task DTOs.</returns>
        private static IEnumerable<TaskDto> MapToTaskDtos(IEnumerable<ProjectTask> tasks)
        {
            return tasks.Select(MapToTaskDto);
        }

        /// <summary>
        /// Maps a task to a task DTO.
        /// </summary>
        /// <param name="task">The task to map.</param>
        /// <returns>A task DTO.</returns>
        private static TaskDto MapToTaskDto(ProjectTask task)
        {
            return new TaskDto
            {
                Id = task.Id,
                Description = task.Description,
                Done = task.Done,
                Deadline = task.Deadline,
                ProjectName = task.ProjectName
            };
        }
    }
}