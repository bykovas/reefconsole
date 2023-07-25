using Microsoft.Extensions.Options;
using Microsoft.Azure.Cosmos;

namespace ReefPiWorker.Clients
{
    public class CosmosDbClient : ICosmosDbClient
    {
        private readonly ILogger<CosmosDbClient> _logger;
        private readonly CosmosDbClientOptions _options;
        private readonly string _partitionKeyPath = "/id";

        public CosmosDbClient(
            IOptions<CosmosDbClientOptions> options,
            ILogger<CosmosDbClient> logger) => 
            (_options, _logger) =
            (options.Value, logger);
        
        public async Task<ItemResponse<object?>> CreateItemAsync(object? data, string partitionKey)
        {
            try
            {
                using CosmosClient client = new(_options.CosmosConnectionString!);
                Database db = await client.CreateDatabaseIfNotExistsAsync(_options.DatabaseName);
                Container container = await db.CreateContainerIfNotExistsAsync(_options.DatabaseContainer, _partitionKeyPath, 400);
                var createdItem = await container.CreateItemAsync(data, new PartitionKey(partitionKey));
                return createdItem;
            }
            catch (Exception ex)
            {
                _logger.LogError("Error writing Item to CosmosDb", ex);
                throw;
            }

            return null;
        }
    }
}
