namespace TaskList.Core.Models.Commands
{
    public abstract class BaseCommand
    {
        public string CommandType { get; }

        protected BaseCommand(string commandType)
        {
            CommandType = commandType;
        }
    }
}