using NotesTodo.Models;

namespace NotesTodo.Services.Interface
{
    public interface ITodoProjectService
    {
            Task<TodoProjectResponseDTO> CreateProjectAsync(CreateTodoProjectDTO createDto, int userId);
            Task<TodoProjectResponseDTO> GetProjectByIdAsync(int projectId, int userId);
            Task<List<TodoProjectResponseDTO>> GetUserProjectsAsync(int userId);
            Task<TodoProjectResponseDTO> UpdateProjectAsync(int projectId, UpdateTodoProjectDTO updateDto, int userId);
            Task DeleteProjectAsync(int projectId, int userId);
            Task AddMemberToProjectAsync(int projectId, AddProjectMemberDTO addMemberDto, int userId);
            Task UpdateMemberRoleAsync(int projectId, UpdateProjectMemberRoleDTO updateRoleDto, int userId);
            Task RemoveMemberFromProjectAsync(int projectId, int memberUserId, int userId);
            Task<List<ProjectMembershipResponseDTO>> GetMembersFromProjectAsync(int id, int userid);
    }
}
