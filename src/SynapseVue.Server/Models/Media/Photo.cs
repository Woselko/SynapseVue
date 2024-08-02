namespace SynapseVue.Server.Models.Media;

public class Photo
{
    public int Id { get; set; }

    [MaxLength(64)]
    public string Name { get; set; }

    public byte[] Data { get; set; }

    [MaxLength(512)]
    public DateTimeOffset? CreatedAt { get; set; } = DateTimeOffset.UtcNow;

    [MaxLength(512)]
    public string? Description { get; set; }
}
