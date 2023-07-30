using Iot.Device.Arduino;
using Microsoft.Extensions.Options;
using System.Device.Gpio;
using Iot.Device.Pn532.RfConfiguration;

namespace ReefPiWorker.IoT
{
    public class ArduinoUnoR3FirmataCommandsWrapper : IArduinoUnoR3FirmataCommandsWrapper
    {
        private readonly ILogger<ArduinoUnoR3FirmataCommandsWrapper> _logger;
        private readonly ArduinoUnoR3PinOptions _options;

        private ArduinoBoard _arduinoBoard;
        private GpioController _gpioController;
        private DhtSensor _dhtSensor;

        public ArduinoUnoR3FirmataCommandsWrapper(
            ILogger<ArduinoUnoR3FirmataCommandsWrapper> logger,
            IOptions<ArduinoUnoR3PinOptions> options)
        {
            (_logger, _options) =
                (logger, options.Value);

            InitBoardControllers(_options.PortName, _options.BoudRate);
        }

        private void InitBoardControllers(string portName, int boudRate)
        {
            try
            {
                try
                {
                    _arduinoBoard = new ArduinoBoard(portName, boudRate);
                    var fv = _arduinoBoard.FirmwareVersion;
                    _logger.LogTrace(
                        $"Successfully connected to Arduino board ({_options.PortName}:{_options.BoudRate}); Firmware: {_arduinoBoard.FirmwareName} {_arduinoBoard.FirmwareVersion}, Firmata: {_arduinoBoard.FirmataVersion}");
                }
                catch (Exception ex)
                {
                    _logger.LogError($"Unable to connect to Arduino Firmata on {_options.PortName}", ex);
                    ArduinoBoard.TryFindBoard(out var _arduinoBoard);
                    try
                    {
                        var fv = _arduinoBoard?.FirmwareVersion;
                        _logger.LogTrace(
                                $"Successfully connected to Arduino board ({_options.PortName}:{_options.BoudRate}); Firmware: {_arduinoBoard.FirmwareName} {_arduinoBoard.FirmwareVersion}, Firmata: {_arduinoBoard.FirmataVersion}");
                    }
                    catch (Exception e)
                    {
                        _logger.LogError("Unable to find Arduino board", ex);
                    }
                }
                
                _arduinoBoard.SetPinMode(_options.PinLedStatus, SupportedMode.DigitalOutput);
                _arduinoBoard.SetPinMode(_options.PinButton, SupportedMode.DigitalInput);
                _arduinoBoard.SetPinMode(_options.PinDht22, SupportedMode.Dht);

                _gpioController = _arduinoBoard.CreateGpioController();
                _gpioController.OpenPin(_options.PinLedStatus);
                _gpioController.OpenPin(_options.PinButton);
                _gpioController.OpenPin(_options.PinDht22);

                _dhtSensor = new DhtSensor();
                _arduinoBoard.AddCommandHandler(_dhtSensor);
            }
            catch (Exception ex)
            {
                _logger.LogCritical("Error while initiating ArduinoUnoR3 board", ex);
                throw;
            }
        }

        private void CheckAndRecconectArduinoBoardIfNotConnected()
        {
            if (_arduinoBoard.GetPinMode(_options.PinLedStatus) != SupportedMode.DigitalOutput)
                InitBoardControllers(_options.PortName, _options.BoudRate);
        }

        public void ReadDhtData(out double temperatureDegreesCelsius, out double humidityPercent)
        {
            try
            {
                CheckAndRecconectArduinoBoardIfNotConnected();

            _dhtSensor.TryReadDht(_options.PinDht22, 22, out var temperature, out var humidity);
            temperatureDegreesCelsius = temperature.DegreesCelsius;
            humidityPercent = humidity.Percent;

            var retries = 3;
            while (retries > 0 || temperatureDegreesCelsius == 0 || humidityPercent == 0)
            {
                _dhtSensor.TryReadDht(_options.PinDht22, 22, out temperature, out humidity);
                temperatureDegreesCelsius = temperature.DegreesCelsius;
                humidityPercent = humidity.Percent;
                retries--;
            }
            }
            catch (Exception ex)
            {
                _logger.LogError("Unable to read DHT", ex);
                temperatureDegreesCelsius = -1;
                humidityPercent = -1;
            }
        }
    }
}
