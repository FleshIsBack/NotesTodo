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
        public ICollection<User> AssingedUser { get; set; } = new List<User>();
        public User CreatedByUser { get; set; } = null!;
        public User? CompletedByUser { get; set; } = new User();
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
