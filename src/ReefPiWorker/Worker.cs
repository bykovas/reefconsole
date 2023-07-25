using System.Text.Json;
using ReefPiWorker.Clients;
using ReefPiWorker.IoT;
using ReefPiWorker.Scrappers;
using ReefPiWorker.Scrappers.Models;
using static ReefPiWorker.Clients.CosmosDbClient;

namespace ReefPiWorker
{
    public class Worker : BackgroundService
    {
        private readonly ILogger<Worker> _logger;
        private readonly ICosmosDbClient _cosmosDbClient;
        private readonly IReefFactoryScrapper _reefFactoryScrapper;
        private readonly IArduinoUnoR3FirmataCommandsWrapper _arduinoUnoR3FirmataCommandsWrapper;

        public Worker(
            ILogger<Worker> logger,
            ICosmosDbClient cosmosDbClient,
            IArduinoUnoR3FirmataCommandsWrapper arduinoUnoR3FirmataCommandsWrapper,
            IReefFactoryScrapper reefFactoryScrapper
            ) =>
            (_logger, _cosmosDbClient, _arduinoUnoR3FirmataCommandsWrapper, _reefFactoryScrapper) = 
            (logger, cosmosDbClient, arduinoUnoR3FirmataCommandsWrapper, reefFactoryScrapper);
        

        protected override async Task ExecuteAsync(CancellationToken stoppingToken)
        {

            while (!stoppingToken.IsCancellationRequested)
            {
                _arduinoUnoR3FirmataCommandsWrapper.ReadDhtData(out var temp, out var hum);
                var data = await _reefFactoryScrapper.ReadLastKhPhValues();

                if (data == null)
                {
                    _logger.LogInformation("Kh measurement in progress, skipping");
                }
                else
                {
                    _logger.LogInformation(
                        $"At {DateTime.Now.ToShortTimeString()} temperature is {temp}, humidity is {hum}" + Environment.NewLine +
                        $"Ph is {data.Ph}, Kh is {data.Kh} (by {data.OnDateTimeUtc:yyyy-MM-dd HH:mm})");
                    _ = _cosmosDbClient.CreateItemAsync(data, data.id);
                }

                await Task.Delay(1000 * 60 * 20, stoppingToken);
            }
        }
    }    
}