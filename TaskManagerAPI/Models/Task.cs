using System.ComponentModel.DataAnnotations;

namespace TaskManagerAPI.Models
{
    public class TaskItem
    {
        public int Id { get; set; }
        
        [Required]
        [StringLength(200)]
        public string Title { get; set; } = string.Empty;
        
        [StringLength(500)]
        public string Description { get; set; } = string.Empty;
        
        public int UserId { get; set; }
        public User? User { get; set; }
        
        public int TaskTypeId { get; set; }
        public TaskType? TaskType { get; set; }
        
        public TaskItemStatus Status { get; set; } = TaskItemStatus.NotStarted;
        
        public DateTime AssignedDate { get; set; } = DateTime.Now;
        public DateTime? DueDate { get; set; }
        
        public int WeekNumber { get; set; }
        public int Year { get; set; }
    }
    
    public enum TaskItemStatus
    {
        NotStarted = 0,
        InProgress = 1,
        Completed = 2
    }
}

namespace TaskManagerAPI.Models
{
    public class TaskType
    {
        public int Id { get; set; }
        
        [Required]
        [StringLength(100)]
        public string Name { get; set; } = string.Empty;
        
        [Range(1, 6)]
        public int DifficultyLevel { get; set; }
        
        [StringLength(200)]
        public string Description { get; set; } = string.Empty;
    }
}
