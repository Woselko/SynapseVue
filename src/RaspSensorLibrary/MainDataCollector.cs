using System.Collections.Generic;

namespace RaspSensorLibrary;

public class MainDataCollector
{
    public static void CollectData()
    {
        // Twoja logika do odczytu danych z czytników
        var data = ReadFromReaders();

        // Zapisz dane do bazy danych
        SaveToDatabase(data);
    }

    private static void SaveToDatabase(List<YourDataType> data)
    {
        throw new NotImplementedException();
    }

    private static List<YourDataType> ReadFromReaders()
    {
        // Implementacja odczytu danych z czytników
        // np. przez GPIO, I2C, UART itp.
        // Zwróć listę danych
        throw new NotImplementedException();
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


