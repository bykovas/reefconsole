namespace ReefPiWorker.IoT
{
    public interface IArduinoUnoR3FirmataCommandsWrapper
    {
        void ReadDhtData(out double temperatureDegreesCelsius, out double humidityPercent);
    }
}