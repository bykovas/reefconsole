using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Microsoft.Extensions.Options;
using Microsoft.Azure.Cosmos;

namespace ReefPiWorker.Clients
{
    public class CosmosDbClient : ICosmosDbClient
    {
        private readonly CosmosDbClientOptions _options;
        private readonly string _partitionKeyPath = "/id";

        public CosmosDbClient(IOptions<CosmosDbClientOptions> options) =>
            (_options) =
            (options.Value);

        public async Task<ItemResponse<string>> CreateItemAsync(string jsonItem, string partitionKey)
        {
            using CosmosClient client = new(_options.CosmosConnectionString!);
            Database db = await client.CreateDatabaseIfNotExistsAsync(_options.DatabaseName);
            Container container = await db.CreateContainerIfNotExistsAsync(_options.DatabaseContainer, _partitionKeyPath, 400);
            ItemResponse<string> createdItem = await container.CreateItemAsync(jsonItem, new PartitionKey(partitionKey));
            return createdItem;
        }
    }
}
