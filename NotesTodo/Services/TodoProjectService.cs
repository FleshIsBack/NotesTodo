using NotesTodo.Models;
using NotesTodo.Services.Interface;

namespace NotesTodo.Services
{
    public class TodoProjectService : ITodoProjectService
    {
        public Task AddMemberToProjectAsync(int projectId, AddProjectMemberDTO addMemberDto, int userId)
        {
            throw new NotImplementedException();
        }

        public Task<TodoProjectResponseDTO> CreateProjectAsync(CreateTodoProjectDTO createDto, int userId)
        {
            throw new NotImplementedException();
        }

        public Task DeleteProjectAsync(int projectId, int userId)
        {
            throw new NotImplementedException();
        }

        public Task<TodoProjectResponseDTO> GetProjectByIdAsync(int projectId, int userId)
        {
            throw new NotImplementedException();
        }

        public Task<List<TodoProjectResponseDTO>> GetUserProjectsAsync(int userId)
        {
            throw new NotImplementedException();
        }

        public Task RemoveMemberFromProjectAsync(int projectId, int memberUserId, int userId)
        {
            throw new NotImplementedException();
        }

        public Task UpdateMemberRoleAsync(int projectId, UpdateProjectMemberRoleDTO updateRoleDto, int userId)
        {
            throw new NotImplementedException();
        }

        public Task<TodoProjectResponseDTO> UpdateProjectAsync(int projectId, UpdateTodoProjectDTO updateDto, int userId)
        {
            throw new NotImplementedException();
        }

        public Task<List<ProjectMembershipResponseDTO>> GetMembersFromProjectAsync(int id, int userid)
        {
            throw new NotImplementedException();
        }
    }
}
