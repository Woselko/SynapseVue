using SynapseVue.Client.Core.Controllers.Media;
using SynapseVue.Shared.Dtos.Media;

namespace SynapseVue.Server.Controllers.Media;

[Route("api/[controller]/[action]")]
[ApiController]
public partial class VideoController : AppControllerBase, IVideoController
{
    [HttpGet, EnableQuery]
    public IQueryable<VideoDto> Get()
    {
        return DbContext.Videos
            .Project();
    }

    [HttpGet]
    public async Task<PagedResult<VideoDto>> GetVideos(ODataQueryOptions<VideoDto> odataQuery, CancellationToken cancellationToken)
    {
        var query = (IQueryable<VideoDto>)odataQuery.ApplyTo(Get(), ignoreQueryOptions: AllowedQueryOptions.Top | AllowedQueryOptions.Skip);

        var totalCount = await query.LongCountAsync(cancellationToken);

        if (odataQuery.Skip is not null)
            query = query.Skip(odataQuery.Skip.Value);

        if (odataQuery.Top is not null)
            query = query.Take(odataQuery.Top.Value);

        return new PagedResult<VideoDto>(await query.ToArrayAsync(cancellationToken), totalCount);
    }

    [HttpGet("{id}")]
    public async Task<VideoDto> Get(int id, CancellationToken cancellationToken)
    {
        var dto = await Get().FirstOrDefaultAsync(t => t.Id == id, cancellationToken);

        if (dto is null)
            throw new ResourceNotFoundException(Localizer[nameof(AppStrings.ProductCouldNotBeFound)]);

        return dto;
    }

    [HttpPost]
    public async Task<VideoDto> Create(VideoDto dto, CancellationToken cancellationToken)
    {
        var entityToAdd = dto.Map();

        await DbContext.Videos.AddAsync(entityToAdd, cancellationToken);

        await DbContext.SaveChangesAsync(cancellationToken);

        return entityToAdd.Map();
    }

    [HttpPut]
    public async Task<VideoDto> Update(VideoDto dto, CancellationToken cancellationToken)
    {
        var entityToUpdate = await DbContext.Videos.FirstOrDefaultAsync(t => t.Id == dto.Id, cancellationToken);

        if (entityToUpdate is null)
            throw new ResourceNotFoundException(Localizer[nameof(AppStrings.ProductCouldNotBeFound)]);

        dto.Patch(entityToUpdate);

        await DbContext.SaveChangesAsync(cancellationToken);

        return entityToUpdate.Map();
    }

    [HttpDelete("{id}")]
    public async Task Delete(int id, CancellationToken cancellationToken)
    {
        var entityToDelete = Get().FirstOrDefault(t => t.Id == id);
        // var entityToDelete = await DbContext.Videos.FirstOrDefaultAsync(t => t.Id == id, cancellationToken);
        var fileToDelete = entityToDelete.FilePath;
        DbContext.Videos.Remove(new() { Id = id });

        var affectedRows = await DbContext.SaveChangesAsync(cancellationToken);
        
        //delete file
        if (System.IO.File.Exists(fileToDelete))
            System.IO.File.Delete(fileToDelete);
        
        if (affectedRows < 1)
            throw new ResourceNotFoundException(Localizer[nameof(AppStrings.ProductCouldNotBeFound)]);
    }
}
