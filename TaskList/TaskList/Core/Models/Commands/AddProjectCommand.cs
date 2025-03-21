namespace TaskList.Core.Models.Commands
{
    public class AddProjectCommand : BaseCommand
    {
        public string ProjectName { get; }

        public AddProjectCommand(string projectName) : base("add_project")
        {
            ProjectName = projectName;
        }
    }
}