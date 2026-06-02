using Microsoft.AspNetCore.Mvc;
using StudentProjectManagementSystem.Application.DTOs;
using StudentProjectManagementSystem.Application.Interfaces.Services;

namespace StudentProjectManagementSystem.Presentation.Controllers;

[Route("api/[controller]")]
public class UserController(IUserService userService) : ApiControllerBase
{
    private readonly IUserService _userService = userService;

    [HttpGet(Name = "GetAllUsers")]
    public async Task<IActionResult> GetAllUsers(
        [FromQuery] UserFilterDto filters,
        [FromHeader(Name = "Accept-Language")] string? language = null)
    {
        return ToActionResult(await _userService.GetAllAsync(filters));
    }

    [HttpGet("{id:long}", Name = "GetUserById")]
    public async Task<IActionResult> GetUserById([FromRoute] long id)
    {
        return ToActionResult(await _userService.GetByIdAsync(id));
    }

    [HttpPost(Name = "CreateUser")]
    public async Task<IActionResult> CreateUser(
        [FromForm] CreateUserDto user,
        [FromHeader(Name = "X-Correlation-Id")] string? correlationId = null)
    {
        return ToActionResult(await _userService.CreateAsync(user));
    }

    [HttpPut("{id:long}", Name = "UpdateUser")]
    public async Task<IActionResult> UpdateUser(
        [FromRoute] long id,
        [FromForm] UpdateUserDto user,
        [FromHeader(Name = "X-Correlation-Id")] string? correlationId = null)
    {
        return ToActionResult(await _userService.UpdateAsync(id, user));
    }

    [HttpDelete("{id:long}", Name = "DeleteUser")]
    public async Task<IActionResult> DeleteUser(
        [FromRoute] long id,
        [FromHeader(Name = "X-Correlation-Id")] string? correlationId = null)
    {
        return ToActionResult(await _userService.DeleteAsync(id));
    }

    [HttpGet("search", Name = "SearchUsers")]
    public async Task<IActionResult> SearchUsersByName([FromQuery] string name)
    {
        return ToActionResult(await _userService.SearchByNameAsync(name));
    }

    [HttpGet("active", Name = "GetActiveUsers")]
    public async Task<IActionResult> GetActiveUsers(
        [FromHeader(Name = "Accept-Language")] string? language = null)
    {
        return ToActionResult(await _userService.GetActiveAsync());
    }
}
