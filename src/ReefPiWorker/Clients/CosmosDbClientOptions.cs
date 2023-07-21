namespace ReefPiWorker.Clients
{
    public class CosmosDbClientOptions
    {        
        public string CosmosConnectionString { get; set; } = string.Empty;
        public string DatabaseName { get; set; } = string.Empty;
        public string DatabaseContainer { get; set; } = string.Empty;        
    }
}
