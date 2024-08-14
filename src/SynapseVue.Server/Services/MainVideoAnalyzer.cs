using System;
using System.Collections.Generic;
using RaspSensorLibrary;
using SynapseVue.Server;
using SynapseVue.Server.Models.Categories;
using SynapseVue.Server.Models.Media;
using SynapseVue.Server.Models.Products;
using Emgu.CV.Dnn;
using Emgu.CV.Structure;
using Emgu.CV.Util;
using Emgu.CV;
using System.Diagnostics;
using FluentEmail.Core;

namespace SynapseVue.Server.Services;

public partial class MainVideoAnalyzer
{
    private readonly IDbContextFactory<AppDbContext> _dbContextFactory;
    private readonly IServiceProvider _serviceProvider;
    private readonly ILogger<MainVideoAnalyzer> _logger;
    private List<Video> _videos;
    private bool _isProcessing = false;

    public MainVideoAnalyzer(IDbContextFactory<AppDbContext> dbContextFactory, IServiceProvider serviceProvider, ILogger<MainVideoAnalyzer> logger)
    {
        _dbContextFactory = dbContextFactory;
        _serviceProvider = serviceProvider;
        _logger = logger;
    }

    public void ProcessVideo()
    { 
        if (_isProcessing)
        {
            _logger.LogInformation("Is actively processing.");
            return;
        }
    
        using (var context = _dbContextFactory.CreateDbContext())
        {
            _videos = context.Videos.Where(x => x.IsProcessed == false).ToList();
        }
    
        if (_videos.Count == 0)
        {
            _logger.LogInformation("No video to process.");
            return;
        }
    
        _isProcessing = true;
        var vidToAnalyze = _videos.Last();
        try
        {
            AnalyzeVideo(ref vidToAnalyze);
            ConvertAviToMp4(ref vidToAnalyze);
            SaveToDatabase(ref vidToAnalyze);
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Fail during processing video");
            throw;
        }
        finally
        {
            _isProcessing = false;
        }
    }
    
    private void AnalyzeVideo(ref Video vidToAnalyze)
    {
        vidToAnalyze.DetectedObjects = "";
        var listOfObjects = new List<string>();
        var net = Emgu.CV.Dnn.DnnInvoke.ReadNetFromDarknet("yolov3.cfg", "yolov3.weights");
        var classLabels = File.ReadAllLines("coco.names");
        net.SetPreferableBackend(Emgu.CV.Dnn.Backend.OpenCV);
        net.SetPreferableTarget(Emgu.CV.Dnn.Target.Cpu);
    
        if (!File.Exists(vidToAnalyze.FilePath))
        {
            _logger.LogWarning("Video file not found: {FilePath}", vidToAnalyze.FilePath);
            return;
        }
    
        VideoCapture vc = new VideoCapture(vidToAnalyze.FilePath);
        Mat frame = new();
        VectorOfMat output = new();
    
        while (true)
        {
            if (!vc.Read(frame) || frame.IsEmpty)
            {
                // If there are no more frames, break out of the loop
                break;
            }
    
            CvInvoke.Resize(frame, frame, new System.Drawing.Size(0, 0), .4, .4);
    
            var image = frame.ToImage<Bgr, byte>();
            var input = DnnInvoke.BlobFromImage(image, 1 / 255.0, swapRB: true);
            net.SetInput(input);
            net.Forward(output, net.UnconnectedOutLayersNames);
    
            for (int i = 0; i < output.Size; i++)
            {
                var mat = output[i];
                var data = (float[,])mat.GetData();
    
                for (int j = 0; j < data.GetLength(0); j++)
                {
                    float[] row = Enumerable.Range(0, data.GetLength(1))
                                  .Select(x => data[j, x])
                                  .ToArray();
    
                    var rowScore = row.Skip(5).ToArray();
                    var classId = rowScore.ToList().IndexOf(rowScore.Max());
                    var confidence = rowScore[classId];
    
                    if (confidence > 0.8f)
                    {
                        var text = classLabels[classId];
                        if (!listOfObjects.Contains(text))
                        {
                            vidToAnalyze.DetectedObjects += text + ", ";
                            listOfObjects.Add(text);
                            if(text == "person")
                            {
                                vidToAnalyze.IsPersonDetected = true;
                                SendEmail(ref vidToAnalyze);
                            }
                        }
                    }
                }
            }
        }
    }
    
    private void SendEmail(ref Video vidToAnalyze)
    {
        using (var context = _dbContextFactory.CreateDbContext())
        {
            var users = context.Users.ToList();
    
            if (users == null || users.Count == 0)
            {
                _logger.LogInformation("No users found to send emails.");
                return;
            }
    
            var body = $"Person detected in video: {vidToAnalyze.Name}, probably is processed and saved in .mp4 format. \r\n Detected objects: {vidToAnalyze.DetectedObjects} \r\n created at: {vidToAnalyze.CreatedAt}";
    
            foreach (var user in users)
            {
                FluentEmail.Core.Models.SendResponse result = null;
                try
                {
                    using (var scope = _serviceProvider.CreateScope())
                    {
                        IFluentEmail fluentEmail = scope.ServiceProvider.GetRequiredService<IFluentEmail>();
                        result = fluentEmail
                            .To(user.Email, user.UserName)
                            .Subject(vidToAnalyze.Name + " - person detected!! " + vidToAnalyze.CreatedAt)
                            .Body(body, isHtml: true)
                            .SendAsync().Result;
                    }
    
                    if (result.Successful)
                    {
                        _logger.LogInformation($"Email sent to {user.Email}");
                    }
                    else
                    {
                        _logger.LogWarning($"Failed to send email to {user.Email}: {result.ErrorMessages}");
                    }
                }
                catch (Exception ex)
                {
                    _logger.LogError(ex, $"Exception while sending email to {user.Email}");
                }
            }
        }
    }
    
    private void ConvertAviToMp4(ref Video vidToAnalyze)
    {
        string inputFilePath = vidToAnalyze.FilePath;
        string outputFilePath = Path.ChangeExtension(inputFilePath, ".mp4");
    
        try
        {
            var startInfo = new ProcessStartInfo
            {
                FileName = "ffmpeg",
                Arguments = $"-i \"{inputFilePath}\" \"{outputFilePath}\"",
                RedirectStandardOutput = true,
                RedirectStandardError = true,
                UseShellExecute = false,
                CreateNoWindow = true
            };
    
            using (var process = new Process { StartInfo = startInfo })
            {
                process.Start();
                process.WaitForExit();
    
                if (process.ExitCode != 0)
                {
                    string error = process.StandardError.ReadToEnd();
                    _logger.LogError($"ffmpeg failed with error: {error}");
                    throw new Exception("ffmpeg conversion failed");
                }
            }
    
            if (File.Exists(outputFilePath))
            {
                File.Delete(inputFilePath);
                vidToAnalyze.FilePath = outputFilePath;
            }
            else
            {
                throw new Exception("Output file was not created");
            }
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, $"Error during video conversion: {ex.Message}");
            throw;
        }
    }
    
    private void SaveToDatabase(ref Video video)
    {
        using (var context = _dbContextFactory.CreateDbContext())
        {
            video.IsProcessed = true;
            video.Name = video.Name.Replace(".avi", ".mp4");
            video.FileSize = new FileInfo(video.FilePath).Length;
            context.Entry(video).State = EntityState.Modified;
            context.SaveChanges();
        }
    }

}

    // private void AnalyzeVideo(ref Video vidToAnalyze)
    // {
    //     var listOfObjects = new List<string>();
    //     var net = Emgu.CV.Dnn.DnnInvoke.ReadNetFromDarknet("yolov3.cfg", "yolov3.weights");
    //     var classLabels = File.ReadAllLines("coco.names");
    //     net.SetPreferableBackend(Emgu.CV.Dnn.Backend.OpenCV);
    //     net.SetPreferableTarget(Emgu.CV.Dnn.Target.Cpu);
    //     if(!File.Exists(vidToAnalyze.FilePath))
    //     {
    //         Console.WriteLine("Video file not found.");
    //         return;
    //     }

    //     VideoCapture vc = new VideoCapture(vidToAnalyze.FilePath);
    //     Mat frame = new();
    //     VectorOfMat output = new();
    //     VectorOfRect boxes = new();
    //     VectorOfFloat scores = new();
    //     VectorOfInt indices = new();
    //     while (true)
    //     {
    //         if (!vc.Read(frame) || frame.IsEmpty)
    //         {
    //             // If there are no more frames, break out of the loop
    //             break;
    //         }
    //         CvInvoke.Resize(frame, frame, new System.Drawing.Size(0, 0), .4, .4);
    //         boxes = new();
    //         indices = new();
    //         scores = new();
    //         var image = frame.ToImage<Bgr, byte>();
    //         var input = DnnInvoke.BlobFromImage(image, 1 / 255.0, swapRB: true);
    //         net.SetInput(input);
    //         net.Forward(output, net.UnconnectedOutLayersNames);
    //         for (int i = 0; i < output.Size; i++)
    //         {
    //             var mat = output[i];
    //             var data = (float[,])mat.GetData();
    //             for (int j = 0; j < data.GetLength(0); j++)
    //             {
    //                 float[] row = Enumerable.Range(0, data.GetLength(1))
    //                               .Select(x => data[j, x])
    //                               .ToArray();
    //                 var rowScore = row.Skip(5).ToArray();
    //                 var classId = rowScore.ToList().IndexOf(rowScore.Max());
    //                 var confidence = rowScore[classId];
    //                 if (confidence > 0.8f)
    //                 {
    //                     var centerX = (int)(row[0] * frame.Width);
    //                     var centerY = (int)(row[1] * frame.Height);
    //                     var boxWidth = (int)(row[2] * frame.Width);
    //                     var boxHeight = (int)(row[3] * frame.Height);
    //                     var x = (int)(centerX - (boxWidth / 2));
    //                     var y = (int)(centerY - (boxHeight / 2));
    //                     boxes.Push(new System.Drawing.Rectangle[] { new System.Drawing.Rectangle(x, y, boxWidth, boxHeight) });
    //                     indices.Push(new int[] { classId });
    //                     scores.Push(new float[] { confidence });
    //                 }
    //             }
    //         }
    //         var bestIndex = DnnInvoke.NMSBoxes(boxes.ToArray(), scores.ToArray(), .8f, .8f);
    //         for (int i = 0; i < bestIndex.Length; i++)
    //         {
    //             int index = bestIndex[i];
    //             var box = boxes[index];
    //             var text = classLabels[indices[index]];
    //             if(!listOfObjects.Contains(text))
    //             {
    //                 vidToAnalyze.DetectedObjects += text + ", ";
    //                 listOfObjects.Add(text);
    //             }
    //         }
    //     }
    // }


