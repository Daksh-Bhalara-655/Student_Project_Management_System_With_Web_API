using Microsoft.AspNetCore.Mvc;
using StudentProjectManagementSystem.Application.Common;
using StudentProjectManagementSystem.Application.DTOs;

namespace StudentProjectManagementSystem.Presentation.Controllers;

[ApiController]
public abstract class ApiControllerBase : ControllerBase
{
    protected IActionResult ToActionResult<T>(ServiceResult<T> result)
    {
        return StatusCode(result.StatusCode, new ApiResponse<T?>
        {
            Success = result.Success,
            Message = result.Message,
            StatusCode = result.StatusCode,
            Data = result.Data
        });
    }
}
