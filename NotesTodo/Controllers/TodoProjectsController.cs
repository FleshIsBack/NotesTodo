using Microsoft.AspNetCore.Mvc;
using NotesTodo.Models;
using NotesTodo.Services;
using NotesTodo.Services.Interface;
using System.Security.Claims;

[ApiController]
[Route("api/projects")]
public class TodoProjectsController : ControllerBase
{
    private readonly ITodoProjectService _projectService;

    public TodoProjectsController(ITodoProjectService projectService)
    {
        _projectService = projectService;
    }

    [HttpGet("{id:int}")]
    public async Task<IActionResult> GetProjectById(int id)
    {
        var userId = int.Parse(User.FindFirst(ClaimTypes.NameIdentifier)!.Value);
        try
        {
            var result = await _projectService.GetProjectByIdAsync(id, userId);
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

    [HttpGet("user")]
    public async Task<IActionResult> GetUserProjects()
    {
        var userId = int.Parse(User.FindFirst(ClaimTypes.NameIdentifier)!.Value);
        try
        {
            var result = await _projectService.GetUserProjectsAsync(userId);
            return Ok(result);
        }
        catch (KeyNotFoundException) 
        {
            return NotFound();
        }
    }

    [HttpPost]
    public async Task<IActionResult> CreateProject(CreateTodoProjectDTO createDto)
    {
        var userId = int.Parse(User.FindFirst(ClaimTypes.NameIdentifier)!.Value);
        try
        {
            var result = await _projectService.CreateProjectAsync(createDto, userId);
            return CreatedAtAction(nameof(GetProjectById), new { id = result.Id }, result);
        }
        catch (KeyNotFoundException) 
        {
            return NotFound();
        }
    }

    [HttpPut("{id:int}")]
    public async Task<IActionResult> UpdateProject(int id, UpdateTodoProjectDTO updateDto)
    {
        var userId = int.Parse(User.FindFirst(ClaimTypes.NameIdentifier)!.Value);
        try
        {
            var result = await _projectService.UpdateProjectAsync(id, updateDto, userId);
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

    [HttpDelete("{id:int}")]
    public async Task<IActionResult> DeleteProject(int id)
    {
        var userId = int.Parse(User.FindFirst(ClaimTypes.NameIdentifier)!.Value);
        try
        {
            await _projectService.DeleteProjectAsync(id, userId);
            return NoContent();
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

    [HttpPost("{projectId:int}/members")]
    public async Task<IActionResult> AddMemberToProject(int projectId, AddProjectMemberDTO addMemberDto)
    {
        var userId = int.Parse(User.FindFirst(ClaimTypes.NameIdentifier)!.Value);
        try
        {
            await _projectService.AddMemberToProjectAsync(projectId, addMemberDto, userId);
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
        catch (InvalidOperationException e) 
        { 
            return Conflict(e.Message); 
        }
    }

    [HttpPut("{projectId:int}/members")]
    public async Task<IActionResult> UpdateMemberRole(int projectId, UpdateProjectMemberRoleDTO updateRoleDto)
    {
        var userId = int.Parse(User.FindFirst(ClaimTypes.NameIdentifier)!.Value);
        try
        {
            await _projectService.UpdateMemberRoleAsync(projectId, updateRoleDto, userId);
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
        catch (InvalidOperationException e) 
        { 
            return Conflict(e.Message); 
        }
    }
    [HttpGet("{projectId:int}/members")]
    public async Task<IActionResult> GetMembersProject(int id)
    {
        var userId = int.Parse(User.FindFirst(ClaimTypes.NameIdentifier)!.Value);
        try
        {
            var result = await _projectService.GetMembersFromProjectAsync(id, userId);
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
        catch (InvalidOperationException e)
        {
            return Conflict(e.Message);
        }
    }

    [HttpDelete("{projectId:int}/members/{memberUserId:int}")]
    public async Task<IActionResult> RemoveMemberFromProject(int projectId, int memberUserId)
    {
        var userId = int.Parse(User.FindFirst(ClaimTypes.NameIdentifier)!.Value);
        try
        {
            await _projectService.RemoveMemberFromProjectAsync(projectId, memberUserId, userId);
            return NoContent();
        }
        catch (KeyNotFoundException) 
        { 
            return NotFound(); 
        }
        catch (UnauthorizedAccessException) 
        { 
            return Forbid(); 
        }
        catch (InvalidOperationException e) 
        { 
            return Conflict(e.Message); 
        }
    }
}