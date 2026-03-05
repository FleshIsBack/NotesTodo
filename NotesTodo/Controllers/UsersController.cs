using Microsoft.AspNetCore.Mvc;
using NotesTodo.Models;
using NotesTodo.Services.Interface;
using System.Security.Claims;

namespace NotesTodo.Controllers
{
    [Route("api/users")]
    [ApiController]
    public class UsersController : ControllerBase
    {
        private readonly IUserService _userService;

        public UsersController(IUserService userService)
        {
            _userService = userService;
        }

        [HttpPost("register")]
        public async Task<IActionResult> Register(CreateUserDTO registerDto)
        {
            try
            {
                var result = await _userService.RegisterUserAsync(registerDto);
                return CreatedAtAction(nameof(GetUserById), new { id = result.Id }, result);
            }
            catch (InvalidOperationException e)
            {
                return Conflict(e.Message);
            }
            catch (ArgumentException e)
            {
                return BadRequest(e.Message);
            }
        }

        [HttpPost("login")]
        public async Task<IActionResult> Login(LoginUserDTO loginDto)
        {
            try
            {
                var result = await _userService.LoginUserAsync(loginDto);
                return Ok(result);
            }
            catch (KeyNotFoundException e)
            {
                return NotFound(e.Message);
            }
            catch (UnauthorizedAccessException e)
            {
                return Unauthorized(e.Message);
            }
        }

        [HttpGet("{id:int}")]
        public async Task<IActionResult> GetUserById(int id)
        {
            try
            {
                var result = await _userService.GetUserByIdAsync(id);
                return Ok(result);
            }
            catch (KeyNotFoundException e)
            {
                return NotFound(e.Message);
            }
        }

        [HttpPut("{id:int}")]
        public async Task<IActionResult> UpdateUser(int id, UpdateUserDTO updateDto)
        {
            try
            {
                var result = await _userService.UpdateUserAsync(id, updateDto);
                return Ok(result);
            }
            catch (KeyNotFoundException e)
            {
                return NotFound(e.Message);
            }
            catch (InvalidOperationException e)
            {
                return Conflict(e.Message);
            }
            catch (ArgumentException e)
            {
                return BadRequest(e.Message);
            }
        }

        [HttpDelete("{id:int}")]
        public async Task<IActionResult> DeleteUser(int id)
        {
            var userId = int.Parse(User.FindFirst(ClaimTypes.NameIdentifier)!.Value);
            try
            {
                await _userService.DeleteUserAsync(id, userId.ToString());
                return NoContent();
            }
            catch (KeyNotFoundException e)
            {
                return NotFound(e.Message);
            }
            catch (UnauthorizedAccessException)
            {
                return Forbid();
            }
        }
    }
}