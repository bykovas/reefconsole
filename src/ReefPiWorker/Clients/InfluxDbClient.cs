using InfluxDB3.Client;
using Microsoft.Extensions.Options;
using InfluxDB3.Client.Write;

namespace ReefPiWorker.Clients
{
    public enum InfluxDbMeasurements
    {
        Water,
        Air
    }

    public enum InfluxDbFields
    {
        Temperature,
        Humidity,
        Kh,
        Ph
    }

    public class InfluxDbClient : IInfluxDbClient
    {
        private readonly ILogger<InfluxDbClient> _logger;
        private readonly InfluxDbClientOptions _options;
        private readonly InfluxDBClient _client;

        public InfluxDbClient(
            ILogger<InfluxDbClient> logger,
            IOptions<InfluxDbClientOptions> options)
        {
            (this._logger, this._options) = (logger, options.Value);

            _client = new InfluxDBClient(_options.InfluxDbHostUrl, authToken: _options.Token);
        }

        public async Task<bool> AddMeasurement(InfluxDbMeasurements measurement, InfluxDbFields field, double value)
        {
            var point = new[]
            {
                PointData.Measurement(measurement.ToString()).AddField(field.ToString(), value)
            };

            await _client.WritePointAsync(point[0], database: _options.Database);

            return true;
        }

    }
}

