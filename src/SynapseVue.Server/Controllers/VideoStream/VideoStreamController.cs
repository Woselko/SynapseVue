using Microsoft.AspNetCore.Mvc;
using SynapseVue.Client.Core.Controllers.VideoStream;
using SynapseVue.Shared.Dtos.Categories;
using SynapseVue.RaspCameraLibrary;
using System.Diagnostics;

namespace SynapseVue.Server.Controllers.VideoStream;

[Route("api/[controller]/[action]")]
[ApiController, AllowAnonymous]
public partial class VideoStreamController : AppControllerBase, IVideoStreamController
{
    
}
