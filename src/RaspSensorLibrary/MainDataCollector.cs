using System;
using System.Collections.Generic;

namespace RaspSensorLibrary;

public class MainDataCollector
{
    private static readonly Lazy<MainDataCollector> _instance = new Lazy<MainDataCollector>(() => new MainDataCollector());

    public static MainDataCollector Instance => _instance.Value;

    public int BUZZ_PIN { get; private set; }
    public int DHT22_PIN { get; private set; }
    public int PIR_PIN { get; private set; }
    public int LED_PIN { get; private set; }

    private MainDataCollector() { }

    public void Initialize(int dht22Pin, int pirPin, int ledPin, int buzzPin)
    {
        DHT22_PIN = dht22Pin;
        PIR_PIN = pirPin;
        LED_PIN = ledPin;
        BUZZ_PIN = buzzPin;
    }

    public void CollectData()
    {
        Console.WriteLine($"CollectData method executed at {DateTime.Now}");
        Console.WriteLine($"DHT22_PIN: {Instance.DHT22_PIN}, PIR_PIN: {Instance.PIR_PIN}, LED_PIN: {Instance.LED_PIN}, BUZZ_PIN: {Instance.BUZZ_PIN}");
    
        // Twoja logika do odczytu danych z czytników
        var data = ReadFromReaders();
    
        // Zapisz dane do bazy danych
        SaveToDatabase(data);
    }

    private void SaveToDatabase(List<YourDataType> data)
    {
        Console.WriteLine("Save to databasse task");
    }

    private List<YourDataType> ReadFromReaders()
    {
        // Implementacja odczytu danych z czytników
        // np. przez GPIO, I2C, UART itp.
        // Zwróć listę danych
        (double humidity, double temperature)? tempAndHumidity = null;
        int attemptCount = 0;
        while (attemptCount < 3 && tempAndHumidity == null)
        {
            tempAndHumidity = ReadDht22TempAndHumidity();
            attemptCount++;
        }
        if (tempAndHumidity != null)
        {
            (double humidity, double temperature) = tempAndHumidity.Value;
            Console.WriteLine(temperature + "C degrees, " + humidity + "% humidity " + DateTime.Now.ToShortDateString() + " " + DateTime.Now.ToShortTimeString());
            return null;
        }
        else
        {
            Console.WriteLine("Failed to read temperature and humidity after 3 attempts.");
            throw new Exception("Failed to read temperature and humidity after 3 attempts.");
        }
    }

    private (double humidity, double temperature)? ReadDht22TempAndHumidity()
    {
        var dht22 = new Dht22Reader(DHT22_PIN);
        return dht22.ReadDht22();
    }
}
//using System.Data.Entity;
//namespace YourNamespace
//{
//    public class YourDbContext : DbContext
//    {
//        public DbSet<YourDataType> YourDataTable { get; set; }
//    }
//}
public class YourDataType
{
    public int Id { get; set; }
    public string Data { get; set; }
    public DateTime Timestamp { get; set; }
}


