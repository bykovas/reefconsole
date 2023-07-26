namespace ReefPiWorker.Scrappers.Models
{
    public class ReefFactoryKhKeeperPlusDataModel
    {
        public string id { get; set; } = Guid.NewGuid().ToString();
        public DateTime OnDateTimeUtc { get; set; }
        public double Kh { get; set; }
        public double Ph { get; set; }
    }
}
