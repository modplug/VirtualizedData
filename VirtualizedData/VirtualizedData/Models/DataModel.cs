using System;
using System.Reactive.Subjects;
using System.Threading.Tasks;
using DynamicData;
using VirtualizedData.Services;

namespace VirtualizedData.Models
{
    public class DataModel : IDataModel
    {
        private readonly IDataService _dataService;
        private readonly SourceList<Movie> _itemsInternalList;


        public DataModel(IDataService dataService)
        {
            _dataService = dataService;
            _itemsInternalList = new SourceList<Movie>();
            RemainingPages = new Subject<int>();
        }

        public IObservableList<Movie> Items => _itemsInternalList;
        public Subject<int> RemainingPages { get; set; }

        public async Task GetPage(int pageNumber)
        {
            try
            {
                var result = await _dataService.GetItemsAsync("batman", pageNumber);
                _itemsInternalList.Edit(list => list.AddRange(result.Movies));
                RemainingPages.OnNext(result.RemainingPages);
            }
            catch (Exception e)
            {
                
            }
        }
    }

    public interface IDataModel
    {
        Task GetPage(int pageNumber);
        IObservableList<Movie> Items { get; }
        Subject<int> RemainingPages { get; set; }
    }
}
