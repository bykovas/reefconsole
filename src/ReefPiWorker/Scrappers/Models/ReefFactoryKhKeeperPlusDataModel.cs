namespace ReefPiWorker.Scrappers.Models
{
    public class ReefFactoryKhKeeperPlusDataModel
    {
        public string id { get; set; } = Guid.NewGuid().ToString();
        public DateTime OnDateTimeUtc { get; set; }
        public decimal Kh { get; set; }
        public decimal Ph { get; set; }
    }
}
