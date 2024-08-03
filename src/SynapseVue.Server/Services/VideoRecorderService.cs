using System;
using System.Collections.Generic;
using RaspSensorLibrary;
using SynapseVue.Server;
using SynapseVue.Server.Models.Categories;
using SynapseVue.Server.Models.Products;
using System;
using System.Threading;
using System.Threading.Tasks;
using SynapseVue.RaspCameraLibrary.Settings;
using SynapseVue.RaspCameraLibrary.Settings.Enumerations;
using SynapseVue.RaspCameraLibrary.Settings.Codecs;
using System.Diagnostics;
using SynapseVue.Server.Models.Media;


namespace SynapseVue.Server.Services;

public class VideoRecorderService
{
    private static readonly Lazy<VideoRecorderService> _instance = new Lazy<VideoRecorderService>(() => new VideoRecorderService());
    private bool isRecordingVideo = false;
    private IServiceProvider _serviceProvider;

    private VideoRecorderService() { }

    public static VideoRecorderService Instance => _instance.Value;

    public void Initialize(IServiceProvider serviceProvider)
    {
        _serviceProvider = serviceProvider;
    }

    public bool IsRecordingVideo => isRecordingVideo;

    public async Task Record(int seconds = 15)
    {
        if (isRecordingVideo)
        {
            return;
        }
        string videoName = $"MotionDetect_{DateTime.Now:ddMMyyyy_HHmm}.avi";
        var path = Path.Combine(Directory.GetCurrentDirectory(), "wwwroot", "videos", videoName);
        Video video = new Video(){
            Name = videoName,
            IsPersonDetected = false,
            DetectedObjects = "nothing",
            IsProcessed = false,
            FilePath = path,
            CreatedAt = DateTimeOffset.UtcNow,
            FileSize = 0
        };

        VideoSettings Settings = new H264()
        {
            Camera = 0,
            Width = 1280,
            Height = 720,
            Timeout = 0,
            HFlip = false,
            VFlip = false,
            WhiteBalance = WhiteBalance.Incandescent,
            Output = path,
            Framerate = 15,
        };

        isRecordingVideo = true;
        ProcessStartInfo CaptureStartInfo = RaspCameraLibrary.Video.CaptureStartInfo(Settings);
        Process? CaptureProcess = null;
        try
        {
            CaptureProcess = Process.Start(CaptureStartInfo);
        }
        catch (Exception ex)
        {
            Console.WriteLine(ex.Message);
            isRecordingVideo = false;
            return;
        }
        
        try
        {
            await Task.Delay(TimeSpan.FromSeconds(seconds));
        }
        catch (TaskCanceledException)
        {
            // Handle cancellation if needed
        }
        finally
        {
            CaptureProcess.Kill();
            isRecordingVideo = false;
            if(File.Exists(video.FilePath)){
                video.FileSize = new FileInfo(video.FilePath).Length;
                using (var scope = _serviceProvider.CreateScope())
                {
                    var _dbContext = scope.ServiceProvider.GetRequiredService<AppDbContext>();
                    _dbContext.Videos.Add(video);
                    _dbContext.SaveChanges();
                }
            }
        }
    }
}
