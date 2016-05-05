using System;
using System.Reactive.Subjects;
using System.Threading.Tasks;
using DynamicData;
using VirtualizedData.Service;

namespace VirtualizedData.Models
{
    public class DataModel : IDataModel
    {
        private readonly IDataService _dataService;
        private readonly SourceList<Item> _itemsInternalList;


        public DataModel(IDataService dataService)
        {
            _dataService = dataService;
            _itemsInternalList = new SourceList<Item>();
            RemainingPages = new Subject<int>();
        }

        public IObservableList<Item> Items => _itemsInternalList;
        public Subject<int> RemainingPages { get; set; }

        public async Task GetPage(int pageNumber)
        {
            try
            {
                var result = await _dataService.GetItemsAsync(pageNumber);
                _itemsInternalList.Edit(list => list.AddRange(result.Items));
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
        IObservableList<Item> Items { get; }
        Subject<int> RemainingPages { get; set; }
    }
}
