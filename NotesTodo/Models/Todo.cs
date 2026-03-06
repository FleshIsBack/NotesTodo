namespace NotesTodo.Models
{
    public class Todo
    {
        public int Id { get; set; }
        public string Title { get; set; } = string.Empty;
        public string Description { get; set; } = string.Empty;
        public ICollection<SubTask> SubTask { get; set; } = new List<SubTask>();
        public TodoStatus Status { get; set; }
        public Criticality Criticality { get; set; }
        public DateTime CreatedAt { get; set; } = DateTime.UtcNow;
        public DateTime DateDue { get; set; }
        public ICollection<User> AssignedUsers { get; set; } = new List<User>();
        public int CreatedByUserId { get; set; }
        public User CreatedByUser { get; set; } = null!;
        public int? CompletedByUserId { get; set; }
        public User? CompletedByUser { get; set; }
        public int TodoProjectId { get; set; }
        public TodoProject TodoProject { get; set; } = null!;
    }

    public class SubTask
    {
        public int Id { get; set; }
        public string Title { get; set; } = string.Empty;
        public string Description { get; set; } = string.Empty;
        public TodoStatus Status { get; set; }
        public ICollection<User> AssingedUsers { get; set; } = new List<User>();
        public Criticality Criticality { get; set; }
        public DateTime CreatedAt { get; set; } = DateTime.UtcNow;
        public int TodoId { get; set; }
        public Todo Todo { get; set; } = null!;
    }

    public enum TodoStatus
    {
        NotStarted = 0,
        InProgress =1,
        Completed =2,
        OnHold = 3,
    }
    public enum Criticality
    {
        Low = 0,
        Medium = 1,
        High = 2,
    }

}
