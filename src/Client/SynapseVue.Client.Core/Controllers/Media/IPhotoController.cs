using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using SynapseVue.Shared.Dtos.Media;

namespace SynapseVue.Client.Core.Controllers.Media;

[Route("api/[controller]/[action]/")]
public interface IPhotoController : IAppController
{
    [HttpGet("{id}")]
    Task<PhotoDto> Get(int id, CancellationToken cancellationToken = default);

    [HttpPost]
    Task<PhotoDto> Create(PhotoDto body, CancellationToken cancellationToken = default);

    [HttpPut]
    Task<PhotoDto> Update(PhotoDto body, CancellationToken cancellationToken = default);

    [HttpDelete("{id}")]
    Task Delete(int id, CancellationToken cancellationToken = default);

    [HttpGet]
    Task<PagedResult<PhotoDto>> GetPhotos(CancellationToken cancellationToken = default) => default!;

    [HttpGet]
    Task<List<PhotoDto>> Get(CancellationToken cancellationToken) => default!;
}
