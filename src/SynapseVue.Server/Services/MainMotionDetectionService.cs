using System.Device.Gpio;
using System.Text.RegularExpressions;
using Iot.Device.CharacterLcd;
using RaspSensorLibrary;
using SynapseVue.Server.Models.Products;

namespace SynapseVue.Server.Services;

public class MainMotionDetectionService : BackgroundService
{
    private readonly ILogger<MainMotionDetectionService> _logger;
    private readonly IServiceProvider _serviceProvider;
    private GpioController _controller;
    private CancellationTokenSource _cts;
    private bool _isRunning;
    private Product LED;
    private Product PIR;
    private Product DHT22;
    private Product BUZZ;
    private Product Display;
    private string mode=""; 

    public MainMotionDetectionService(ILogger<MainMotionDetectionService> logger, IServiceProvider serviceProvider)
    {
        _logger = logger;
        _serviceProvider = serviceProvider;
        _cts = new CancellationTokenSource();
        _isRunning = true;
        FindProperDevicesForSafetyControlMain();
    }

    public void StopMonitoring()
    {
        if (!_isRunning) return;
        this.mode = "Home";
        _cts.Cancel();
        _isRunning = false;
        _logger.LogInformation("Motion detection stopped.");
    }

    public void StartMonitoring()
    {
        if (_isRunning) return;
        this.mode = "Safe";
        _cts = new CancellationTokenSource();
        _isRunning = true;
        _logger.LogInformation("Motion detection started.");
    }

    private void FindProperDevicesForSafetyControlMain()
    {
        using (var scope = _serviceProvider.CreateScope())
        {
            var _dbContext = scope.ServiceProvider.GetRequiredService<AppDbContext>();
            var products = _dbContext.Products.ToList();
            foreach (var product in products)
            {
                if(!product.Name.Contains("-main"))//-main
                {
                    continue;
                }
                if (product.CategoryId == 1)//DHT22
                {
                    DHT22 = product;
                }
                if (product.CategoryId == 3)//PIR
                {
                    PIR = product;
                }
                if (product.CategoryId == 4)//LED
                {
                    LED = product;
                }
                if (product.CategoryId == 5)//PIR
                {
                    Display = product;
                }
                if (product.CategoryId == 6)//BUZZER
                {
                    BUZZ= product;
                }
            }
            var state = _dbContext.SystemStates.First();
            this.mode = state.Mode;
            if(state.Mode == "Home")
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
        _controller.OpenPin(PIR.PIN, PinMode.Input);
        _controller.RegisterCallbackForPinValueChangedEvent(PIR.PIN, PinEventTypes.Rising, OnMotionDetected);
        _controller.RegisterCallbackForPinValueChangedEvent(PIR.PIN, PinEventTypes.Falling, OnMotionEnded);

        while (!stoppingToken.IsCancellationRequested)
        {
            if(Display != null)
            {
                HandleDisplay();
            }

            try
            {
                await Task.Delay(2000, _cts.Token); // Delay using the token to allow for cancellation
            }
            catch (TaskCanceledException)
            {
                if (stoppingToken.IsCancellationRequested)
                {
                    break; // Exit the loop if the service is stopping
                }
                await Task.Delay(2000); // Delay a bit to avoid a tight loop if cancellation token is set
            }
        }
        _controller.Dispose();
    }

    private void HandleDisplay()
    {
        using (var scope = _serviceProvider.CreateScope())
        {
            var _dbContext = scope.ServiceProvider.GetRequiredService<AppDbContext>();
            var product = _dbContext.Products.Where(p => p.Name.Contains("-main") && p.CategoryId == 1).First();
            var text1 = DateTime.Now.ToString();
            TryDisplayTextOnLCD(text1, 0);
            var text2 = $"Mode: {mode}";
            if(product != null && product.LastReadValue != null)
            {
                Regex regex = new Regex(@"\d+\.?\d*C");
                Match match = regex.Match(product?.LastReadValue);

                if (match.Success)
                {
                    string temperature = match.Value;
                    text2 = $"Mode: {mode} {temperature}";
                    TryDisplayTextOnLCD(text2, 1);
                }
                else
                    TryDisplayTextOnLCD(text2, 1);
            }
            else
            {
                TryDisplayTextOnLCD(text2, 1);
            }
            Display.LastReadValue = text1 + " " + text2;
            _dbContext.Entry(Display).State = EntityState.Modified;
            _dbContext.SaveChanges();
        }
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
            Console.WriteLine($"Exception caught in main loop: {ex.Message}");
        }
    }

    private void OnMotionDetected(object sender, PinValueChangedEventArgs args)
    {
        if (_cts.Token.IsCancellationRequested) return;
        if(LED != null)
        {
            _controller.Write(LED.PIN, PinValue.High);
        }
        if(BUZZ != null)
        {
            _controller.Write(BUZZ.PIN, PinValue.High);
        }
        _logger.LogInformation("Motion detected: LED and BUZZER are ON");
    }

    private void OnMotionEnded(object sender, PinValueChangedEventArgs args)
    {
        if (_cts.Token.IsCancellationRequested) return;

        var time = DateTime.Now;
        using (var scope = _serviceProvider.CreateScope())
        {
            var _dbContext = scope.ServiceProvider.GetRequiredService<AppDbContext>();
            if(LED != null)
            {
                LED.LastSuccessActivity = time;
                _dbContext.Entry(LED).State = EntityState.Modified;
                _controller.Write(LED.PIN, PinValue.Low);
            }
            if(BUZZ != null)
            {
                BUZZ.LastSuccessActivity = time;
                _dbContext.Entry(BUZZ).State = EntityState.Modified;
                _controller.Write(BUZZ.PIN, PinValue.Low);
            }
            _logger.LogInformation("Motion ended: LED and BUZZER are OFF");
            PIR.LastReadValue = "Detected!!!";
            PIR.LastSuccessActivity = time;
            _dbContext.Entry(PIR).State = EntityState.Modified;
            _dbContext.SaveChanges();
        }
    }
}
