using Microsoft.Azure.Cosmos;

namespace ReefPiWorker.Clients
{
    public interface ICosmosDbClient
    {
        Task<ItemResponse<string>> CreateItemAsync(string jsonItem, string partitionKey);
    }
}