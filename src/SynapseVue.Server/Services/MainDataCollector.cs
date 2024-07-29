using System;
using System.Collections.Generic;
using RaspSensorLibrary;
using SynapseVue.Server;
using SynapseVue.Server.Models.Categories;
using SynapseVue.Server.Models.Products;

namespace SynapseVue.Server.Services;

public class MainDataCollector
{
    public static MainDataCollector Instance => _instance.Value;
    private static readonly Lazy<MainDataCollector> _instance = new Lazy<MainDataCollector>(() => new MainDataCollector());
    private AppSettings _appSettings;
    private IServiceProvider _serviceProvider;
    private List<Product> CustomerDevices;
    private List<Category> Categories;

    private MainDataCollector() { }

    public void Initialize(AppSettings appSettings, IServiceProvider serviceProvider)
    {
        _appSettings = appSettings;
        _serviceProvider = serviceProvider;
        using (var scope = _serviceProvider.CreateScope())
        {
            var dataCollectorDbService = scope.ServiceProvider.GetRequiredService<DataCollectorDbService>();
            CustomerDevices = dataCollectorDbService._context.Products.ToList();
            Categories = dataCollectorDbService._context.Categories.ToList();
        }
    }

    public void CollectData()
    { 
        Console.WriteLine($"CollectData method executed at {DateTime.Now}");
        foreach (var device in CustomerDevices)
        {
            try
            {
                var (value, time) = ReadFromReaders(device);
                SaveToDatabase(device, time, value);
            }
            catch
            {
                continue;
            }

        }
    }

    private void SaveToDatabase(Product device, DateTime time, string value)
    {
        using (var scope = _serviceProvider.CreateScope())
        {
            var dataCollectorDbService = scope.ServiceProvider.GetRequiredService<DataCollectorDbService>();
            var toUpdate = dataCollectorDbService._context.Products.FirstOrDefault(x => x.Id == device.Id);
            if (toUpdate != null)
            {
                toUpdate.LastReadValue = value;
                toUpdate.LastSuccessActivity = time;
                dataCollectorDbService._context.SaveChanges();
            }
        }
    }

    private (string value, DateTime time) ReadFromReaders(Product device)
    {
        switch(device.CategoryId)
        {
            case 1: //DHT22
                var data = ReadDht22TempAndHumidity(device);
                return WrapDataFromDHT22(data); 
            default:
                throw new Exception("Unknown device category");
        }
    }

    #region DHT22
    private (double humidity, double temperature, DateTime time)? ReadDht22TempAndHumidity(Product device)
    {
        (double humidity, double temperature)? tempAndHumidity = null;
        int attemptCount = 0;
        while (attemptCount < 3 && tempAndHumidity == null)
        {
            var dht22 = new Dht22Reader(device.PIN);
            tempAndHumidity = dht22.ReadDht22();
            attemptCount++;
        }
        DateTime time = DateTime.Now;
        if (tempAndHumidity != null)
        {  
            (double humidity, double temperature) = tempAndHumidity.Value;
            Console.WriteLine(temperature + "C degrees, " + humidity + "% humidity " + time.ToShortDateString() + " " + time.ToShortTimeString());
            return (humidity, temperature, time);
        }
        else
        {
            SaveToDatabase(device, time, null);
            Console.WriteLine("Failed to read temperature and humidity after 3 attempts.");
            throw new Exception("Failed to read temperature and humidity after 3 attempts.");
        }
    }

    private (string value, DateTime time) WrapDataFromDHT22((double humidity, double temperature, DateTime time)? data)
    {
        var temp = Math.Round(data.Value.temperature, 1);
        var humidity = Math.Round(data.Value.humidity, 1);
        return ($"{temp}C degrees {humidity}% humidity", data.Value.time);
    }
    #endregion
}
