using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace SynapseVue.Client.Core.Controllers.VideoStream;

[Route("api/[controller]/[action]/")]
public interface IVideoStreamController : IAppController
{
    [HttpGet]
    public Task<int> GetVideoStream(CancellationToken cancellationToken);
}
