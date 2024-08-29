using System.Device.Gpio;
using System.Text.RegularExpressions;
using Iot.Device.CharacterLcd;
using RaspSensorLibrary;
using SynapseVue.Server.Models.Products;

namespace SynapseVue.Server.Services;

public class MainMotionDetectionService : BackgroundService
{
    private readonly ILogger<MainMotionDetectionService> _logger;
    private readonly IDbContextFactory<AppDbContext> _dbContextFactory;
    private readonly VideoRecorderService _videoRecorderService;
    private CancellationTokenSource _cts;
    private bool _isRunning;
    private GpioController _controller;
    private Product LED;
    private Product PIR;
    private Product DHT22;
    private Product BUZZ;
    private Product RFID;
    private Product Display;
    private Product AICamera;
    private string _mode = ""; 
    string brelock = "E3E4E4A6";
    string card = "A3E12396";
    private bool _isRecordingVideo = false;

    public MainMotionDetectionService(
        ILogger<MainMotionDetectionService> logger,
        IDbContextFactory<AppDbContext> dbContextFactory,
        VideoRecorderService videoRecorderService)
    {
        _logger = logger;
        _dbContextFactory = dbContextFactory;
        _videoRecorderService = videoRecorderService;
        _cts = new CancellationTokenSource();
        _isRunning = true;

        FindProperDevicesForSafetyControlMain();
    }

    public void StopMonitoring()
    {
        if (!_isRunning) return;
        _mode = "Home";
        _cts.Cancel();
        _isRunning = false;
        _logger.LogInformation("Motion detection stopped.");
    }

    public void StartMonitoring()
    {
        if (_isRunning) return;
        _mode = "Safe";
        _cts = new CancellationTokenSource();
        _isRunning = true;
        _logger.LogInformation("Motion detection started.");
    }

    private void FindProperDevicesForSafetyControlMain()
    {
        using (var context = _dbContextFactory.CreateDbContext())
        {
            var products = context.Products.ToList();
            foreach (var product in products)
            {
                if (!product.Name.Contains("-main")) continue;

                switch (product.CategoryId)
                {
                    case 1: // DHT22
                        DHT22 = product;
                        break;
                    case 3: // PIR
                        PIR = product;
                        break;
                    case 4: // LED
                        LED = product;
                        break;
                    case 5: // Display
                        Display = product;
                        break;
                    case 6: // BUZZER
                        BUZZ = product;
                        break;
                    case 8: // RFID
                        RFID = product;
                        break;
                }

                if (product.Name.Contains("AI-main"))
                {
                    AICamera = product;
                }
            }

            var state = context.SystemStates.First(x=>x.Property == "Mode");
            _mode = state.Value;

            if (state.Value == "Home")
            {
                StopMonitoring();
            }
        }
    }

    protected override async Task ExecuteAsync(CancellationToken stoppingToken)
    {
        _controller = new GpioController();
        if(LED != null)
        {
            _controller.OpenPin(LED.PIN, PinMode.Output);
        }
        if(BUZZ != null)
        {
            _controller.OpenPin(BUZZ.PIN, PinMode.Output);
        }
        if(PIR == null)
        {
            _logger.LogError("PIR sensor not found. Exiting...");
            return;
        }
        if(RFID == null)
        {
            _logger.LogError("RFID sensor not found. Exiting...");
            return;
        }
        RfidReader rfidReader = new RfidReader();
        rfidReader.Setup(RFID.PIN);
        _controller.OpenPin(PIR.PIN, PinMode.Input);
        _controller.RegisterCallbackForPinValueChangedEvent(PIR.PIN, PinEventTypes.Rising, OnMotionDetected);
        _controller.RegisterCallbackForPinValueChangedEvent(PIR.PIN, PinEventTypes.Falling, OnMotionEnded);

        while (!stoppingToken.IsCancellationRequested)
        {
            if(Display != null)
            {
                HandleDisplay();
            }
            HandleRfid(rfidReader);
            try
            {
                await Task.Delay(1000, _cts.Token); // Delay using the token to allow for cancellation
            }
            catch (TaskCanceledException)
            {
                if (stoppingToken.IsCancellationRequested)
                {
                    break; // Exit the loop if the service is stopping
                }
                await Task.Delay(1000); // Delay a bit to avoid a tight loop if cancellation token is set
            }
        }
        _controller.Dispose();
    }

    private void HandleRfid(RfidReader rfidReader)
    {
        string rfidData = rfidReader.ReadData();
        if (rfidData != null)
        {
            if (rfidData == brelock)
            {
                _logger.LogInformation("Brelock detected.");
                ChangeMode(RFID);
            }
            else if (rfidData == card)
            {
                _logger.LogInformation("Card detected.");
                ChangeMode(RFID);
            }
            else
            {
                _logger.LogInformation($"Unknown RFID detected: {rfidData}");
            }
        }
    }

    private void ChangeMode(Product RFID)
    {
        //chand mode in database and in service
        using (var context = _dbContextFactory.CreateDbContext())
        {
            var state = context.SystemStates.First(x=> x.Property == "Mode");
            if (state.Value == "Home")
            {
                StartMonitoring();
                state.Value = "Safe";
            }
            else
            {
                StopMonitoring();
                state.Value = "Home";
            }
            RFID.LastReadValue = $"Mode changed to: {state.Value}";
            RFID.LastSuccessActivity = DateTime.Now;
            context.Entry(RFID).State = EntityState.Modified;
            context.SaveChanges();
            _mode = state.Value;
        }
    }

    private void HandleDisplay()
    {
        Product product = null;
        using (var context = _dbContextFactory.CreateDbContext())
        {
            product = context.Products.FirstOrDefault(p => p.Name.Contains("-main") && p.CategoryId == 1);
        }

        var text1 = DateTime.Now.ToString();
        TryDisplayTextOnLCD(text1, 0);

        var text2 = $"Mode: {_mode}";
        if (product != null && product.LastReadValue != null)
        {
            Regex regex = new Regex(@"\d+\.?\d*C");
            Match match = regex.Match(product.LastReadValue);
            if (match.Success)
            {
                string temperature = match.Value;
                text2 = $"Mode: {_mode} {temperature}";
            }
        }
        TryDisplayTextOnLCD(text2, 1);
    }

    private void TryDisplayTextOnLCD(string text, int line)
    {
        try
        {
            if (!I2C_LCD1602.Instance.WriteLine(text, line))
            {
                throw new Exception("Write operation failed.");
            }
        }
        catch (Exception ex)
        {
            _logger.LogError($"Exception caught in main loop: {ex.Message}");
        }
    }

    private void OnMotionDetected(object sender, PinValueChangedEventArgs args)
    {
        if (_cts.Token.IsCancellationRequested) return;

        if (AICamera != null && !_videoRecorderService.IsRecordingVideo)
        {
            Task.Run(() => _videoRecorderService.Record());
        }
        if (LED != null)
        {
            _controller.Write(LED.PIN, PinValue.High);
        }
        if (BUZZ != null)
        {
            _controller.Write(BUZZ.PIN, PinValue.High);
        }
        _logger.LogInformation("Motion detected: LED and BUZZER are ON");
    }

    private void OnMotionEnded(object sender, PinValueChangedEventArgs args)
    {
        if (_cts.Token.IsCancellationRequested) return;

        if (LED != null)
        {
            _controller.Write(LED.PIN, PinValue.Low);
        }
        if (BUZZ != null)
        {
            _controller.Write(BUZZ.PIN, PinValue.Low);
        }
        _logger.LogInformation("Motion ended: LED and BUZZER are OFF");
    }
}

