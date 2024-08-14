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
    private readonly IDbContextFactory<AppDbContext> _dbContextFactory;
    private bool _isRecordingVideo = false;

    public VideoRecorderService(IDbContextFactory<AppDbContext> dbContextFactory)
    {
        _dbContextFactory = dbContextFactory;
    }

    public bool IsRecordingVideo => _isRecordingVideo;

    public async Task Record(int seconds = 15)
    {
        if (_isRecordingVideo)
        {
            return;
        }

        string videoName = $"MotionDetect_{DateTime.Now:ddMMyyyy_HHmm}.avi";
        var path = Path.Combine(Directory.GetCurrentDirectory(), "wwwroot", "videos", videoName);

        var video = new Video
        {
            Name = videoName,
            IsPersonDetected = false,
            DetectedObjects = "nothing",
            IsProcessed = false,
            FilePath = path,
            CreatedAt = DateTimeOffset.UtcNow,
            FileSize = 0
        };

        var settings = new H264
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

        _isRecordingVideo = true;

        var captureStartInfo = RaspCameraLibrary.Video.CaptureStartInfo(settings);
        Process? captureProcess = null;

        try
        {
            captureProcess = Process.Start(captureStartInfo);
        }
        catch (Exception ex)
        {
            Console.WriteLine(ex.Message);
            _isRecordingVideo = false;
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
            if (captureProcess != null && !captureProcess.HasExited)
            {
                captureProcess.Kill();
            }

            _isRecordingVideo = false;

            if (File.Exists(video.FilePath))
            {
                video.FileSize = new FileInfo(video.FilePath).Length;
                using (var context = _dbContextFactory.CreateDbContext())
                {
                    context.Videos.Add(video);
                    context.SaveChanges();
                }
            }
        }
    }
}

