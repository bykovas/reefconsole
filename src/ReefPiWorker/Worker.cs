using ReefPiWorker.Clients;
using ReefPiWorker.IoT;
using ReefPiWorker.Scrappers;

namespace ReefPiWorker
{
    public class Worker : BackgroundService
    {
        private readonly ILogger<Worker> _logger;
        private readonly ICosmosDbClient _cosmosDbClient;
        private readonly IInfluxDbClient _influxDbClient;
        private readonly IReefFactoryScrapper _reefFactoryScrapper;
        private readonly IArduinoUnoR3FirmataCommandsWrapper _arduinoUnoR3FirmataCommandsWrapper;

        public Worker(
            ILogger<Worker> logger,
            ICosmosDbClient cosmosDbClient,
            IArduinoUnoR3FirmataCommandsWrapper arduinoUnoR3FirmataCommandsWrapper,
            IReefFactoryScrapper reefFactoryScrapper,
            IInfluxDbClient influxDbClient
            ) =>
            (_logger, _cosmosDbClient, _arduinoUnoR3FirmataCommandsWrapper, _reefFactoryScrapper, _influxDbClient) = 
            (logger, cosmosDbClient, arduinoUnoR3FirmataCommandsWrapper, reefFactoryScrapper, influxDbClient);
        

        protected override async Task ExecuteAsync(CancellationToken stoppingToken)
        {

            while (!stoppingToken.IsCancellationRequested)
            {
                _arduinoUnoR3FirmataCommandsWrapper.ReadDhtData(out var temp, out var hum);
                var data = await _reefFactoryScrapper.ReadLastKhPhValues();

                if (data == null)
                {
                    _logger.LogInformation($"{DateTime.Now.ToShortTimeString()} Kh measurement in progress, skipping...");
                }
                else
                {
                    _logger.LogInformation(
                        $"At {DateTime.Now.ToShortTimeString()} temperature is {temp}, humidity is {hum}" + Environment.NewLine +
                        $"Ph is {data.Ph}, Kh is {data.Kh} (by {data.OnDateTimeUtc:yyyy-MM-dd HH:mm})");
                    _ = _cosmosDbClient.CreateItemAsync(data, data.id);
                    _ = _influxDbClient.AddMeasurement(InfluxDbMeasurements.Water, InfluxDbFields.Kh, data.Kh);
                    _ = _influxDbClient.AddMeasurement(InfluxDbMeasurements.Water, InfluxDbFields.Ph, data.Ph);
                    _ = _influxDbClient.AddMeasurement(InfluxDbMeasurements.Air, InfluxDbFields.Temperature, temp);
                    _ = _influxDbClient.AddMeasurement(InfluxDbMeasurements.Air, InfluxDbFields.Humidity, hum);
                }

                var now = DateTime.Now;
                var previousRun = new DateTime(now.Year, now.Month, now.Day, now.Hour, 55, 0, now.Kind);
                var nextTrigger = previousRun + TimeSpan.FromHours(1);
                var wait = nextTrigger - now;

                await Task.Delay(wait, stoppingToken);
            }
        }
    }    
}