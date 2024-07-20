using SynapseVue.Server.Models.System;
using SynapseVue.Shared.Dtos.System;
using Riok.Mapperly.Abstractions;

namespace SynapseVue.Server.Mappers;

/// <summary>
/// More info at Server/Mappers/README.md
/// </summary>
[Mapper(UseDeepCloning = true)]
public static partial class SystemStateMapper
{
    public static partial IQueryable<SystemStateDto> Project(this IQueryable<SystemState> query);
    public static partial SystemStateDto Map(this SystemState source);
    public static partial SystemState Map(this SystemStateDto source);
    public static partial void Patch(this SystemStateDto source, SystemState destination);
    public static partial void Patch(this SystemState source, SystemStateDto destination);
}
