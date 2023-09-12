using Microsoft.EntityFrameworkCore;

namespace ToDoAPI
{
    class ToDoDB : DbContext
    {
        public ToDoDB(DbContextOptions<ToDoDB> options)
            : base(options) { }
        public DbSet<ToDo> ToDos => Set<ToDo>();
    }
}
