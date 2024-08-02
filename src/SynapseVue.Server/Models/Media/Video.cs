namespace SynapseVue.Server.Models.Media;

public class Video
{
    public int Id { get; set; }

    [MaxLength(64)]
    public string Name { get; set; }

    public bool IsProcessed { get; set; } = false;

    public bool IsPersonDetected { get; set; } = false;

    public DateTimeOffset? CreatedAt { get; set; } = DateTimeOffset.UtcNow;

    [MaxLength(512)]
    public string FilePath { get; set; }

    [MaxLength(512)]
    public string DetectedObjects { get; set; }

    [MaxLength(512)]
    public string? Description { get; set; }

    public long FileSize { get; set; }
}
