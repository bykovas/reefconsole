using Microsoft.Azure.Cosmos;
using ReefPiWorker.Clients;
using ReefPiWorker.IoT;
using ReefPiWorker.Scrappers;

namespace ReefPiWorker
{
    public class Program
    {
        /// <summary>
        /// ReefPI Worker starting method.
        /// </summary>
        /// <param name="args">Optional starting parameters.</param>
        public static void Main(string[] args)
        {
            CreateHostBuilder(args).Build().Run();
        }

        /// <summary>
        /// Dependency Injection builder. Sets configuration and interface contracts implementation for this service.
        /// </summary>
        /// <param name="args">Optional starting parameters.</param>
        /// <returns>Constructed host builder</returns>
        public static IHostBuilder CreateHostBuilder(string[] args) =>
            Host.CreateDefaultBuilder(args)
                .ConfigureAppConfiguration((hostContext, configuration) =>
                {
                    configuration.Sources.Clear();

                    IHostEnvironment env = hostContext.HostingEnvironment;

                    configuration
                        .AddJsonFile("appsettings.json", optional: false, reloadOnChange: true)
                        .AddJsonFile($"appsettings.{env.EnvironmentName}.json", optional: false, reloadOnChange: true);

                    IConfigurationRoot configurationRoot = configuration.Build();
                })
                .ConfigureServices((hostContext, services) =>
                {
                    services.AddHostedService<Worker>();

                    //services.Configure<CosmosDbClientOptions>(options => hostContext.Configuration.GetSection(nameof(CosmosDbClientOptions)).Bind(options));
                    //services.AddSingleton<ICosmosDbClient, CosmosDbClient>();

                    services.Configure<InfluxDbClientOptions>(options => hostContext.Configuration.GetSection(nameof(InfluxDbClientOptions)).Bind(options));
                    services.AddSingleton<IInfluxDbClient, InfluxDbClient>();

                    services.Configure<ArduinoUnoR3PinOptions>(options => hostContext.Configuration.GetSection(nameof(ArduinoUnoR3PinOptions)).Bind(options));
                    services.AddSingleton<IArduinoUnoR3FirmataCommandsWrapper, ArduinoUnoR3FirmataCommandsWrapper>();

                    services.Configure<ReefFactoryScrapperOptions>(options => hostContext.Configuration.GetSection(nameof(ReefFactoryScrapperOptions)).Bind(options));
                    services.AddSingleton<IReefFactoryScrapper, ReefFactoryScrapper>();
                });
    }
}