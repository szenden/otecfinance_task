namespace TaskList.Core.Models.Commands
{
    public class AddTaskCommand : BaseCommand
    {
        public string ProjectName { get; }
        public string Description { get; }

        public AddTaskCommand(string projectName, string description) : base("add_task")
        {
            ProjectName = projectName;
            Description = description;
        }
    }
}