using TaskList.Core.Models;

namespace TaskList.Core.Processing
{
    /// <summary>
    /// Defines the contract for processing command-line commands in the TaskList application.
    /// Provides methods for parsing and executing user commands.
    /// </summary>
    public interface ICommandProcessor
    {
        /// <summary>
        /// Processes a command line input and executes the corresponding command.
        /// </summary>
        /// <param name="commandLine">The command line string to process.</param>
        /// <returns>A task that represents the asynchronous operation. The task result contains the result of the command execution.</returns>
        Task<CommandResult> ProcessCommandAsync(string commandLine);
    }
}