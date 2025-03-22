using Microsoft.AspNetCore.Mvc;
using TaskList.Core.Models;
using TaskList.Core.Models.Commands;
using TaskList.Core.Services;
using TaskList.Interfaces.Web.DTOs;

namespace TaskList.Interfaces.Web.Controllers
{
    /// <summary>
    /// Controller for managing projects and their tasks through a RESTful API.
    /// Provides endpoints for creating, updating, and retrieving projects and tasks.
    /// </summary>
    [ApiController]
    [Route("api/[controller]")]
    public class ProjectController : ControllerBase
    {
        private readonly ITaskService _taskService;

        /// <summary>
        /// Initializes a new instance of the ProjectController class.
        /// </summary>
        /// <param name="taskService">The service for managing tasks and projects.</param>
        public ProjectController(ITaskService taskService)
        {
            _taskService = taskService;
        }

        /// <summary>
        /// Retrieves all projects.
        /// </summary>
        /// <returns>A list of all projects.</returns>
        /// <response code="200">Returns the list of all projects.</response>
        [HttpGet]
        public async Task<IActionResult> GetProjects()
        {
            var projects = await _taskService.GetAllProjectsAsync();
            return Ok(projects.Select(MapToProjectDto));
        }

        /// <summary>
        /// Retrieves a project by its ID.
        /// </summary>
        /// <param name="projectId">The ID of the project to retrieve.</param>
        /// <returns>The project if found.</returns>
        /// <response code="200">Returns the requested project.</response>
        /// <response code="404">If the project is not found.</response>
        [HttpGet("{projectId}")]
        public IActionResult GetProject(long projectId)
        {
            try
            {
                var project = _taskService.GetProject(projectId);
                return Ok(MapToProjectDto(project));
            }
            catch (InvalidOperationException ex)
            {
                return NotFound(ex.Message);
            }
        }

        /// <summary>
        /// Retrieves a specific task from a project.
        /// </summary>
        /// <param name="projectId">The ID of the project containing the task.</param>
        /// <param name="taskId">The ID of the task to retrieve.</param>
        /// <returns>The task if found.</returns>
        /// <response code="200">Returns the requested task.</response>
        /// <response code="404">If the project or task is not found.</response>
        [HttpGet("{projectId}/tasks/{taskId}")]
        public IActionResult GetTask(long projectId, long taskId)
        {
            try
            {
                var project = _taskService.GetProject(projectId);
                var task = project.GetTaskById(taskId);
                if (task == null)
                {
                    return NotFound($"Task with ID {taskId} not found in project {projectId}");
                }
                return Ok(MapToTaskDto(task));
            }
            catch (InvalidOperationException ex)
            {
                return NotFound(ex.Message);
            }
        }

        /// <summary>
        /// Creates a new project.
        /// </summary>
        /// <param name="request">The project creation request containing the project name.</param>
        /// <returns>The newly created project.</returns>
        /// <response code="201">Returns the newly created project.</response>
        /// <response code="400">If the project creation fails.</response>
        [HttpPost]
        public async Task<IActionResult> CreateProject([FromBody] CreateProjectRequest request)
        {
            var command = new AddProjectCommand(request.Name);
            var result = await _taskService.ExecuteCommandAsync(command);

            if (!result.Success)
                return BadRequest(result.Error);

            if (result.Data is not Project project)
                return BadRequest("Failed to create project: Invalid response data");

            return CreatedAtAction(nameof(GetProject), new { projectId = project.Id }, MapToProjectDto(project));
        }

        /// <summary>
        /// Creates a new task in the specified project.
        /// </summary>
        /// <param name="projectId">The ID of the project to add the task to.</param>
        /// <param name="request">The task creation request containing task details.</param>
        /// <returns>The newly created task.</returns>
        /// <response code="201">Returns the newly created task.</response>
        /// <response code="400">If the task creation fails.</response>
        /// <response code="404">If the project is not found.</response>
        [HttpPost("{projectId}/tasks")]
        public async Task<IActionResult> CreateTask(long projectId, [FromBody] CreateTaskRequest request)
        {
            try
            {
                var project = _taskService.GetProject(projectId);
                var command = new AddTaskCommand(projectId, request.Description);
                var result = await _taskService.ExecuteCommandAsync(command);

                if (!result.Success)
                    return BadRequest(result.Error);

                if (result.Data is not ProjectTask task)
                    return BadRequest("Failed to create task: Invalid response data");

                if (request.Deadline.HasValue)
                {
                    var deadlineCommand = new SetDeadlineCommand(task.Id, request.Deadline.Value.Date);
                    var deadlineResult = await _taskService.ExecuteCommandAsync(deadlineCommand);
                    if (!deadlineResult.Success)
                        return BadRequest("Failed to set task deadline");
                }

                return CreatedAtAction(nameof(GetTask), new { projectId, taskId = task.Id }, MapToTaskDto(task));
            }
            catch (InvalidOperationException ex)
            {
                return NotFound(ex.Message);
            }
        }

        /// <summary>
        /// Updates the deadline of a task.
        /// </summary>
        /// <param name="projectId">The ID of the project containing the task.</param>
        /// <param name="taskId">The ID of the task to update.</param>
        /// <param name="deadline">The new deadline for the task.</param>
        /// <returns>The updated task.</returns>
        /// <response code="200">Returns the updated task.</response>
        /// <response code="400">If the deadline update fails.</response>
        /// <response code="404">If the project or task is not found.</response>
        [HttpPut("{projectId}/tasks/{taskId}/deadline")]
        public async Task<IActionResult> UpdateTaskDeadline(long projectId, long taskId, [FromQuery] DateTime deadline)
        {
            try
            {
                var project = _taskService.GetProject(projectId);
                var task = project.GetTaskById(taskId);
                if (task == null)
                {
                    return NotFound($"Task with ID {taskId} not found in project {projectId}");
                }
                var command = new SetDeadlineCommand(taskId, deadline);
                var result = await _taskService.ExecuteCommandAsync(command);

                if (!result.Success)
                    return BadRequest(result.Error);

                return Ok(MapToTaskDto(project.GetTaskById(taskId)));
            }
            catch (InvalidOperationException ex)
            {
                return NotFound(ex.Message);
            }
        }

        /// <summary>
        /// Updates the completion status of a task.
        /// </summary>
        /// <param name="projectId">The ID of the project containing the task.</param>
        /// <param name="taskId">The ID of the task to update.</param>
        /// <param name="checked">The new completion status of the task.</param>
        /// <returns>The updated task.</returns>
        /// <response code="200">Returns the updated task.</response>
        /// <response code="400">If the status update fails.</response>
        /// <response code="404">If the project or task is not found.</response>
        [HttpPut("{projectId}/tasks/{taskId}/check")]
        public async Task<IActionResult> CheckTask(long projectId, long taskId, [FromQuery] bool @checked)
        {
            try
            {
                var project = _taskService.GetProject(projectId);
                var task = project.GetTaskById(taskId);
                if (task == null)
                {
                    return NotFound($"Task with ID {taskId} not found in project {projectId}");
                }
                var command = new CheckTaskCommand(taskId, @checked);
                var result = await _taskService.ExecuteCommandAsync(command);

                if (!result.Success)
                    return BadRequest(result.Error);

                return Ok(MapToTaskDto(project.GetTaskById(taskId)));
            }
            catch (InvalidOperationException ex)
            {
                return NotFound(ex.Message);
            }

        }

        /// <summary>
        /// Retrieves all tasks grouped by their deadlines.
        /// </summary>
        /// <returns>A list of deadline groups containing tasks.</returns>
        /// <response code="200">Returns the tasks grouped by deadline.</response>
        [HttpGet("view_by_deadline")]
        public async Task<IActionResult> GetTasksByDeadline()
        {
            var allProjects = await _taskService.GetAllProjectsAsync();
            var tasksByDeadline = allProjects
                .SelectMany(p => p.Tasks)
                .GroupBy(t => t.Deadline)
                .OrderBy(g => g.Key == null)
                .ThenBy(g => g.Key)
                .Select(g => new
                {
                    Deadline = g.Key,
                    Tasks = g.Select(MapToTaskDto).ToList()
                })
                .ToList();

            return Ok(tasksByDeadline);
        }

        /// <summary>
        /// Retrieves tasks filtered by their deadline.
        /// </summary>
        /// <param name="deadline">The deadline to filter tasks by. If not provided, defaults to the current day.</param>
        /// <returns>A list of tasks with the specified deadline.</returns>
        /// <response code="200">Returns the filtered tasks.</response>
        [HttpGet("tasks/by-deadline")]
        public async Task<IActionResult> GetTasksBySpecificDeadline([FromQuery] DateTime? deadline = null)
        {
            var targetDate = deadline?.Date ?? DateTime.Today;
            var allProjects = await _taskService.GetAllProjectsAsync();

            var tasks = allProjects
                .SelectMany(p => p.Tasks)
                .Where(t => t.Deadline?.Date == targetDate)
                .Select(MapToTaskDto)
                .ToList();

            return Ok(tasks);
        }

        /// <summary>
        /// Maps a Project entity to a ProjectDto.
        /// </summary>
        /// <param name="project">The project entity to map.</param>
        /// <returns>A ProjectDto containing the mapped project data.</returns>
        private static ProjectDto MapToProjectDto(Project project)
        {
            return new ProjectDto
            {
                Id = project.Id,
                Name = project.Name,
                Tasks = project.Tasks.Select(MapToTaskDto).ToList()
            };
        }

        /// <summary>
        /// Maps a ProjectTask entity to a TaskDto.
        /// </summary>
        /// <param name="task">The task entity to map.</param>
        /// <returns>A TaskDto containing the mapped task data.</returns>
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