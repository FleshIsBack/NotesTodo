using NotesTodo.DAL;
using NotesTodo.Models;
using NotesTodo.Services.Interface;

namespace NotesTodo.Services
{
    public class TodoService : ITodoService
    {
        private TodoDb db = null!;

        public TodoService(TodoDb db)
        {
            this.db = db;
        }

        public Task<AddProjectMemberDTO> AddMemberToTodoAsync(int todoId, AddProjectMemberDTO addMemberDto, int userId)
        {

            throw new NotImplementedException();
        }

        public Task<SubTaskResponseDTO> CreateSubTaskAsync(CreateSubTaskDTO createDto, int userId)
        {
            throw new NotImplementedException();
        }

        public Task<TodoResponseDTO> CreateTodoAsync(CreateTodoDTO createDto, int userId)
        {
            throw new NotImplementedException();
        }

        public Task DeleteMemberFromTodoAsync(int todoId, int memberUserId, int userId)
        {
            throw new NotImplementedException();
        }

        public Task DeleteSubTaskAsync(int subTaskId, int userId)
        {
            throw new NotImplementedException();
        }

        public Task DeleteTodoAsync(int todoId, int userId)
        {
            throw new NotImplementedException();
        }

        public Task<List<ProjectMembershipResponseDTO>> GetAssingedMembers(int todoId, int userid)
        {
            throw new NotImplementedException();
        }

        public Task<SubTaskResponseDTO> GetSubTaskByIdAsync(int subTaskId, int userId)
        {
            throw new NotImplementedException();
        }

        public Task<List<SubTaskResponseDTO>> GetSubTasksByTodoIdAsync(int todoId, int userId)
        {
            throw new NotImplementedException();
        }

        public Task<TodoResponseDTO> GetTodoByIdAsync(int todoId, int userId)
        {
            throw new NotImplementedException();
        }

        public Task<List<TodoResponseDTO>> GetTodosByProjectIdAsync(int projectId, int userId)
        {
            throw new NotImplementedException();
        }

        public Task RemoveMemberFromTodoAsync(int todoId, int memberUserId, int userId)
        {
            throw new NotImplementedException();
        }

        public Task<SubTaskResponseDTO> UpdateSubTaskAsync(int subTaskId, UpdateSubTaskDTO updateDto, int userId)
        {
            throw new NotImplementedException();
        }

        public Task<TodoResponseDTO> UpdateTodoAsync(int todoId, UpdateTodoDTO updateDto, int userId)
        {
            throw new NotImplementedException();
        }
    }
}
