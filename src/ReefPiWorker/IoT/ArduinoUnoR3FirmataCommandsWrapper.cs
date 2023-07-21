using Iot.Device.Arduino;
using Iot.Device.Board;
using Microsoft.Extensions.Options;
using ReefPiWorker.Clients;
using System;
using System.Collections.Generic;
using System.Device.Gpio;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

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
                ArduinoBoard.TryFindBoard(out ArduinoBoard? _arduinoBoard);

                //_arduinoBoard = new ArduinoBoard(portName, boudRate);

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

        public void ReadDhtData(out double temperatureDegreesCelsius, out double humidityPercent)
        {
            UnitsNet.Temperature temperature = new();
            UnitsNet.RelativeHumidity humidity = new();

            _dhtSensor.TryReadDht(_options.PinDht22, 22, out temperature, out humidity);

            temperatureDegreesCelsius = temperature.DegreesCelsius;
            humidityPercent = humidity.Percent;
        }
    }
}
