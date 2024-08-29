using System.Diagnostics;
using SynapseVue.Client.Core.Controllers.System;
using SynapseVue.Server.Models.System;
using SynapseVue.Shared.Dtos.System;

namespace SynapseVue.Server.Controllers;

[Route("api/[controller]/[action]")]
[ApiController]
public partial class SystemController : AppControllerBase, ISystemController
{
    [HttpGet, EnableQuery]
    public IQueryable<SystemStateDto> Get()
    {
        return DbContext.SystemStates
            .Project();
    }

    [HttpPost]
    public async Task<SystemStateDto> Reboot()
    {
        Task.Run(() =>
        {
            var processStartInfo = new ProcessStartInfo
            {
                FileName = "/bin/bash",
                Arguments = "-c \"sudo reboot\"",
                RedirectStandardOutput = true,
                RedirectStandardError = true,
                UseShellExecute = false,
                CreateNoWindow = true
            };

            using (var process = new Process { StartInfo = processStartInfo })
            {
                process.Start();
                process.WaitForExit();
            }
        });
        return new SystemStateDto{Id = 1, Property ="Mode", Value = "Rebooting"};
    }

    [HttpGet]
    public async Task<SystemStateDto> GetSystemMode(CancellationToken cancellationToken)
    {
        var dto = await DbContext.SystemStates.Project().FirstOrDefaultAsync(t => t.Property == "Mode", cancellationToken);

        if (dto is null)
            throw new ResourceNotFoundException(Localizer[nameof(AppStrings.ProductCouldNotBeFound)]);

        return dto;
    }

    [HttpGet]
    public async Task<PagedResult<SystemStateDto>> GetSystemSettings(ODataQueryOptions<SystemStateDto> odataQuery, CancellationToken cancellationToken)
    {
        var query = (IQueryable<SystemStateDto>)odataQuery.ApplyTo(Get(), ignoreQueryOptions: AllowedQueryOptions.Top | AllowedQueryOptions.Skip);

        var totalCount = await query.LongCountAsync(cancellationToken);

        if (odataQuery.Skip is not null)
            query = query.Skip(odataQuery.Skip.Value);

        if (odataQuery.Top is not null)
            query = query.Take(odataQuery.Top.Value);

        return new PagedResult<SystemStateDto>(await query.ToArrayAsync(cancellationToken), totalCount);
    }

    [HttpPut]
    public async Task<SystemStateDto> Update(SystemStateDto dto, CancellationToken cancellationToken)
    {
        if (dto.Property=="Mode" && dto.Value != "Home" && dto.Value != "Safe")
            throw new InvalidOperationException("Invalid mode");

        var entityToUpdate = await DbContext.SystemStates.FirstOrDefaultAsync(t => t.Id == dto.Id, cancellationToken);
        
        if (entityToUpdate is null)
            throw new ResourceNotFoundException(Localizer[nameof(AppStrings.ProductCouldNotBeFound)]);

        dto.Patch(entityToUpdate);

        await DbContext.SaveChangesAsync(cancellationToken);
        if (dto.Property == "Mode" && dto.Value == "Home")
        {
            MainMotionDetectionService.StopMonitoring();
        }
        if(dto.Property == "Mode" && dto.Value == "Safe")
        {
            MainMotionDetectionService.StartMonitoring();
        }
        return entityToUpdate.Map();
    }

}

