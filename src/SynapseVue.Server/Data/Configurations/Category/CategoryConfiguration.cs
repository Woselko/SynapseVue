using SynapseVue.Server.Models.Categories;

namespace SynapseVue.Server.Data.Configurations.Identity;

public class CategoryConfiguration : IEntityTypeConfiguration<Category>
{
    public void Configure(EntityTypeBuilder<Category> builder)
    {
        builder.HasData(
            new() { Id = 1, Name = "Safety", Color = "#FFCD56" },
            new() { Id = 2, Name = "Sensor", Color = "#FF6384" },
            new() { Id = 3, Name = "Display", Color = "#4BC0C0" },
            new() { Id = 4, Name = "Sound", Color = "#FF9124" },
            new() { Id = 5, Name = "Camera", Color = "#2B88D8" });
    }
}

