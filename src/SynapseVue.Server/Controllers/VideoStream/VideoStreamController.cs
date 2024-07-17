using Microsoft.AspNetCore.Mvc;
using SynapseVue.Client.Core.Controllers.VideoStream;
using SynapseVue.Shared.Dtos.Categories;
using SynapseVue.RaspCameraLibrary;
using SynapseVue.RaspCameraLibrary.Helpers;
using SynapseVue.RaspCameraLibrary.Settings;
using System.Diagnostics;
using Microsoft.AspNetCore.Http.Features;
using SynapseVue.RaspCameraLibrary.Settings.Enumerations;
using SynapseVue.RaspCameraLibrary.Settings.Codecs;
using SynapseVue.RaspCameraLibrary.Settings.Types;
using Newtonsoft.Json;

namespace SynapseVue.Server.Controllers.VideoStream;

[Route("api/[controller]/[action]")]
[ApiController, AllowAnonymous]
public partial class VideoStreamController : AppControllerBase, IVideoStreamController
{
    private static CancellationTokenSource? cancellationTokenSource { get; set; }

    [HttpGet(Name = "StopVideo")]
    public async Task<IActionResult> Stop()
    {
        if(cancellationTokenSource != null)
        {
            cancellationTokenSource.Cancel();
            return Ok();
        }
        return BadRequest();
    }

    [HttpGet(Name = "GetVideo")]
    public async Task Get()
    {
        List<Camera>? Cameras = RaspCameraLibrary.Video.ListCameras().Result;

        Console.WriteLine(JsonConvert.SerializeObject(Cameras, Formatting.Indented));
        //var modes = Cameras.FirstOrDefault(c => c.Id == 0).Modes;
        //var mode = Cameras.FirstOrDefault(c => c.Id == 0).Modes[3];

        cancellationTokenSource = new CancellationTokenSource();

        VideoSettings Settings = new Mjpeg()
        {
            Camera = 0,
            Width = 1280,
            Height = 720,
            // Width = 320,
            // Height = 240,
            Timeout = 0,
            Flush = true,
            HFlip = false,
            VFlip = false,
            Framerate = 10,
            //Mode= mode,
            WhiteBalance = WhiteBalance.Incandescent,
            Output = "/dev/stdout"
        };

        ProcessStartInfo captureStartInfo = RaspCameraLibrary.VideoStream.CaptureStartInfo(Settings);
        var client = new RaspCameraLibrary.VideoStream();
        client.NewImageReceived += NewImageReceived;

        try
        {
            var bufferingFeature = HttpContext.Response.HttpContext.Features.Get<IHttpResponseBodyFeature>();
                bufferingFeature?.DisableBuffering();

            HttpContext.Response.StatusCode = 200;
            HttpContext.Response.ContentType = "multipart/x-mixed-replace; boundary=--frame";
            HttpContext.Response.Headers.Add("Connection", "Keep-Alive");
            HttpContext.Response.Headers.Add("CacheControl", "no-cache");
            HttpContext.Response.Headers.Add("Access-Control-Allow-Origin", "*");
            
            var task = client.StartFrameReaderAsync(captureStartInfo, cancellationTokenSource.Token);
            while (!HttpContext.RequestAborted.IsCancellationRequested) { }
        }
        catch (Exception ex)
        {

        }
        finally
        {
            HttpContext.Response.Body.Close();
        }

        client.NewImageReceived -= NewImageReceived;

    }

    private byte[] CreateHeader(int length)
    {
        string header =
            $"--frame\r\nContent-Type:image/jpeg\r\nContent-Length:{length}\r\n\r\n";
        return System.Text.Encoding.ASCII.GetBytes(header);
    }

    private byte[] CreateFooter()
    {
        return System.Text.Encoding.ASCII.GetBytes("\r\n");
    }

    private async void NewImageReceived(byte[] imageData)
    {
        try
        {
            await HttpContext.Response.BodyWriter.WriteAsync(CreateHeader(imageData.Length));
            await HttpContext.Response.BodyWriter.WriteAsync(imageData);
            await HttpContext.Response.BodyWriter.WriteAsync(CreateFooter());
        }
        catch (ObjectDisposedException)
        {
            // ignore this as its thrown when the stream is stopped
        }

    }
}
