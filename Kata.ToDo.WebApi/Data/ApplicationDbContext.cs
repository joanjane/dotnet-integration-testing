using Kata.ToDo.WebApi.Data.Configuration;
using Microsoft.EntityFrameworkCore;

namespace Kata.ToDo.WebApi.Data
{
    public class ApplicationDbContext : DbContext
    {
        public ApplicationDbContext(DbContextOptions options) : base(options)
        {

        }
        public DbSet<Entities.ToDo> ToDos { get; set; }

        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {
            modelBuilder.ApplyConfiguration(new ToDoConfiguration());
        }
    }
}
