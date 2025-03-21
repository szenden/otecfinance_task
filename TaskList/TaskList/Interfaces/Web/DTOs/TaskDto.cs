using System;

namespace TaskList.Interfaces.Web.DTOs
{
    public class TaskDto
    {
        public long Id { get; set; }
        public string Description { get; set; }
        public bool Done { get; set; }
        public DateTime? Deadline { get; set; }
        public string ProjectName { get; set; }
    }
}