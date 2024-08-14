using System.Device.Gpio;
using System.Device.Spi;
using Iot.Device.Mfrc522;
using Iot.Device.Rfid;
namespace RaspSensorLibrary;

public class RfidReader
{
    private GpioController _controller;
    private SpiConnectionSettings _connection;
    private int _pinReset;

    public void Setup(int pinReset)
    {
        _controller = new GpioController(); // Usunięto lokalne tworzenie zmiennej _controller
        _pinReset = pinReset; // Użyto parametru pinReset
        _connection = new SpiConnectionSettings(0, 0)
        {
            ClockFrequency = 10_000_000
        };
    }

    public string ReadData()
    {
        try
        {
            using (SpiDevice spi = SpiDevice.Create(_connection))
            {
                using (MfRc522 mfRc522 = new(spi, _pinReset, _controller, false))
                {
                    Data106kbpsTypeA card;
                    var result = mfRc522.ListenToCardIso14443TypeA(out card, TimeSpan.FromSeconds(2));
                    if (result)
                    {
                        return GetCardIdentifier(card);
                    }
                }
            }
        }
        catch (Exception ex)
        {
            throw new Exception($"Error while reading RFID data: {ex.Message}", ex);
        }

        return null; // Zwraca null, jeśli karta nie została odczytana
    }

    private string GetCardIdentifier(Data106kbpsTypeA card) => Convert.ToHexString(card.NfcId);
}
