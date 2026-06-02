# Exception Handling & API Responses

> Standardizing API responses and handling errors globally

---

# Table of Contents

1. Problems with Normal API Responses
2. What is an API Response Wrapper?
3. Implementing `ApiResponse<T>`
4. HTTP Status Codes Explained
5. What is Middleware?
6. Why Centralized Exception Handling?
7. Implementing Exception Handling Middleware
8. Summary

---

# 1. Problems with Normal API Responses

When building an API, different developers might return data in different ways.

Developer 1 returns just the data:
```json
{
  "name": "Amit",
  "age": 20
}
```

Developer 2 returns an object with a message:
```json
{
  "message": "Student found",
  "data": { "name": "Raj" }
}
```

**The Problem:** The frontend developer (React, Angular, Mobile App) gets confused because the structure changes on every API call!

---

# 2. What is an API Response Wrapper?

An API Response Wrapper is a **standard, consistent format** for every single API response.

Whether the request is a **Success** or a **Failure**, the structure stays exactly the same.

A good wrapper contains:
- `Success` (true/false)
- `Message` (e.g., "User created successfully" or "User not found")
- `StatusCode` (e.g., 200, 404, 500)
- `Data` (The actual payload, like the user object or list of users)

---

# 3. Implementing `ApiResponse<T>`

We create a Generic Class in C#. `<T>` means it can hold any type of data (a single User, a List of Projects, or just a string).

```csharp
public class ApiResponse<T>
{
    public bool Success { get; set; }
    public string Message { get; set; }
    public int StatusCode { get; set; }
    public T Data { get; set; }
}
```

---

# Example Usage in Controller

Now, every API returns this exact structure.

```csharp
[HttpGet("{id}")]
public async Task<IActionResult> GetUserById(long id)
{
    var user = await _db.Users.FindAsync(id);
    
    if (user == null)
    {
        return NotFound(new ApiResponse<object> 
        { 
            Success = false, 
            Message = "User not found", 
            StatusCode = 404, 
            Data = null 
        });
    }

    return Ok(new ApiResponse<User> 
    { 
        Success = true, 
        Message = "User retrieved successfully", 
        StatusCode = 200, 
        Data = user 
    });
}
```

---

# 4. HTTP Status Codes Explained

Status codes tell the client what happened without them needing to read the message.

| Code | Meaning | When to use |
| --- | --- | --- |
| **200** | OK | Request was successful (GET, PUT, DELETE). |
| **201** | Created | A new resource was successfully created (POST). |
| **400** | Bad Request | The client sent invalid data (e.g., missing fields). |
| **401** | Unauthorized | User is not logged in or token is invalid. |
| **403** | Forbidden | User is logged in, but doesn't have permission. |
| **404** | Not Found | The requested resource does not exist. |
| **500** | Internal Server Error | Code crashed or database connection failed. |

---

# 5. What is Middleware?

Middleware is software that is assembled into an application pipeline to handle requests and responses.

Think of it like a **security guard or receptionist** at the front door of a building.
Every request coming IN goes through the middleware.
Every response going OUT goes through the middleware.

---

# 6. Why Centralized Exception Handling?

### The Old Way (Try-Catch everywhere)
If you put `try-catch` blocks in every single controller action, your code becomes messy, repetitive, and hard to maintain.

```csharp
try {
   // do something
} catch (Exception ex) {
   return StatusCode(500, ex.Message);
}
```

### The New Way (Centralized Middleware)
We remove `try-catch` blocks from our controllers. If an error happens *anywhere* in the application, it crashes up to the middleware. The middleware catches it, formats it nicely using our `ApiResponse`, and sends it to the user.

---

# 7. Implementing Exception Handling Middleware

Here is how we catch errors globally:

```csharp
public class ExceptionHandlingMiddleware
{
    private readonly RequestDelegate _next;

    public ExceptionHandlingMiddleware(RequestDelegate next)
    {
        _next = next; // The next step in the pipeline
    }

    public async Task Invoke(HttpContext context)
    {
        try
        {
            // Try to process the request normally
            await _next(context);
        }
        catch (Exception ex)
        {
            // If ANY error happens ANYWHERE, it lands here!
            await HandleExceptionAsync(context, ex);
        }
    }

    private static Task HandleExceptionAsync(HttpContext context, Exception exception)
    {
        context.Response.ContentType = "application/json";

        // Default to 500 Server Error
        int statusCode = 500;
        string message = "An unexpected error occurred.";

        // We can check specific error types!
        switch (exception)
        {
            case UnauthorizedAccessException:
                statusCode = 401;
                message = "You are not authorized to perform this action.";
                break;
            case KeyNotFoundException:
                statusCode = 404;
                message = "The requested resource was not found.";
                break;
            case ValidationException:
                statusCode = 400;
                message = exception.Message; // E.g., "Email is required"
                break;
        }

        context.Response.StatusCode = statusCode;

        // Return our standard ApiResponse wrapper!
        var response = new ApiResponse<object>
        {
            Success = false,
            Message = message,
            StatusCode = statusCode,
            Data = exception.Message // (Only show this in development)
        };

        return context.Response.WriteAsync(JsonSerializer.Serialize(response));
    }
}
```

### Registration in `Program.cs`
We must tell the application to use this middleware at the very top of the pipeline:

```csharp
app.UseMiddleware<ExceptionHandlingMiddleware>();
```

---

# 8. Summary

1. **`ApiResponse<T>`** ensures that frontend developers always receive the same JSON structure.
2. **Proper HTTP Status Codes** (200, 400, 404, 500) tell the client exactly what type of success or error occurred.
3. **Exception Handling Middleware** acts as a global safety net, catching crashes anywhere in the app and returning a clean, formatted JSON error instead of an ugly HTML crash page.
