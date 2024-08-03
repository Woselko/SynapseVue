using Microsoft.AspNetCore.Components.WebAssembly.Services;
using SynapseVue.Client.Core.Controllers.Media;
using SynapseVue.Client.Core.Controllers.VideoStream;
using SynapseVue.Shared.Dtos.Media;

namespace SynapseVue.Client.Core.Components.Pages.VideoStream;

[Authorize]
public partial class VideoStreamPage
{
    [AutoInject] IVideoStreamController systemController = default!;
    [AutoInject] static IPhotoController photoController = default!;
    private static bool isStreaming = false;
    private static string videoSrc = string.Empty;
    private Task streamTask;
    private static string capturedImage = string.Empty;
    private CancellationTokenSource cancellationTokenSource = new CancellationTokenSource();

    public bool  isLoading = true;
    protected override async Task OnInitAsync()
    {
        isLoading = false;
        await base.OnInitAsync();
    }
    private async Task SaveImage()
    {
        await JSRuntime.InvokeVoidAsync("saveImage");
        await Task.Delay(5000);
        StateHasChanged();
    }

    [JSInvokable]
    public static async Task ReceiveImage(string base64Image)
    {
        capturedImage = base64Image;
        var base64Data = capturedImage.Split(',')[1]; // Usunięcie prefiksu
        var imageBytes = Convert.FromBase64String(base64Data);
        PhotoDto photo = new PhotoDto
        {
            Name = $"Photo_{DateTime.Now:ddMMyyyy_HHmm}",
            Data = imageBytes,
            Description = "Photo description",
            CreatedAt = DateTimeOffset.Now
        };
        await photoController.Create(photo);
    }

    private async Task CaptureImage()
    {
        await JSRuntime.InvokeVoidAsync("captureImage");
    }
    private async Task StartStream()
    {
        isStreaming = true; 
        //streamTask = FetchVideoStream();
        //await Http.GetAsync("http://localhost:6030/api/videostream/get"); // Ustaw URL endpointa zatrzymania strumieniowania
        videoSrc = "http://localhost:6030/api/videostream/get"; // Ustaw URL endpointa strumieniowania  
        //await Http.GetAsync("http://10.0.2.2:6030/api/videostream/get"); // Ustaw URL endpointa zatrzymania strumieniowania
        //videoSrc = "http://10.0.2.2:6030/api/videostream/get";
        StateHasChanged();
    }

    private async Task StopStream()
    {
        isStreaming = false;
        //cancellationTokenSource.Cancel();
        await Http.GetAsync("http://localhost:6030/api/videostream/stop"); // Ustaw URL endpointa zatrzymania strumieniowania
        //await Http.GetAsync("http://10.0.2.2:6030/api/videostream/stop"); // Ustaw URL endpointa zatrzymania strumieniowania
        videoSrc = string.Empty;
        StateHasChanged();
    }

    private async Task FetchVideoStream()
    {
        var request = new HttpRequestMessage(HttpMethod.Get, "http://localhost:6030/api/videostream/get");
        request.Headers.Add("Accept", "multipart/x-mixed-replace; boundary=--frame");

        try
        {
            using var response = await Http.SendAsync(request, HttpCompletionOption.ResponseHeadersRead, cancellationTokenSource.Token);

            response.EnsureSuccessStatusCode();

            var stream = await response.Content.ReadAsStreamAsync(cancellationTokenSource.Token);
            var buffer = new byte[1024 * 512]; // Adjust buffer size as needed

            while (isStreaming && !cancellationTokenSource.Token.IsCancellationRequested)
            {
                var imageBuffer = new List<byte>();
                var bytesRead = await stream.ReadAsync(buffer, 0, buffer.Length, cancellationTokenSource.Token);
                if (bytesRead == 0)
                {
                    break;
                }

                imageBuffer.AddRange(buffer.Take(bytesRead));
                //var imageStart = imageBuffer.IndexOf((byte)0xFF);
                //var imageEnd = imageBuffer.IndexOf((byte)0xD9, imageStart) + 1;

                //if (imageStart != -1 && imageEnd != -1 && imageEnd > imageStart)
                //{
                    //var imageData = imageBuffer.Skip(imageStart).Take(imageEnd - imageStart).ToArray();
                    //var imageUrl = "data:image/jpeg;base64," + Convert.ToBase64String(imageData);
                    //videoSrc = imageUrl;
                    StateHasChanged();
                    //imageBuffer = imageBuffer.Skip(imageEnd).ToList();
                //}
            }
        }
        catch (TaskCanceledException)
        {
            // Handle task cancellation
        }
        catch (Exception ex)
        {
            System.Console.WriteLine($"Error fetching video stream: {ex.Message}");
        }
    }
}
