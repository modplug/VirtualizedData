using System.Collections.Generic;

namespace VirtualizedData.Service
{
    public class ItemResult
    {
        public ItemResult(List<Item> items, int remainingPages)
        {
            this.Items = items;
            RemainingPages = remainingPages;
        }

        public List<Item> Items { get; }
        public int RemainingPages { get; }
    }
}