using SynapseVue.Shared.Dtos.System;


namespace SynapseVue.Client.Core.Controllers.System;

[Route("api/[controller]/[action]/")]
public interface ISystemController : IAppController
{
    [HttpPost]
    Task<SystemStateDto> Reboot();

    [HttpGet]
    Task<SystemStateDto> Get(CancellationToken cancellationToken = default);

    [HttpPut]
    Task<SystemStateDto> Update(SystemStateDto body, CancellationToken cancellationToken = default);
}