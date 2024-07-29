using SynapseVue.Server.Models.Categories;

namespace SynapseVue.Server.Data.Configurations.Identity;

public class CategoryConfiguration : IEntityTypeConfiguration<Category>
{
    public void Configure(EntityTypeBuilder<Category> builder)
    {
        builder.HasData(
            new() { Id = 1, Name = "DHT22", Color = "#FFCD56" },
            new() { Id = 2, Name = "DHT11", Color = "#441C4D" },
            new() { Id = 3, Name = "PIR", Color = "#FF6384" },
            new() { Id = 4, Name = "LED", Color = "#07E80B" },
            new() { Id = 5, Name = "LCD-DISPLAY", Color = "#4BC0C0" },
            new() { Id = 6, Name = "BUZZ", Color = "#FF9124" },
            new() { Id = 7, Name = "CAMERA", Color = "#2B88D8" },
            new() { Id = 8, Name = "RFID", Color = "#DB650B" },
            new() { Id = 9, Name = "BH1750", Color = "#07E8B0" },
            new() { Id = 10, Name = "MQ-9", Color = "#BD040D" },
            new() { Id = 11, Name = "MQ-3", Color = "#DE0064" });
    }
}

