using System.Threading.Tasks;

namespace VirtualizedData.Service
{
    public interface IDataService
    {
        Task<ItemResult> GetItemsAsync(int page);
    }
}