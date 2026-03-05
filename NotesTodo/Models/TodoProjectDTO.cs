namespace NotesTodo.Models
{
    public class CreateTodoProjectDTO
    {
        public string Name { get; set; } = string.Empty;
        public string Description { get; set; } = string.Empty;
    }

    public class UpdateTodoProjectDTO
    {
        public string Name { get; set; } = string.Empty;
        public string Description { get; set; } = string.Empty;
    }

    public class TodoProjectResponseDTO
    {
        public int Id { get; set; }
        public string Name { get; set; } = string.Empty;
        public string Description { get; set; } = string.Empty;
        public List<ProjectMembershipResponseDTO> Members { get; set; } = new List<ProjectMembershipResponseDTO>();
        public List<TodoResponseDTO> Todos { get; set; } = new List<TodoResponseDTO>();
    }

    public class ProjectMembershipResponseDTO
    {
        public UserResponseDTO User { get; set; } = null!;
        public Role Role { get; set; }
    }

    public class AddProjectMemberDTO
    {
        public int UserId { get; set; }
        public Role Role { get; set; }
    }

    public class UpdateProjectMemberRoleDTO
    {
        public int UserId { get; set; }
        public Role Role { get; set; }
    }
}