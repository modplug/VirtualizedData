using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace VirtualizedData.Service
{
    public class FakeDataService : IDataService
    {
        private readonly List<Item> _items;
        private const int MaxNumberOfPages = 4;
        private const int PageSize = 20;

        public FakeDataService()
        {
            _items = new List<Item>();
            for (int i = 0; i < PageSize*MaxNumberOfPages; i++)
            {
                _items.Add(Item.CreateDefault());
            }
        }

        public async Task<ItemResult> GetItemsAsync(int page)
        {
            Random rnd = new Random();
            var itemsToReturn = _items.Skip(page*PageSize).Take(PageSize).ToList();
            await Task.Delay(rnd.Next(500, 3000));
            return new ItemResult(itemsToReturn, MaxNumberOfPages - page);
        }
    }
}