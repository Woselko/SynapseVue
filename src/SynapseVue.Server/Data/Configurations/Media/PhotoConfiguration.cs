using SynapseVue.Server.Models.Media;

namespace SynapseVue.Server.Data.Configurations.Media;

public class PhotoConfiguration : IEntityTypeConfiguration<Photo>
{
    public void Configure(EntityTypeBuilder<Photo> builder)
    {
        DateTime baseDate = DateTime.Parse("2024-06-03");

        builder.HasData(
            new Photo { Id = 1, Name = "photo1", CreatedAt = baseDate, Description = "MyPhoto1", Data = new byte[] { 1, 2, 3, 4, 5, 6, 7, 8, 9, 10 } },
            new Photo { Id = 2, Name = "photo2", CreatedAt = baseDate, Description = "MyPhoto2", Data = new byte[] { 1, 2, 3, 4, 5, 6, 7, 8, 9, 10 } });
    }
}
