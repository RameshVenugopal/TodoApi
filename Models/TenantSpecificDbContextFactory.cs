using TodoApi.Models;
using Microsoft.EntityFrameworkCore;

namespace TodoApi.Models
{
    public class TenantSpecificDbContextFactory
    {
        private readonly DbContextOptions<TodoContext> _options;
        public string _schema;

        public TenantSpecificDbContextFactory(DbContextOptions<TodoContext> options, string schema)
        {
            _options = options;
            _schema = schema;
        }

        public TodoContext Create()
        {
            return new TodoContext(_options, _schema);
        }
    }
}