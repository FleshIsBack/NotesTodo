namespace NotesTodo.Models
{
    public class User
    {
        public int Id { get; set; } 
        public string Name { get; set; } = string.Empty;
        public string Email { get; set; } = string.Empty;
        public string Password { get; set; } = string.Empty;
        public ICollection<ProjectMembership> ProjectMemberships { get; set; } = new List<ProjectMembership>();
        public ICollection<Todo> AssignedTodos { get; set; } = new List<Todo>();
        public ICollection<SubTask> AssignedSubTasks { get; set; } = new List<SubTask>();
    }
}
