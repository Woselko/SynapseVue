using Riok.Mapperly.Abstractions;
using SynapseVue.Server.Models.Media;
using SynapseVue.Shared.Dtos.Media;


namespace SynapseVue.Server.Mappers;

/// <summary>
/// More info at Server/Mappers/README.md
/// </summary>
[Mapper(UseDeepCloning = true)]
public static partial class VideoMapper
{
    //public static partial IQueryable<VideoDto> Project(this IQueryable<Video> query);
    //public static partial VideoDto Map(this Video source);
    //public static partial Video Map(this VideoDto source);
    //public static partial void Patch(this VideoDto source, Video destination);
    //public static partial void Patch(this Video source, VideoDto destination);


    public static partial IQueryable<VideoDto> Project(this IQueryable<Video> query);
    public static partial VideoDto Map(this Video source);
    public static partial Video Map(this VideoDto source);
    public static partial void Patch(this VideoDto source, Video destination);
    public static partial void Patch(this Video source, VideoDto destination);
}
