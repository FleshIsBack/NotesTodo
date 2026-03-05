using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using NotesTodo.Models;
using NotesTodo.Services.Interface;
using System.Runtime.InteropServices;

namespace NotesTodo.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class TodoController : ControllerBase
    {
        private ITodoService _todoService = null!;
        [HttpGet("{id:int}")]
        public async Task<IActionResult> GetTodoById(int id, int userid)
        {
            try
            {
                var result = await _todoService.GetTodoByIdAsync(id, userid);
                return Ok(result);
            }
            catch (KeyNotFoundException)
            {
                return NotFound();
            }
             catch (UnauthorizedAccessException)
            {
                return Forbid();
            }
        }
        [Authorize]
        [HttpGet("getTodosByProjectId/${projectId:int}")]
        public async Task<IActionResult> GetTodosByProjectId(int projectId, int userid)
        {
            try
            {
                var result = await _todoService.GetTodosByProjectIdAsync(projectId, userid);
                return Ok(result);
            }
            catch (KeyNotFoundException)
            {
                return NotFound();
            }
             catch (UnauthorizedAccessException)
            {
                return Forbid();
            }
        }
        [HttpPost]
        public async Task<IActionResult> CreateTodo(CreateTodoDTO createDto, int userid)
        {
            try
            {
                var result = await _todoService.CreateTodoAsync(createDto, userid);
                return Ok(result);
            }
            catch(KeyNotFoundException)
            {
                return NotFound();
            }
             catch (UnauthorizedAccessException)
            {
                return Forbid();
            }
        }
        [HttpPut("{id:int}")]
        public async Task<IActionResult> UpdateTodo(int id, UpdateTodoDTO updateDto, int userid)
        {
            try
            {
                var result = await _todoService.UpdateTodoAsync(id, updateDto, userid);
                return Ok(result);
            }
            catch(KeyNotFoundException)
            {
                return NotFound();
            }
             catch (UnauthorizedAccessException)
            {
                return Forbid();
            }
        }
        [HttpDelete("{id:int}")]
        public async Task<IActionResult> DeleteTodo(int id, int userid)
        {
            try
            {
                await _todoService.DeleteTodoAsync(id, userid);
                return NoContent();
            }
            catch(KeyNotFoundException)
            {
                return NotFound();
            }
             catch (UnauthorizedAccessException)
            {
                return Forbid();
            }

        }

        [HttpPost("createSubTask")]
        public async Task<IActionResult> CreateSubTask(CreateSubTaskDTO createDto, int userid)
        {
            try
            {
                var result = await _todoService.CreateSubTaskAsync(createDto, userid);
                return Ok(result);
            }
            catch(KeyNotFoundException)
            {
                return NotFound();
            }
             catch (UnauthorizedAccessException)
            {
                return Forbid();
            }
        }
        [HttpGet("getSubTaskById/${id:int}")]
        public async Task<IActionResult> GetSubTaskById(int id, int userid)
        {
            try
            {
                var result = await _todoService.GetSubTaskByIdAsync(id, userid);
                return Ok(result);
            }
            catch (KeyNotFoundException)
            {
                return NotFound();
            }
             catch (UnauthorizedAccessException)
            {
                return Forbid();
            }
        }

        [HttpGet("getSubTasksByTodoId/${todoId:int}")]
        public async Task<IActionResult> GetSubTasksByTodoId(int todoId, int userid)
        {
            try
            {
                var result = await _todoService.GetSubTasksByTodoIdAsync(todoId, userid);
                return Ok(result);
            }
            catch (KeyNotFoundException)
            {
                return NotFound();
            }
             catch (UnauthorizedAccessException)
            {
                return Forbid();
            }
        }

        [HttpPut("updateSubTask/${id:int}")]
        public async Task<IActionResult> UpdateSubTask(int id, UpdateSubTaskDTO updateDto, int userid)
        {
            try
            {
                var result = await _todoService.UpdateSubTaskAsync(id, updateDto, userid);
                return Ok(result);
            }
            catch(KeyNotFoundException)
            {
                return NotFound();
            }
             catch (UnauthorizedAccessException)
            {
                return Forbid();
            }
        }

        [HttpDelete("deleteSubTask/${id:int}")]
        public async Task<IActionResult> DeleteSubTask(int id, int userid)
        {
            try
            {
                await _todoService.DeleteSubTaskAsync(id, userid);
                return Ok();
            }
            catch (KeyNotFoundException)
            {
                return NotFound();
            }
            catch (UnauthorizedAccessException)
            {
                return Forbid();
            }
        }
    }   
}
