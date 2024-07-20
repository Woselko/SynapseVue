
namespace SynapseVue.Client.Core.Controllers.VideoStream;

[Route("api/[controller]/[action]/")]
public interface IVideoStreamController : IAppController
{
    Task Get();
    Task<bool> Stop();
}
