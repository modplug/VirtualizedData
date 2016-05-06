using System.Threading.Tasks;

namespace VirtualizedData.Services
{
    public interface IDataService
    {
        Task<MovieResponse> GetItemsAsync(string searchQuery, int page);
    }
}