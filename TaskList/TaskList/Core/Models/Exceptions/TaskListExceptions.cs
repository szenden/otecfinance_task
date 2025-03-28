using System;

namespace TaskList.Core.Models.Exceptions
{
    public class TaskListException : Exception
    {
        public TaskListException(string message) : base(message) { }
        public TaskListException(string message, Exception innerException) : base(message, innerException) { }
    }

    public class ProjectNotFoundException : TaskListException
    {
        public ProjectNotFoundException(long projectId)
            : base($"Project with ID {projectId} not found.") { }
    }

    public class TaskNotFoundException : TaskListException
    {
        public TaskNotFoundException(long taskId)
            : base($"Task with ID {taskId} not found.") { }
    }

    public class ProjectAlreadyExistsException : TaskListException
    {
        public ProjectAlreadyExistsException(string projectName)
            : base($"Project '{projectName}' already exists.") { }
    }

    public class InvalidTaskOperationException : TaskListException
    {
        public InvalidTaskOperationException(string message) : base(message) { }
    }

    public class ValidationException : TaskListException
    {
        public ValidationException(string message) : base(message) { }
    }
}