using Microsoft.EntityFrameworkCore;

namespace NotesTodo.DAL
{
    public class TodoDb: DbContext
    {
        public TodoDb(DbContextOptions<TodoDb> options) : base(options) 
        {
        
        }
        public DbSet<Models.User> Users { get; set; } = null!;
        public DbSet<Models.TodoProject> TodoProjects { get; set; } = null!;
        public DbSet<Models.ProjectMembership> ProjectMemberships { get; set; } = null!;
        public DbSet<Models.Todo> Todos { get; set; } = null!;
        public DbSet<Models.SubTask> SubTasks { get; set; } = null!;
         protected override void OnModelCreating(ModelBuilder modelBuilder)
        {
            //modelBuilder.Entity<Models.ProjectMembership>()
            //    .HasKey(pm => new { pm.UserId, pm.TodoProject.Id });
            //modelBuilder.Entity<Models.ProjectMembership>()
            //    .HasOne(pm => pm.User)
            //    .WithMany(u => u.ProjectMemberships)
            //    .HasForeignKey(pm => pm.UserId);
            //modelBuilder.Entity<Models.ProjectMembership>()
            //    .HasOne(pm => pm.TodoProject)
            //    .WithMany(tp => tp.Members)
            //    .HasForeignKey(pm => pm.TodoProject.Id);
            //modelBuilder.Entity<Models.SubTask>()
            //    .HasOne(st => st.Todo)
            //    .WithMany(t => t.Members)
            //    .HasForeignKey(st => st.Id);
        }
    }
}
