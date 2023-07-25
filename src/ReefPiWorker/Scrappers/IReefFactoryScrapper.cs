using ReefPiWorker.Scrappers.Models;

namespace ReefPiWorker.Scrappers
{
    public interface IReefFactoryScrapper
    {
        Task<ReefFactoryKhKeeperPlusDataModel?> ReadLastKhPhValues();
    }
}
