using FluentValidation;
using FluentValidation.AspNetCore;
using Microsoft.EntityFrameworkCore;
using Scalar.AspNetCore;
using StudentProjectManagementSystem.Application.Interfaces.Repositories;
using StudentProjectManagementSystem.Application.Interfaces.Services;
using StudentProjectManagementSystem.Application.Services;
using StudentProjectManagementSystem.Application.Validators;
using StudentProjectManagementSystem.Infrastructure.Data;
using StudentProjectManagementSystem.Infrastructure.Repositories;
using StudentProjectManagementSystem.Presentation.Middlewares;

var builder = WebApplication.CreateBuilder(args);   

// Add services to the container.

builder.Services.AddControllers();
// Learn more about configuring OpenAPI at https://aka.ms/aspnet/openapi
builder.Services.AddOpenApi();

builder.Services.AddDbContext<AppDbContext>(options => options.UseSqlServer(builder.Configuration.GetConnectionString("connectionString")));
builder.Services.AddScoped(typeof(IRepository<>), typeof(Repository<>));
builder.Services.AddScoped<IPermissionService, PermissionService>();
builder.Services.AddScoped<IProjectAllocationService, ProjectAllocationService>();
builder.Services.AddScoped<IProjectService, ProjectService>();
builder.Services.AddScoped<IProjectTaskService, ProjectTaskService>();
builder.Services.AddScoped<IRoleService, RoleService>();
builder.Services.AddScoped<IRolePermissionService, RolePermissionService>();
builder.Services.AddScoped<IUserService, UserService>();
// Register validators in the DI container so they can be resolved and
// invoked manually (do not enable automatic model validation).
builder.Services.AddValidatorsFromAssemblyContaining<PermissionValidators>();



var app = builder.Build();

// Configure the HTTP request pipeline.
app.UseMiddleware<ExceptionHandlingMiddleware>();

if (app.Environment.IsDevelopment())
{
    app.MapOpenApi();
    app.MapScalarApiReference("/");
}

app.UseHttpsRedirection();

app.UseAuthorization();

//Attribute Routing
app.MapControllers();

//Conventional Routing (default pattern)
app.MapControllerRoute(
    name: "default",
    pattern: "{controller}/{action}/{id?}"
);

// Conventional Routing (versioned pattern)
app.MapControllerRoute(
    name: "versioned",
    pattern: "v1/{controller}/{action}/{id?}"
);

app.Run();
