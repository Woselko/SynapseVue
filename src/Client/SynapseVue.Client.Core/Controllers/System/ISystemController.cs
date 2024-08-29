using SynapseVue.Shared.Dtos.Products;
using SynapseVue.Shared.Dtos.System;


namespace SynapseVue.Client.Core.Controllers.System;

[Route("api/[controller]/[action]/")]
public interface ISystemController : IAppController
{
    [HttpPost]
    Task<SystemStateDto> Reboot();

    [HttpGet]
    Task<SystemStateDto> GetSystemMode(CancellationToken cancellationToken = default);

    [HttpGet]
    Task<PagedResult<SystemStateDto>> GetSystemSettings(CancellationToken cancellationToken = default) => default!;

    [HttpPut]
    Task<SystemStateDto> Update(SystemStateDto body, CancellationToken cancellationToken = default);
}
