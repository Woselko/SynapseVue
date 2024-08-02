using Riok.Mapperly.Abstractions;
using SynapseVue.Server.Models.Media;
using SynapseVue.Server.Models.System;
using SynapseVue.Shared.Dtos.Media;
using SynapseVue.Shared.Dtos.System;

namespace SynapseVue.Server.Mappers;
/// <summary>
/// More info at Server/Mappers/README.md
/// </summary>
[Mapper(UseDeepCloning = true)]
public static partial class PhotoMapper
{
    public static partial IQueryable<PhotoDto> Project(this IQueryable<Photo> query);
    public static partial PhotoDto Map(this Photo source);
    public static partial Photo Map(this PhotoDto source);
    public static partial void Patch(this PhotoDto source, Photo destination);
    public static partial void Patch(this Photo source, PhotoDto destination);
}
