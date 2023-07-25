using Microsoft.Azure.Cosmos;

namespace ReefPiWorker.Clients
{
    public interface ICosmosDbClient
    {
        /// <summary>
        /// Creates Item in specified in appsettings CosmosDb container by partition "id"
        /// </summary>
        /// <param name="data">Item as object</param>
        /// <param name="partitionKey">Partition key in "id" path</param>
        /// <returns></returns>
        Task<ItemResponse<object?>> CreateItemAsync(object? data, string partitionKey);
    }
}