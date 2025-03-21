using System;

namespace TaskList.Core.Models.Commands
{
    public class SetDeadlineCommand : BaseCommand
    {
        public long TaskId { get; }
        public DateTime Deadline { get; }

        public SetDeadlineCommand(long taskId, DateTime deadline) : base("set_deadline")
        {
            TaskId = taskId;
            Deadline = deadline;
        }
    }
}