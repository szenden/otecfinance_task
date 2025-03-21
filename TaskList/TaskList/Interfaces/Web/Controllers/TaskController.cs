using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using TaskList.Core.Models;
using TaskList.Core.Models.Commands;
using TaskList.Core.Services;
using TaskList.Interfaces.Web.DTOs;

namespace TaskList.Interfaces.Web.Controllers
{
    [ApiController]
    [Route("api/[controller]")]
    public class TaskController : ControllerBase
    {
        private readonly ITaskService _taskService;

        public TaskController(ITaskService taskService)
        {
            _taskService = taskService;
        }

        [HttpGet("projects")]
        public async Task<ActionResult<IEnumerable<ProjectDto>>> GetProjects()
        {
            var projects = await _taskService.GetAllProjectsAsync();
            return Ok(MapToProjectDtos(projects));
        }

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

        [HttpPost("tasks")]
        public async Task<ActionResult<TaskDto>> AddTask([FromBody] AddTaskRequest request)
        {
            var command = new AddTaskCommand(request.ProjectName, request.Description);
            var result = await _taskService.ExecuteCommandAsync(command);

            if (!result.Success)
            {
                return BadRequest(result.Error);
            }

            var projects = await _taskService.GetAllProjectsAsync();
            var project = projects.First(p => p.Name == request.ProjectName);
            var task = project.Tasks.Last();
            return Ok(MapToTaskDto(task));
        }

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

        [HttpGet("tasks/today")]
        public async Task<ActionResult<IEnumerable<TaskDto>>> GetTodayTasks()
        {
            var tasks = await _taskService.GetTasksForTodayAsync();
            return Ok(MapToTaskDtos(tasks));
        }

        [HttpGet("tasks/by-deadline")]
        public async Task<ActionResult<IEnumerable<TaskDto>>> GetTasksByDeadline([FromQuery] DateTime? deadline)
        {
            var tasks = await _taskService.GetTasksByDeadlineAsync(deadline);
            return Ok(MapToTaskDtos(tasks));
        }

        private static IEnumerable<ProjectDto> MapToProjectDtos(IEnumerable<Project> projects)
        {
            return projects.Select(MapToProjectDto);
        }

        private static ProjectDto MapToProjectDto(Project project)
        {
            return new ProjectDto
            {
                Name = project.Name,
                Tasks = project.Tasks.Select(MapToTaskDto).ToList()
            };
        }

        private static IEnumerable<TaskDto> MapToTaskDtos(IEnumerable<ProjectTask> tasks)
        {
            return tasks.Select(MapToTaskDto);
        }

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