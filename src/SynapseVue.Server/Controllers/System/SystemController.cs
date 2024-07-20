using SynapseVue.Client.Core.Controllers.System;
using SynapseVue.Server.Models.System;
using SynapseVue.Shared.Dtos.System;

namespace SynapseVue.Server.Controllers;

[Route("api/[controller]/[action]")]
[ApiController]
public partial class SystemController : AppControllerBase, ISystemController
{
    // public IQueryable<SystemStateDto> Get()
    // {
    //     return DbContext.SystemStates.Project();
    // }

    [HttpGet]
    public async Task<SystemStateDto> Get(CancellationToken cancellationToken)
    {
        var dto = await DbContext.SystemStates.Project().FirstOrDefaultAsync(t => t.Id == 1, cancellationToken);

        if (dto is null)
            throw new ResourceNotFoundException(Localizer[nameof(AppStrings.ProductCouldNotBeFound)]);

        return dto;
    }

    [HttpPut]
    public async Task<SystemStateDto> Update(SystemStateDto dto, CancellationToken cancellationToken)
    {
        if (dto.Mode != "Home" && dto.Mode != "Safe")
            throw new InvalidOperationException("Invalid mode");

        var entityToUpdate = await DbContext.SystemStates.FirstOrDefaultAsync(t => t.Id == dto.Id, cancellationToken);

        if (entityToUpdate is null)
            throw new ResourceNotFoundException(Localizer[nameof(AppStrings.ProductCouldNotBeFound)]);

        dto.Patch(entityToUpdate);

        await DbContext.SaveChangesAsync(cancellationToken);

        return entityToUpdate.Map();
    }

}

