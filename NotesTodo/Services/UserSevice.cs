using Microsoft.EntityFrameworkCore;
using Microsoft.IdentityModel.Tokens;
using NotesTodo.DAL;
using NotesTodo.Models;
using NotesTodo.Services.Interface;
using System.IdentityModel.Tokens.Jwt;
using System.Security.Claims;
using System.Text;

namespace NotesTodo.Services;
    public class UserSevice : IUserService
    {
    private readonly TodoDb db = null!;
    private readonly IConfiguration _configuration = null!;
    public UserSevice(TodoDb context, IConfiguration configuration) 
    {
        db = context;
        _configuration = configuration;
    }
    public async Task DeleteUserAsync(int userId)
    {
        var user = await db.Users.FirstOrDefaultAsync(u => u.Id == userId);

        if (user == null)
            throw new KeyNotFoundException("User not found");

        db.Users.Remove(user);
        await db.SaveChangesAsync();
    }

    public Task<List<UserResponseDTO>> GetAllUsersInProjectAsync()
    {
        var users = db.Users.Select(u => new UserResponseDTO {
            Id = u.Id,
            Name = u.Name,
            Email = u.Email
        }).ToList();

        return Task.FromResult(users);
    }

    public async Task<UserResponseDTO> GetUserByIdAsync(int userId)
    {
        var user = await db.Users.Where(u => u.Id == userId)
            .Select(u => new UserResponseDTO {
                Id = u.Id,
                Name = u.Name,
                Email = u.Email
            }).FirstOrDefaultAsync();

        if (user == null)
            throw new KeyNotFoundException("User not found");

        return user;
    }

    public async Task<AuthResponseDTO> LoginUserAsync(LoginUserDTO loginDto)
    {
        if (!loginDto.Email.Contains("@")) throw new Exception("Invalid Email");

        var user = await db.Users.FirstOrDefaultAsync(u => u.Email == loginDto.Email)
            ?? throw new KeyNotFoundException("User not found");

        if (!BCrypt.Net.BCrypt.Verify(loginDto.Password, user.Password))
            throw new UnauthorizedAccessException("Invalid Login Credentials");

        var token = GenerateToken(user);

        return new AuthResponseDTO
        {
            Token = token,
            User = new UserResponseDTO
            {
                Id = user.Id,
                Email = user.Email,
                Name = user.Name
            }
        };
    }

    public async Task<UserResponseDTO> RegisterUserAsync(CreateUserDTO registerDto)
    {
        if (string.IsNullOrWhiteSpace(registerDto.Name))
            throw new ArgumentException("Name is required");
        if (string.IsNullOrWhiteSpace(registerDto.Email))
            throw new ArgumentException("Email is required");
        if (string.IsNullOrWhiteSpace(registerDto.Password))
            throw new ArgumentException("Password is required");

        var existingUser = await db.Users.FirstOrDefaultAsync(u => u.Email == registerDto.Email);
        if (existingUser != null)
            throw new InvalidOperationException("Email already in use");

        var newUser = new User
        {
            Name = registerDto.Name,
            Email = registerDto.Email,
            Password = BCrypt.Net.BCrypt.HashPassword(registerDto.Password)
        };

        db.Users.Add(newUser);
        await db.SaveChangesAsync();

        return new UserResponseDTO
        {
            Id = newUser.Id,
            Name = newUser.Name,
            Email = newUser.Email
        };
    }
    public Task<UserResponseDTO> UpdateUserAsync(int userId, UpdateUserDTO updateDto)
    {
        throw new NotImplementedException();
    }

    public Task<List<UserResponseDTO>> GetAllUsersInProjectAsync(int id)
    {
        throw new NotImplementedException();
    }

    private string GenerateToken(User user)
    {
        var key = new SymmetricSecurityKey(
            Encoding.UTF8.GetBytes(_configuration["Jwt:Secret"]!));

        var credentials = new SigningCredentials(key, SecurityAlgorithms.HmacSha256);

        var claims = new[]
        {
        new Claim(ClaimTypes.NameIdentifier, user.Id.ToString()),
        new Claim(ClaimTypes.Email, user.Email),
        new Claim(ClaimTypes.Name, user.Name)
    };

        var token = new JwtSecurityToken(
            issuer: _configuration["Jwt:Issuer"],
            audience: _configuration["Jwt:Audience"],
            claims: claims,
            expires: DateTime.UtcNow.AddHours(24),
            signingCredentials: credentials
        );

        return new JwtSecurityTokenHandler().WriteToken(token);
    }

}
