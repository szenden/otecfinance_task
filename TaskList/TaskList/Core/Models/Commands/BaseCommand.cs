namespace TaskList.Core.Models.Commands
{
    /// <summary>
    /// Base class for all commands in the TaskList application.
    /// Provides common functionality for command identification and execution.
    /// </summary>
    public abstract class BaseCommand
    {
        /// <summary>
        /// Gets the type of the command.
        /// </summary>
        public string CommandType { get; }

        /// <summary>
        /// Initializes a new instance of the BaseCommand class.
        /// </summary>
        /// <param name="commandType">The type identifier for the command.</param>
        protected BaseCommand(string commandType)
        {
            CommandType = commandType;
        }
    }
}