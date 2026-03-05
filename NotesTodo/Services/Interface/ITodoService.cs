using NotesTodo.Models;

namespace NotesTodo.Services.Interface
{
    public interface ITodoService
    {
        Task<TodoResponseDTO> CreateTodoAsync(CreateTodoDTO createDto, int userId);
        Task<TodoResponseDTO> GetTodoByIdAsync(int todoId, int userId);
        Task<List<TodoResponseDTO>> GetTodosByProjectIdAsync(int projectId, int userId);
        Task<TodoResponseDTO> UpdateTodoAsync(int todoId, UpdateTodoDTO updateDto, int userId);
        Task DeleteTodoAsync(int todoId, int userId);
        Task<SubTaskResponseDTO> CreateSubTaskAsync(CreateSubTaskDTO createDto, int userId);
        Task<SubTaskResponseDTO> GetSubTaskByIdAsync(int subTaskId, int userId);
        Task<List<SubTaskResponseDTO>> GetSubTasksByTodoIdAsync(int todoId, int userId);
        Task<SubTaskResponseDTO> UpdateSubTaskAsync(int subTaskId, UpdateSubTaskDTO updateDto, int userId);
        Task DeleteSubTaskAsync(int subTaskId, int userId);
        Task<AddProjectMemberDTO> AddMemberToTodoAsync(int todoId, AddProjectMemberDTO addMemberDto, int userId);
        Task RemoveMemberFromTodoAsync(int todoId, int memberUserId, int userId);
        Task DeleteMemberFromTodoAsync(int todoId, int memberUserId, int userId);
        Task<List<ProjectMembershipResponseDTO>> GetAssingedMembers(int todoId, int userid);

    }
}
