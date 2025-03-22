namespace TaskList.Core.Models
{
    /// <summary>
    /// Represents the result of executing a command in the TaskList application.
    /// Contains information about the success or failure of the command execution and any associated data.
    /// </summary>
    public class CommandResult
    {
        /// <summary>
        /// Gets or sets a value indicating whether the command was executed successfully.
        /// </summary>
        public bool Success { get; set; }

        /// <summary>
        /// Gets or sets the error message if the command execution failed.
        /// </summary>
        public string Error { get; set; }

        /// <summary>
        /// Gets or sets the data returned by the command execution.
        /// </summary>
        public object Data { get; set; }
    }
}