using ReefPiWorker.Clients;
using ReefPiWorker.IoT;
using static ReefPiWorker.Clients.CosmosDbClient;

namespace ReefPiWorker
{
    public class Worker : BackgroundService
    {
        private readonly ILogger<Worker> _logger;
        private readonly ICosmosDbClient _cosmosDbClient;
        private readonly IArduinoUnoR3FirmataCommandsWrapper _arduinoUnoR3FirmataCommandsWrapper;

        public Worker(
            ILogger<Worker> logger,
            ICosmosDbClient cosmosDbClient,
            IArduinoUnoR3FirmataCommandsWrapper arduinoUnoR3FirmataCommandsWrapper
            ) =>
            (_logger, _cosmosDbClient, _arduinoUnoR3FirmataCommandsWrapper) = 
            (logger, cosmosDbClient, arduinoUnoR3FirmataCommandsWrapper);
        

        protected override async Task ExecuteAsync(CancellationToken stoppingToken)
        {
            double temp;
            double hum;

            while (!stoppingToken.IsCancellationRequested)
            {                
                _arduinoUnoR3FirmataCommandsWrapper.ReadDhtData(out temp, out hum);

                _logger.LogInformation($"At {DateTime.Now.ToShortDateString} temperature is {temp} and humidity is {hum}");

                //_logger.LogInformation("Worker running at: {time}", DateTimeOffset.Now);
                //_ = _cosmosDbClient.CreateItemAsync($"Worker running at: {DateTimeOffset.Now}", Guid.NewGuid().ToString());
                await Task.Delay(1000, stoppingToken);
            }
        }
    }    
}