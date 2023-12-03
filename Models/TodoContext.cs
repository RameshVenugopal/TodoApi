using Microsoft.EntityFrameworkCore;

namespace TodoApi.Models
{
    public class TodoContext : DbContext
    {
        private readonly string _schema;

        public TodoContext(DbContextOptions<TodoContext> options, string schema = "dbo")
            : base(options)
        {
            _schema = schema;
        }

        public DbSet<TodoItem> TodoItems { get; set; }

        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {
            if (!string.IsNullOrEmpty(_schema))
            {
                modelBuilder.HasDefaultSchema(_schema);
            }
            base.OnModelCreating(modelBuilder);
        }
    }
}


