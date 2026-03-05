using NotesTodo.Models;
using NotesTodo.Services.Interface;

namespace NotesTodo.Services;
    public class UserSevice : IUserService
{
    public Task DeleteUserAsync(int userId, string token)
    {
        throw new NotImplementedException();
    }

    public Task<List<UserResponseDTO>> GetAllUsersInProjectAsync()
    {
        throw new NotImplementedException();
    }

    public Task<UserResponseDTO> GetUserByIdAsync(int userId)
    {
        throw new NotImplementedException();
    }

    public Task<AuthResponseDTO> LoginUserAsync(LoginUserDTO loginDto)
    {
        throw new NotImplementedException();
    }

    public Task<UserResponseDTO> RegisterUserAsync(CreateUserDTO registerDto)
    {
        throw new NotImplementedException();
    }

    public Task<UserResponseDTO> UpdateUserAsync(int userId, UpdateUserDTO updateDto)
    {
        throw new NotImplementedException();
    }

    public Task<List<UserResponseDTO>> GetAllUsersInProjectAsync(int id)
    {
        throw new NotImplementedException();
    }
}
