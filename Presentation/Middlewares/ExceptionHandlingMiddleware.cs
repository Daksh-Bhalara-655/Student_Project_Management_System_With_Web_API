using Microsoft.AspNetCore.Http;
using Microsoft.Extensions.Logging;
using StudentProjectManagementSystem.Application.DTOs;
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Net;
using System.Text.Json;
using System.Threading.Tasks;

namespace StudentProjectManagementSystem.Presentation.Middlewares;

public class ExceptionHandlingMiddleware(RequestDelegate next, ILogger<ExceptionHandlingMiddleware> logger)
{
    private readonly RequestDelegate _next = next;
    private readonly ILogger<ExceptionHandlingMiddleware> _logger = logger;

    public async Task InvokeAsync(HttpContext httpContext)
    {
        try
        {
            // Pass the request to the next middleware in the pipeline
            await _next(httpContext);
        }
        catch (Exception ex)
        {
            _logger.LogError($"Something went wrong: {ex}");
            await HandleExceptionAsync(httpContext, ex);
        }
    }

    private async Task HandleExceptionAsync(HttpContext context, Exception exception)
    {
        context.Response.ContentType = "application/json";
        
        // Common ApiResponse so all errors follow the same structure
        var response = new ApiResponse<object>
        {
            Success = false,
            Data = null
        };

        switch (exception)
        {
            case FluentValidation.ValidationException fvValidationException:
                // 400 Bad Request for FluentValidation errors
                context.Response.StatusCode = (int)HttpStatusCode.BadRequest;
                response.StatusCode = (int)HttpStatusCode.BadRequest;
                response.Message = string.Join("; ", fvValidationException.Errors.Select(e => e.ErrorMessage));
                break;

            case ValidationException validationException:
                // 400 Bad Request
                context.Response.StatusCode = (int)HttpStatusCode.BadRequest;
                response.StatusCode = (int)HttpStatusCode.BadRequest;
                response.Message = validationException.Message;
                break;

            case UnauthorizedAccessException unauthorizedException:
                // 401 Unauthorized
                context.Response.StatusCode = (int)HttpStatusCode.Unauthorized;
                response.StatusCode = (int)HttpStatusCode.Unauthorized;
                response.Message = "You are not authorized to perform this action.";
                break;

            case KeyNotFoundException notFoundException:
                // 404 Not Found
                context.Response.StatusCode = (int)HttpStatusCode.NotFound;
                response.StatusCode = (int)HttpStatusCode.NotFound;
                response.Message = notFoundException.Message;
                break;

            default:
                // 500 Internal Server Error
                context.Response.StatusCode = (int)HttpStatusCode.InternalServerError;
                response.StatusCode = (int)HttpStatusCode.InternalServerError;
                response.Message = "An internal server error occurred: " + exception.Message;
                break;
        }

        // Convert the response object to JSON format
        var result = JsonSerializer.Serialize(response, new JsonSerializerOptions { PropertyNamingPolicy = JsonNamingPolicy.CamelCase });
        
        // Write the JSON response back to the client
        await context.Response.WriteAsync(result);
    }
}
