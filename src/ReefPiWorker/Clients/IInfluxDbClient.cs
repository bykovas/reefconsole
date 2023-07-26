namespace ReefPiWorker.Clients;

public interface IInfluxDbClient
{
    Task<bool> AddMeasurement(InfluxDbMeasurements measurement, InfluxDbFields field, double value);
}