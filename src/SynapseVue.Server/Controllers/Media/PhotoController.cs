using SynapseVue.Client.Core.Controllers.Media;
using SynapseVue.Client.Core.Controllers.Product;
using SynapseVue.Shared.Dtos.Media;

namespace SynapseVue.Server.Controllers.Media;


[Route("api/[controller]/[action]")]
[ApiController]
public partial class PhotoController : AppControllerBase, IPhotoController
{
    [HttpGet, EnableQuery]
    public IQueryable<PhotoDto> Get()
    {
        return DbContext.Photos
            .Project();
    }

    [HttpGet]
    public async Task<PagedResult<PhotoDto>> GetPhotos(ODataQueryOptions<PhotoDto> odataQuery, CancellationToken cancellationToken)
    {
        var query = (IQueryable<PhotoDto>)odataQuery.ApplyTo(Get(), ignoreQueryOptions: AllowedQueryOptions.Top | AllowedQueryOptions.Skip);

        var totalCount = await query.LongCountAsync(cancellationToken);

        if (odataQuery.Skip is not null)
            query = query.Skip(odataQuery.Skip.Value);

        if (odataQuery.Top is not null)
            query = query.Take(odataQuery.Top.Value);

        return new PagedResult<PhotoDto>(await query.ToArrayAsync(cancellationToken), totalCount);
    }

    [HttpGet("{id}")]
    public async Task<PhotoDto> Get(int id, CancellationToken cancellationToken)
    {
        var dto = await Get().FirstOrDefaultAsync(t => t.Id == id, cancellationToken);

        if (dto is null)
            throw new ResourceNotFoundException(Localizer[nameof(AppStrings.ProductCouldNotBeFound)]);

        return dto;
    }

    [HttpPost]
    public async Task<PhotoDto> Create(PhotoDto dto, CancellationToken cancellationToken)
    {
        var entityToAdd = dto.Map();

        await DbContext.Photos.AddAsync(entityToAdd, cancellationToken);

        await DbContext.SaveChangesAsync(cancellationToken);

        return entityToAdd.Map();
    }

    [HttpPut]
    public async Task<PhotoDto> Update(PhotoDto dto, CancellationToken cancellationToken)
    {
        var entityToUpdate = await DbContext.Photos.FirstOrDefaultAsync(t => t.Id == dto.Id, cancellationToken);

        if (entityToUpdate is null)
            throw new ResourceNotFoundException(Localizer[nameof(AppStrings.ProductCouldNotBeFound)]);

        dto.Patch(entityToUpdate);

        await DbContext.SaveChangesAsync(cancellationToken);

        return entityToUpdate.Map();
    }

    [HttpDelete("{id}")]
    public async Task Delete(int id, CancellationToken cancellationToken)
    {
        DbContext.Photos.Remove(new() { Id = id });

        var affectedRows = await DbContext.SaveChangesAsync(cancellationToken);

        if (affectedRows < 1)
            throw new ResourceNotFoundException(Localizer[nameof(AppStrings.ProductCouldNotBeFound)]);
    }
}
