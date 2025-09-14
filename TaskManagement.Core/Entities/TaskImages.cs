using System;

namespace TaskManagement.Core.Entities
{
    public class TaskImages
    {
        public long Id { get; set; }
        public long TaskId { get; set; }
        public string ImagePath { get; set; } = string.Empty;
        public DateTime CreatedAt { get; set; }
    }
}