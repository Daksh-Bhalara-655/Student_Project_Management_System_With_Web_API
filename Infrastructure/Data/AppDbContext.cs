using Microsoft.EntityFrameworkCore;
using StudentProjectManagementSystem.Domain.Entities;

namespace StudentProjectManagementSystem.Infrastructure.Data;

public class AppDbContext(
    DbContextOptions<AppDbContext> options
) : DbContext(options)
{

    // =========================
    // DbSets
    // =========================

    public DbSet<User> Users { get; set; }

    public DbSet<Role> Roles { get; set; }

    public DbSet<Permission> Permissions { get; set; }

    public DbSet<RolePermission> RolePermissions { get; set; }

    public DbSet<Project> Projects { get; set; }

    public DbSet<ProjectAllocation> ProjectAllocations { get; set; }

    public DbSet<ProjectTask> Tasks { get; set; }

    protected override void OnModelCreating(ModelBuilder modelBuilder)
    {
        base.OnModelCreating(modelBuilder);

        // =========================================================
        // USERS
        // =========================================================

        modelBuilder.Entity<User>(entity =>
        {
            entity.HasKey(e => e.UserId);

            entity.Property(e => e.FullName)
                .IsRequired()
                .HasMaxLength(150);

            entity.Property(e => e.Email)
                .IsRequired()
                .HasMaxLength(150);

            entity.HasIndex(e => e.Email)
                .IsUnique();

            entity.Property(e => e.PasswordHash)
                .IsRequired();

            entity.Property(e => e.MobileNumber)
                .HasMaxLength(15);

            entity.Property(e => e.ProfilePicturePath)
                .HasMaxLength(500);

            entity.Property(e => e.IsActive)
                .HasDefaultValue(true);

            entity.Property(e => e.IsDeleted)
                .HasDefaultValue(false);

            entity.Property(e => e.CreatedAt)
                .HasDefaultValueSql("GETDATE()")
                .HasPrecision(0);

            entity.Property(e => e.UpdatedAt)
                .HasPrecision(0);

            entity.Property(e => e.DeletedAt)
                .HasPrecision(0);

            // Role Relation
            entity.HasOne(e => e.Role)
                .WithMany(r => r.Users)
                .HasForeignKey(e => e.RoleId)
                .OnDelete(DeleteBehavior.Restrict);

            // Self Referencing Relations
            entity.HasOne(e => e.CreatedByUser)
                .WithMany()
                .HasForeignKey(e => e.CreatedBy)
                .OnDelete(DeleteBehavior.Restrict);

            entity.HasOne(e => e.UpdatedByUser)
                .WithMany()
                .HasForeignKey(e => e.UpdatedBy)
                .OnDelete(DeleteBehavior.Restrict);

            entity.HasOne(e => e.DeletedByUser)
                .WithMany()
                .HasForeignKey(e => e.DeletedBy)
                .OnDelete(DeleteBehavior.Restrict);
        });

        // =========================================================
        // ROLES
        // =========================================================

        modelBuilder.Entity<Role>(entity =>
        {
            entity.HasKey(e => e.RoleId);

            entity.Property(e => e.RoleName)
                .IsRequired()
                .HasMaxLength(50);

            entity.Property(e => e.Description)
                .HasMaxLength(250);
        });

        // =========================================================
        // PERMISSIONS
        // =========================================================

        modelBuilder.Entity<Permission>(entity =>
        {
            entity.HasKey(e => e.PermissionId);

            entity.Property(e => e.PermissionName)
                .IsRequired()
                .HasMaxLength(100);

            entity.Property(e => e.ModuleName)
                .IsRequired()
                .HasMaxLength(100);

            entity.Property(e => e.Description)
                .HasMaxLength(250);
        });

        // =========================================================
        // ROLE PERMISSIONS
        // =========================================================

        modelBuilder.Entity<RolePermission>(entity =>
        {
            entity.HasKey(e => e.RolePermissionId);

            entity.Property(e => e.CreatedAt)
                .HasDefaultValueSql("GETDATE()")
                .HasPrecision(0);

            entity.HasOne(e => e.Role)
                .WithMany(r => r.RolePermissions)
                .HasForeignKey(e => e.RoleId)
                .OnDelete(DeleteBehavior.Cascade);

            entity.HasOne(e => e.Permission)
                .WithMany(p => p.RolePermissions)
                .HasForeignKey(e => e.PermissionId)
                .OnDelete(DeleteBehavior.Cascade);
        });

        // =========================================================
        // PROJECTS
        // =========================================================

        modelBuilder.Entity<Project>(entity =>
        {
            entity.HasKey(e => e.ProjectId);

            entity.Property(e => e.ProjectTitle)
                .IsRequired()
                .HasMaxLength(200);

            entity.Property(e => e.ProjectStatus)
                .IsRequired()
                .HasMaxLength(50);

            entity.Property(e => e.ProgressPercentage)
                .HasPrecision(5, 2);

            entity.Property(e => e.IsDeleted)
                .HasDefaultValue(false);

            entity.Property(e => e.CreatedAt)
                .HasDefaultValueSql("GETDATE()")
                .HasPrecision(0);

            entity.Property(e => e.UpdatedAt)
                .HasPrecision(0);

            entity.Property(e => e.DeletedAt)
                .HasPrecision(0);

            entity.HasOne(e => e.CreatedByUser)
                .WithMany()
                .HasForeignKey(e => e.CreatedBy)
                .OnDelete(DeleteBehavior.Restrict);

            entity.HasOne(e => e.UpdatedByUser)
                .WithMany()
                .HasForeignKey(e => e.UpdatedBy)
                .OnDelete(DeleteBehavior.Restrict);

            entity.HasOne(e => e.DeletedByUser)
                .WithMany()
                .HasForeignKey(e => e.DeletedBy)
                .OnDelete(DeleteBehavior.Restrict);
        });

        // =========================================================
        // PROJECT ALLOCATIONS
        // =========================================================

        modelBuilder.Entity<ProjectAllocation>(entity =>
        {
            entity.HasKey(e => e.AllocationId);

            entity.Property(e => e.AssignedDate)
                .HasDefaultValueSql("GETDATE()")
                .HasPrecision(0);

            entity.Property(e => e.AllocationStatus)
                .IsRequired()
                .HasMaxLength(50);

            entity.Property(e => e.IsDeleted)
                .HasDefaultValue(false);

            entity.Property(e => e.CreatedAt)
                .HasDefaultValueSql("GETDATE()")
                .HasPrecision(0);

            // Project Relation
            entity.HasOne(e => e.Project)
                .WithMany(p => p.ProjectAllocations)
                .HasForeignKey(e => e.ProjectId)
                .OnDelete(DeleteBehavior.Cascade);

            // Student Relation
            entity.HasOne(e => e.Student)
                .WithMany()
                .HasForeignKey(e => e.StudentId)
                .OnDelete(DeleteBehavior.Restrict);

            // Faculty Relation
            entity.HasOne(e => e.Faculty)
                .WithMany()
                .HasForeignKey(e => e.FacultyId)
                .OnDelete(DeleteBehavior.Restrict);

            entity.HasOne(e => e.CreatedByUser)
                .WithMany()
                .HasForeignKey(e => e.CreatedBy)
                .OnDelete(DeleteBehavior.Restrict);
        });

        // =========================================================
        // TASKS
        // =========================================================

        modelBuilder.Entity<ProjectTask>(entity =>
        {
            entity.HasKey(e => e.TaskId);

            entity.Property(e => e.TaskTitle)
                .IsRequired()
                .HasMaxLength(200);

            entity.Property(e => e.TaskStatus)
                .IsRequired()
                .HasMaxLength(50);

            entity.Property(e => e.Priority)
                .HasMaxLength(20);

            entity.Property(e => e.AssignedScore)
                .HasPrecision(5, 2);

            entity.Property(e => e.EarnedScore)
                .HasPrecision(5, 2);

            entity.Property(e => e.ProgressPercentage)
                .HasPrecision(5, 2);

            entity.Property(e => e.FacultyRemarks)
                .HasMaxLength(500);

            entity.Property(e => e.StudentRemarks)
                .HasMaxLength(500);

            entity.Property(e => e.IsDeleted)
                .HasDefaultValue(false);

            entity.Property(e => e.CreatedAt)
                .HasDefaultValueSql("GETDATE()")
                .HasPrecision(0);

            entity.Property(e => e.UpdatedAt)
                .HasPrecision(0);

            entity.Property(e => e.DeletedAt)
                .HasPrecision(0);

            entity.Property(e => e.StartDate)
                .HasPrecision(0);

            entity.Property(e => e.DueDate)
                .HasPrecision(0);

            entity.Property(e => e.CompletedDate)
                .HasPrecision(0);

            // Project Relation
            entity.HasOne(e => e.Project)
                .WithMany(p => p.Tasks)
                .HasForeignKey(e => e.ProjectId)
                .OnDelete(DeleteBehavior.Cascade);

            // Student Relation
            entity.HasOne(e => e.Student)
                .WithMany()
                .HasForeignKey(e => e.StudentId)
                .OnDelete(DeleteBehavior.Restrict);

            // Faculty Relation
            entity.HasOne(e => e.Faculty)
                .WithMany()
                .HasForeignKey(e => e.FacultyId)
                .OnDelete(DeleteBehavior.Restrict);

            entity.HasOne(e => e.CreatedByUser)
                .WithMany()
                .HasForeignKey(e => e.CreatedBy)
                .OnDelete(DeleteBehavior.Restrict);

            entity.HasOne(e => e.UpdatedByUser)
                .WithMany()
                .HasForeignKey(e => e.UpdatedBy)
                .OnDelete(DeleteBehavior.Restrict);

            entity.HasOne(e => e.DeletedByUser)
                .WithMany()
                .HasForeignKey(e => e.DeletedBy)
                .OnDelete(DeleteBehavior.Restrict);
        });
    }
}