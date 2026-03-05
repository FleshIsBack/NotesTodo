namespace NotesTodo.Models
{
    public class CreateTodoDTO
    {
        public string Title { get; set; } = string.Empty;
        public string Description { get; set; } = string.Empty;
        public DateTime DateDue { get; set; }
        public List<int> AssignedUserIds { get; set; } = new List<int>();
        public int TodoProjectId { get; set; }
        public Criticality Criticality { get; set; }
    }

    public class UpdateTodoDTO
    {
        public string Title { get; set; } = string.Empty;
        public string Description { get; set; } = string.Empty;
        public TodoStatus  Status { get; set; }
        public DateTime DateDue { get; set; }
        public List<int> AssignedUserIds { get; set; } = new List<int>();

        public Criticality Criticality { get; set; }
    }

    public class TodoResponseDTO
    {
        public int Id { get; set; }
        public string Title { get; set; } = string.Empty;
        public string Description { get; set; } = string.Empty;
        public TodoStatus Status { get; set; }
        public DateTime CreatedAt { get; set; }
        public DateTime DateDue { get; set; }
        public List<UserResponseDTO> AssignedUsers { get; set; } = new List<UserResponseDTO>();
        public int CreatedByUserId { get; set; }
        public int? CompletedByUserId { get; set; }
        public Criticality Criticality { get; set; }
        public List<SubTaskResponseDTO> SubTasks { get; set; } = new List<SubTaskResponseDTO>();
    }

    public class CreateSubTaskDTO
    {
        public string Title { get; set; } = string.Empty;
        public string Description { get; set; } = string.Empty;
        public List<int> AssignedUserIds { get; set; } = new List<int>();
        public Criticality Criticality { get; set; }
        public int TodoId { get; set; }
    }

    public class UpdateSubTaskDTO
    {
        public string Title { get; set; } = string.Empty;
        public string Description { get; set; } = string.Empty;
        public TodoStatus  Status { get; set; }
        public List<int> AssignedUserIds { get; set; } = new List<int>();
        public Criticality Criticality { get; set; }

    }

    public class SubTaskResponseDTO
    {
        public int Id { get; set; }
        public string Title { get; set; } = string.Empty;
        public string Description { get; set; } = string.Empty;
        public TodoStatus  Status { get; set; }
        public Criticality Criticality { get; set; }
        public List<UserResponseDTO> AssignedUsers { get; set; } = new List<UserResponseDTO>();
        public int TodoId { get; set; }
    }
}