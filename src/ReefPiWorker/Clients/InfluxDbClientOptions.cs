namespace ReefPiWorker.Clients
{
    public class InfluxDbClientOptions
    {
        public string Token { get; set; } = string.Empty;
        public string InfluxDbHostUrl { get; set; } = string.Empty;
        public string Database { get; set; } = string.Empty;
    }
}
