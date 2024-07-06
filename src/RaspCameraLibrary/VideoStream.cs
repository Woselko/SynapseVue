using SynapseVue.RaspCameraLibrary.Settings;
using SynapseVue.RaspCameraLibrary.Settings.Types;
using System.Diagnostics;

namespace SynapseVue.RaspCameraLibrary
{
    /// <summary>
    /// libcamera-vid Wrapper
    /// </summary>
    public class VideoStream : Hello
    {
        /// <summary>
        /// Binary name
        /// </summary>
        protected override string Executable => "libcamera-vid";

        /// <summary>
        /// New frame received event
        /// </summary>
        public event Action<byte[]> NewImageReceived;

        /// <summary>
        /// New Video info received event
        /// </summary>
        public event Action<string> VideoInfoReceived;

        /// <summary>
        /// New VideoStream instance
        /// </summary>
        private static VideoStream Instance { get; } = new VideoStream();

        /// <summary>
        /// Generate a ProcessStartInfo for libcamera-vid
        /// </summary>
        public static ProcessStartInfo CaptureStartInfo(VideoSettings settings) => Instance.StartInfo(settings);

        /// <summary>
        /// List cameras
        /// </summary>
        public static new async Task<List<Camera>?> ListCameras() => await Instance.List();

        /// <summary>
        /// Actual process running libcamera-vid
        /// </summary>
        public static Process CaptureStreamProcess { get; set; }

        /// <summary>
        /// Start Frame reader
        /// </summary>
        public async Task StartFrameReaderAsync(
            ProcessStartInfo processStartInfo,
            CancellationToken cancellationToken = default,
            bool useShellExecute = false)
        {
            ConfigureProcessStartInfo(processStartInfo, useShellExecute);

            using (CaptureStreamProcess = StartProcess(processStartInfo))
            {
                using var frameOutputStream = CaptureStreamProcess.StandardOutput.BaseStream;
                var buffer = new byte[65536];
                var imageData = new List<byte>();

                while (!cancellationToken.IsCancellationRequested)
                {
                    if (CaptureStreamProcess.HasExited) break;

                    var length = await frameOutputStream.ReadAsync(buffer, 0, buffer.Length);
                    if (length == 0) break;

                    imageData.AddRange(buffer.Take(length));
                    if (imageData.Count > 0)
                    {
                        ProcessImageData(imageData);
                        imageData.Clear();
                    }
                }

                CleanupProcess();
            }
        }

        /// <summary>
        /// Start Frame reader
        /// </summary>
        public async Task StartVideoStream(
            ProcessStartInfo processStartInfo,
            CancellationToken cancellationToken = default,
            bool useShellExecute = false)
        {
            ConfigureProcessStartInfo(processStartInfo, useShellExecute);
            using (CaptureStreamProcess = StartProcess(processStartInfo))
            {
                await Task.Run(() => WaitForCancellation(cancellationToken));
            }
        }

        /// <summary>
        /// Stop the video stream
        /// </summary>
        public async Task StopVideoStream()
        {
            if (CaptureStreamProcess != null && !CaptureStreamProcess.HasExited)
            {
                CaptureStreamProcess.Kill();
            }
        }

        private void ProcessDataReceived(object sender, DataReceivedEventArgs e)
        {
            Console.WriteLine(e.Data);
        }

        private void ConfigureProcessStartInfo(ProcessStartInfo processStartInfo, bool useShellExecute)
        {
            var args = processStartInfo.Arguments;
            args += " --libav-format mjpeg";
            processStartInfo.Arguments = args;
            processStartInfo.UseShellExecute = useShellExecute;
            processStartInfo.RedirectStandardOutput = true;
        }

        private Process StartProcess(ProcessStartInfo processStartInfo)
        {
            var process = new Process
            {
                StartInfo = processStartInfo,
            };
            process.ErrorDataReceived += ProcessDataReceived;
            process.Start();
            process.BeginErrorReadLine();
            return process;
        }

        private void WaitForCancellation(CancellationToken cancellationToken)
        {
            while (!cancellationToken.IsCancellationRequested)
            {
                Task.Delay(100).Wait(); // Delay to prevent busy-waiting
            }

            if (!CaptureStreamProcess.HasExited)
            {
                CaptureStreamProcess.Kill();
            }
        }

        private void CleanupProcess()
        {
            CaptureStreamProcess.ErrorDataReceived -= ProcessDataReceived;
            CaptureStreamProcess.WaitForExit(1000);
            if (!CaptureStreamProcess.HasExited)
            {
                CaptureStreamProcess.Kill();
            }
        }

        private void ProcessImageData(List<byte> imageData)
        {
            var videoBytes = imageData.ToArray();
            int frameStartIndex = -1;

            for (int i = 0; i < videoBytes.Length - 1; i++)
            {
                if (videoBytes[i] == 0xFF && videoBytes[i + 1] == 0xD8)
                {
                    if (frameStartIndex != -1)
                    {
                        byte[] frame = new byte[i - frameStartIndex];
                        Array.Copy(videoBytes, frameStartIndex, frame, 0, i - frameStartIndex);
                        NewImageReceived?.Invoke(frame);
                    }
                    frameStartIndex = i;
                }
                else if (videoBytes[i] == 0xFF && videoBytes[i + 1] == 0xD9)
                {
                    if (frameStartIndex != -1)
                    {
                        byte[] frame = new byte[i - frameStartIndex + 2];
                        Array.Copy(videoBytes, frameStartIndex, frame, 0, i - frameStartIndex + 2);
                        NewImageReceived?.Invoke(frame);
                        frameStartIndex = -1;
                    }
                }
            }
        }

        public List<byte[]> ExtractJPEGFrames(byte[] videoBytes)
        {
            List<byte[]> frames = new List<byte[]>();
            int frameStartIndex = -1;
            for (int i = 0; i < videoBytes.Length - 1; i++)
            {
                if (videoBytes[i] == 0xFF && videoBytes[i + 1] == 0xD8)
                {
                    if (frameStartIndex != -1)
                    {
                        byte[] frame = new byte[i - frameStartIndex];
                        Array.Copy(videoBytes, frameStartIndex, frame, 0, i - frameStartIndex);
                        frames.Add(frame);
                    }
                    frameStartIndex = i;
                }
                else if (videoBytes[i] == 0xFF && videoBytes[i + 1] == 0xD9)
                {
                    if (frameStartIndex != -1)
                    {
                        byte[] frame = new byte[i - frameStartIndex + 2];
                        Array.Copy(videoBytes, frameStartIndex, frame, 0, i - frameStartIndex + 2);
                        frames.Add(frame);
                        frameStartIndex = -1;
                    }
                }
            }
            return frames;
        }
    }
}
