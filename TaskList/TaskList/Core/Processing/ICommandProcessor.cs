using TaskList.Core.Models;

namespace TaskList.Core.Processing
{
    public interface ICommandProcessor
    {
        Task<CommandResult> ProcessCommandAsync(string commandLine);
    }
}