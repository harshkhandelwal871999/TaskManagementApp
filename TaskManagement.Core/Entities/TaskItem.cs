using Common;
using System;
using System.Collections.Generic;

namespace TaskManagement.Core.Entities
{
    public class TaskItem
    {
        public long Id { get; set; }
        public string Name { get; set; } = null!;
        public string? Description { get; set; }
        public DateTime? Deadline { get; set; }
        public bool IsFavorite { get; set; }
        public StatusEnum Status { get; set; }
        public string? ImagePath { get; set; }
        public DateTime CreatedAt { get; set; } = DateTime.UtcNow;
        public DateTime? UpdatedAt { get; set; } = DateTime.UtcNow;
        public bool IsDeleted { get; set; }
        public List<TaskImages> Images { get; set; } = new List<TaskImages>();
    }
}
