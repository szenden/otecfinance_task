namespace TaskList.Core.Models.Commands
{
    public class CheckTaskCommand : BaseCommand
    {
        public long TaskId { get; }
        public bool Checked { get; }

        public CheckTaskCommand(long taskId, bool @checked) : base("check_task")
        {
            TaskId = taskId;
            Checked = @checked;
        }
    }
}