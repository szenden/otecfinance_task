namespace TaskList.Core.Models
{
    public class CommandResult
    {
        public bool Success { get; set; }
        public string Error { get; set; }
        public object Data { get; set; }
    }
}