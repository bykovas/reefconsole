using ReefPiWorker.Clients;
using static ReefPiWorker.Clients.CosmosDbClient;

namespace ReefPiWorker
{
    public class Worker : BackgroundService
    {
        private readonly ILogger<Worker> _logger;
        private readonly ICosmosDbClient _cosmosDbClient;

        public Worker(
            ILogger<Worker> logger,
            ICosmosDbClient cosmosDbClient
            ) =>
            (_logger, _cosmosDbClient) = 
            (logger, cosmosDbClient);
        

        protected override async Task ExecuteAsync(CancellationToken stoppingToken)
        {
            while (!stoppingToken.IsCancellationRequested)
            {
                _logger.LogInformation("Worker running at: {time}", DateTimeOffset.Now);
                _ = _cosmosDbClient.CreateItemAsync($"Worker running at: {DateTimeOffset.Now}", Guid.NewGuid().ToString());
                await Task.Delay(1000, stoppingToken);
            }
        }
    }    
}