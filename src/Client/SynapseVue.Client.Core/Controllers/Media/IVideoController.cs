using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using SynapseVue.Shared.Dtos.Media;
using SynapseVue.Shared.Dtos.Products;

namespace SynapseVue.Client.Core.Controllers.Media;

[Route("api/[controller]/[action]/")]
public interface IVideoController : IAppController
{
    [HttpGet("{id}")]
    Task<VideoDto> Get(int id, CancellationToken cancellationToken = default);

    [HttpPost]
    Task<VideoDto> Create(VideoDto body, CancellationToken cancellationToken = default);

    [HttpPut]
    Task<VideoDto> Update(VideoDto body, CancellationToken cancellationToken = default);

    [HttpDelete("{id}")]
    Task Delete(int id, CancellationToken cancellationToken = default);

    [HttpGet]
    Task<PagedResult<VideoDto>> GetVideos(CancellationToken cancellationToken = default) => default!;

    [HttpGet]
    Task<List<VideoDto>> Get(CancellationToken cancellationToken) => default!;
}
