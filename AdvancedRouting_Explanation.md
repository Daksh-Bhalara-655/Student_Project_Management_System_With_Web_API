# Advanced Routing Introduction

> Advanced Routing from scratch examples

---

# Table of Contents

1. What is Routing?
2. Conventional vs Attribute Routing
3. What are Route Constraints?
4. Common Route Constraints
5. What are Nested Routes?
6. Custom Route Naming
7. Version-Compatible Route Structures
8. Summary Table

---

# 1. What is Routing?

Routing is the process of matching an incoming HTTP request (URL) to a specific piece of code (Controller Action).

It helps the application understand:
- Which Controller to execute
- Which Action method to run
- What parameters to pass

---

# 2. Conventional vs Attribute Routing

ASP.NET Core supports two main types of routing.

---

# Conventional Routing

Defined centrally in one place (usually `Program.cs`).
It follows a standard pattern.

```csharp
app.MapControllerRoute(
    name: "default",
    pattern: "{controller}/{action}/{id?}"
);
```

URL Example: `GET /User/GetAllUsers`

---

# Attribute Routing

Defined directly on the Controller and Action methods using attributes.

```csharp
[Route("api/[controller]")]
public class UserController : ControllerBase
{
    [HttpGet("active")]
    public async Task<IActionResult> GetActiveUsers()
    {
        // ...
    }
}
```

URL Example: `GET /api/User/active`

---

# Industry Recommendation

✅ Use **Attribute Routing** for building REST APIs.

---

# 3. What are Route Constraints?

Route Constraints allow you to restrict the type or value of a route parameter.

It tells ASP.NET to match a route **only if** the parameter passes the condition.

---

# Example Without Constraint

```csharp
[HttpGet("{id}")]
public IActionResult GetProjectById([FromRoute] string id)
```

Problem:
- `GET /api/Project/5` → Matches (id = "5")
- `GET /api/Project/abc` → Matches (id = "abc") → Might cause database errors if we expect a number!

---

# Example With Constraint

```csharp
[HttpGet("{id:long}")]
public IActionResult GetProjectById([FromRoute] long id)
```

Now:
- `GET /api/Project/5` → Matches (valid number)
- `GET /api/Project/abc` → Returns **404 Not Found** (because "abc" is not a number)

Benefits:
- Safer code
- Automatic validation before code even runs

---

# 4. Common Route Constraints

You can combine multiple constraints using a colon `:`.

| Constraint Example | Description | Matches | Fails |
| --- | --- | --- | --- |
| `{id:int}` | Must be an integer | `/5` | `/abc` |
| `{id:long}` | Must be a long integer | `/5` | `/abc` |
| `{status:alpha}` | Must contain only letters | `/Active` | `/123` |
| `{projectId:min(1)}` | Number must be >= 1 | `/3` | `/0` |
| `{name:minlength(3)}` | String length >= 3 | `/Raj` | `/Om` |
| `{projectId:long:min(1)}` | **Combined**: long AND >= 1 | `/3` | `/abc` or `/0` |

---

# 5. What are Nested Routes?

Nested routes show a **parent-child relationship** directly in the URL structure.

---

# Example

Tasks belong to a Project.

Instead of writing:
`GET /api/Task/project/5`

We can write a nested route on the Project Controller:
`GET /api/Project/5/tasks`

---

# Nested Route Syntax

```csharp
[HttpGet("{id:long}/tasks")]
public async Task<IActionResult> GetProjectTasks([FromRoute] long id)
{
    // Return tasks belonging to Project ID
}
```

Meaning: "Get the tasks for the Project with this ID".

---

# 6. Custom Route Naming

You can give a specific **Name** to any route.

---

# Example

```csharp
[HttpGet("{id:long}", Name = "GetTaskById")]
public async Task<IActionResult> GetTaskById([FromRoute] long id)
```

---

# Why is Naming Useful?

It helps when generating URLs inside your C# code dynamically.

Instead of hardcoding a string URL like `"/api/Task/5"`, you can do:

```csharp
string url = Url.Link("GetTaskById", new { id = 5 });
```

Commonly used after creating a resource (`POST`), to tell the client where to find the newly created item using `CreatedAtRoute()`.

---

# 7. Version-Compatible Route Structures

When building APIs, requirements change over time. 
If you change an API response, old client applications might break.

Solution: **API Versioning**

---

# Example

You can apply MULTIPLE `[Route]` attributes to a single controller.

```csharp
[Route("api/[controller]")]
[Route("api/v1/[controller]")]
[ApiController]
public class UserController : ControllerBase
```

---

# How it Works

Now the same controller can be accessed via two URLs:
1. `GET /api/User` (Original)
2. `GET /api/v1/User` (Version 1)

When you need to create a breaking change (Version 2):
You create a completely new controller with:
`[Route("api/v2/[controller]")]`

This way, existing clients using `/v1/` are not broken!

---

# 8. Summary Table

| Concept | Description | Syntax Example |
| --- | --- | --- |
| **Route Constraints** | Validates URL parameters | `[HttpGet("{id:long}")]` |
| **Nested Routes** | Parent/Child relationship in URL | `[HttpGet("{id:long}/tasks")]` |
| **Route Naming** | Naming a route to generate URLs | `[HttpGet(Name="GetAll")]` |
| **Route Versioning** | Supporting multiple URL patterns | `[Route("api/v1/[controller]")]` |
