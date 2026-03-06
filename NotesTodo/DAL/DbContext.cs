using Microsoft.EntityFrameworkCore;
using NotesTodo.Models;

namespace NotesTodo.DAL
{
    public class TodoDb: DbContext
    {
        public TodoDb(DbContextOptions<TodoDb> options) : base(options) 
        {
        
        }
        public DbSet<User> Users { get; set; } = null!;
        public DbSet<TodoProject> TodoProjects { get; set; } = null!;
        public DbSet<ProjectMembership> ProjectMemberships { get; set; } = null!;
        public DbSet<Todo> Todos { get; set; } = null!;
        public DbSet<SubTask> SubTasks { get; set; } = null!;
         protected override void OnModelCreating(ModelBuilder modelBuilder)
        {
            modelBuilder.Entity<ProjectMembership>()
                .HasKey(pm => new { pm.UserId, pm.TodoProjectId });

            modelBuilder.Entity<ProjectMembership>()
                .HasOne(pm => pm.User)
                .WithMany(u => u.ProjectMemberships)
                .HasForeignKey(pm => pm.UserId);

            modelBuilder.Entity<ProjectMembership>()
                .HasOne(pm => pm.TodoProject)
                .WithMany(tp => tp.Members)
                .HasForeignKey(pm => pm.TodoProjectId);

            modelBuilder.Entity<SubTask>()
                .HasOne(st => st.Todo)
                .WithMany(t => t.SubTask)
                .HasForeignKey(st => st.TodoId);

            modelBuilder.Entity<Todo>()
                .HasOne(t => t.TodoProject)
                .WithMany(tp => tp.Todos)
                .HasForeignKey(p => p.TodoProjectId)
                .OnDelete(DeleteBehavior.Restrict);

            modelBuilder.Entity<Todo>()
                .HasOne(t=> t.CompletedByUser)
                .WithMany()
                .HasForeignKey(t => t.CompletedByUserId)
                .OnDelete(DeleteBehavior.Restrict);

            modelBuilder.Entity<Todo>()
                .HasOne(t => t.CreatedByUser)
                .WithMany()
                .HasForeignKey(t => t.CreatedByUserId)
                .OnDelete(DeleteBehavior.Restrict);

            modelBuilder.Entity<Todo>()
                .HasMany(t => t.AssignedUsers)
                .WithMany(u => u.AssignedTodos);

                modelBuilder.Entity<SubTask>()
                .HasMany(t => t.AssingedUsers)
                .WithMany(u => u.AssignedSubTasks);
        }
    }
}
