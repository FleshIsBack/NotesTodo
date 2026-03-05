using System.Data;

namespace NotesTodo.Models
{
    public class TodoProject
    {
        public int Id { get; set; }
        public string Name { get; set; } = string.Empty;
        public string Description { get; set; } = string.Empty;
        public ICollection<ProjectMembership> Members { get; set; } = new List<ProjectMembership>();
        public ICollection<Todo> Todos { get; set; } = new List<Todo>();
    }
    public class ProjectMembership
    {
        public int UserId { get; set; }
        public User User { get; set; } = new User();
        public TodoProject TodoProject { get; set; } = new TodoProject();
        public Role Role { get; set; }
    }

    public enum Role
    {         
        Owner = 0,
        Admin = 1,
        Member = 2,
    }
}
