using NotesTodo.Models;

namespace NotesTodo.Services.Interface
{
    public interface IUserService
    {
            Task<UserResponseDTO> RegisterUserAsync(CreateUserDTO registerDto);
            Task<AuthResponseDTO> LoginUserAsync(LoginUserDTO loginDto);
            Task<UserResponseDTO> GetUserByIdAsync(int userId);
            Task<List<UserResponseDTO>> GetAllUsersInProjectAsync(int id);
            Task<UserResponseDTO> UpdateUserAsync(int userId, UpdateUserDTO updateDto);
            Task DeleteUserAsync(int userId, string token);
    }
}
