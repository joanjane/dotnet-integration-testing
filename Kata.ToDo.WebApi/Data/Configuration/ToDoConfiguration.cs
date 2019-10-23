using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace Kata.ToDo.WebApi.Data.Configuration
{
    public class ToDoConfiguration : IEntityTypeConfiguration<Entities.ToDo>
    {
        public void Configure(EntityTypeBuilder<Entities.ToDo> builder)
        {
            builder
                .ToTable("ToDos")
                .HasKey(e => e.Id);
        }
    }
}
