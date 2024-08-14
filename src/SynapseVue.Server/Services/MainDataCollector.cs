using System;
using System.Collections.Generic;
using RaspSensorLibrary;
using SynapseVue.Server;
using SynapseVue.Server.Models.Categories;
using SynapseVue.Server.Models.Products;

namespace SynapseVue.Server.Services;

public class MainDataCollector
{
    private readonly IDbContextFactory<AppDbContext> _dbContextFactory;
    private List<Product> _customerDevices;
    private List<Category> _categories;

    public MainDataCollector(IDbContextFactory<AppDbContext> dbContextFactory)
    {
        _dbContextFactory = dbContextFactory;
        Initialize();
    }

    private void Initialize()
    {
        using (var context = _dbContextFactory.CreateDbContext())
        {
            _customerDevices = context.Products.ToList();
            _categories = context.Categories.ToList();
        }
    }

    public void CollectData()
    {
        Console.WriteLine($"CollectData method executed at {DateTime.Now}");

        foreach (var device in _customerDevices)
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
        using (var context = _dbContextFactory.CreateDbContext())
        {
            var toUpdate = context.Products.FirstOrDefault(x => x.Id == device.Id);
            if (toUpdate != null)
            {
                toUpdate.LastReadValue = value;
                toUpdate.LastSuccessActivity = time;
                context.SaveChanges();
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
            Console.WriteLine($"{temperature}C degrees, {humidity}% humidity at {time.ToShortDateString}");
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
}
