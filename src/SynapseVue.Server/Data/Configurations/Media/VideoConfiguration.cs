using System.IO;
using SynapseVue.Server.Models.Media;
using SynapseVue.Server.Models.Products;

namespace SynapseVue.Server.Data.Configurations.Media;

public class VideoConfiguration : IEntityTypeConfiguration<Video>
{
    public void Configure(EntityTypeBuilder<Video> builder)
    {
        DateTime baseDate = DateTime.Parse("2024-06-03");

        builder.HasData(
            new Video { Id = 1, Name = "test.avi", CreatedAt = baseDate, IsProcessed = true, Description = "MyVideo 1", FilePath = Path.Combine(Directory.GetCurrentDirectory(), "wwwroot", "videos", "test.avi"), IsPersonDetected = false, DetectedObjects = "nothing", FileSize= 14324 },
            new Video { Id = 2, Name = "test1.avi", CreatedAt = baseDate, IsProcessed = false, Description = "MyVideo 2", FilePath = Path.Combine(Directory.GetCurrentDirectory(), "wwwroot", "videos", "test1.avi"), IsPersonDetected = false, DetectedObjects = "nothing", FileSize = 34124 },
            new Video { Id = 3, Name = "film.mp4", CreatedAt = baseDate, IsProcessed = true, Description = "MyVideo 3 mp3", FilePath = Path.Combine(Directory.GetCurrentDirectory(), "wwwroot", "videos", "film.mp4"), IsPersonDetected = false, DetectedObjects = "nothing", FileSize = 34124 }
            );
    }
}

