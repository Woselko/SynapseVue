using SynapseVue.Server.Models.Media;
using SynapseVue.Server.Models.Products;

namespace SynapseVue.Server.Data.Configurations.Media;

public class VideoConfiguration : IEntityTypeConfiguration<Video>
{
    public void Configure(EntityTypeBuilder<Video> builder)
    {
        DateTime baseDate = DateTime.Parse("2024-06-03");

        builder.HasData(
            new Video { Id = 1, Name = "video1", CreatedAt = baseDate, IsProcessed = false, Description = "MyVideo 1", FilePath = @"Media/test.avi", IsPersonDetected = false, DetectedObjects = "nothing", FileSize= 14324 },
            new Video { Id = 2, Name = "video2", CreatedAt = baseDate, IsProcessed = false, Description = "MyVideo 2", FilePath = @"Media/test1.avi", IsPersonDetected = false, DetectedObjects = "nothing", FileSize = 34124 }
            );
    }
}

