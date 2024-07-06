using Microsoft.AspNetCore.Mvc;
using SynapseVue.Client.Core.Controllers.VideoStream;
using SynapseVue.Shared.Dtos.Categories;

namespace SynapseVue.Server.Controllers.VideoStream;

[Route("api/[controller]/[action]")]
[ApiController, AllowAnonymous]
public partial class VideoStreamController : AppControllerBase, IVideoStreamController
{
    [HttpGet]
    public async Task<int> GetVideoStream(CancellationToken cancellationToken)
    {
        return 13;
    }
}
