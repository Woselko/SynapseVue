using SynapseVue.Server.Models.Products;

namespace SynapseVue.Server.Data.Configurations.Identity;

public class ProductConfiguration : IEntityTypeConfiguration<Product>
{
    public void Configure(EntityTypeBuilder<Product> builder)
    {
        DateTime baseDate = DateTime.Parse("2024-06-03");

        builder.HasData(
            new Product { Id = 1, Name = "PIRSensor", PIN = 18, Description = "Passive Infra Red motion detector", CreatedOn = baseDate, LastSuccessActivity = baseDate, LastReadValue = "", CategoryId = 1 },
            new Product { Id = 2, Name = "LED", PIN = 17, Description = "Led diode", CreatedOn = baseDate, LastSuccessActivity = baseDate, LastReadValue = "", CategoryId = 1 },
            new Product { Id = 3, Name = "Buzzer", PIN = 23, Description = "Buzzer for sound generating", CreatedOn = baseDate.AddDays(-20), LastSuccessActivity = baseDate, LastReadValue = "", CategoryId = 4 },
            new Product { Id = 4, Name = "DHT22Sensor", PIN = 27, Description = "DHT Temperature and humidity reader", CreatedOn = baseDate, LastSuccessActivity = baseDate, LastReadValue = "", CategoryId = 2 },
            new Product { Id = 5, Name = "Display", PIN = 0, Description = "Display, no pin needed", CreatedOn = baseDate, LastSuccessActivity = baseDate, LastReadValue = "", CategoryId = 3 },
            new Product { Id = 6, Name = "Camera0", PIN = 0, Description = "Camera connected to PCI slot 0, no pin needed", CreatedOn = baseDate.AddDays(-19), LastSuccessActivity = baseDate, LastReadValue = "", CategoryId = 5 },
            new Product { Id = 7, Name = "Camera1", PIN = 0, Description = "Camera connected to PCI slot 1, no pin needed", CreatedOn = baseDate.AddDays(-10), LastSuccessActivity = baseDate, LastReadValue = "", CategoryId = 5 }
            );
    }
}

