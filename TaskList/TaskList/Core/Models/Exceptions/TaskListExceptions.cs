using System;


/// <summary>
/// Contains custom exceptions used throughout the TaskList application
/// </summary>
namespace TaskList.Core.Models.Exceptions
{
    /// <summary>
    /// Base exception class for all TaskList-specific exceptions
    /// </summary>
    public class TaskListException : Exception
    {
        public TaskListException(string message) : base(message) { }
        public TaskListException(string message, Exception innerException) : base(message, innerException) { }
    }

    /// <summary>
    /// Thrown when attempting to access a project that does not exist
    /// </summary>
    public class ProjectNotFoundException : TaskListException
    {
        public ProjectNotFoundException(long projectId)
            : base($"Project with ID {projectId} not found.") { }
    }

    /// <summary>
    /// Thrown when attempting to access a task that does not exist
    /// </summary>
    public class TaskNotFoundException : TaskListException
    {
        public TaskNotFoundException(long taskId)
            : base($"Task with ID {taskId} not found.") { }
    }

    /// <summary>
    /// Thrown when attempting to create a project with a name that already exists
    /// </summary>
    public class ProjectAlreadyExistsException : TaskListException
    {
        public ProjectAlreadyExistsException(string projectName)
            : base($"Project '{projectName}' already exists.") { }
    }

    /// <summary>
    /// Thrown when an invalid operation is attempted on a task
    /// </summary>
    public class InvalidTaskOperationException : TaskListException
    {
        public InvalidTaskOperationException(string message) : base(message) { }
    }

    /// <summary>
    /// Thrown when validation fails for an entity or operation
    /// </summary>
    public class ValidationException : TaskListException
    {
        public ValidationException(string message) : base(message) { }
    }
}