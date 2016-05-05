namespace VirtualizedData.Service
{
    public class Item
    {        
        public string Title { get; set; }
        public string ImageUrl { get; set; }
        public static Item CreateDefault()
        {
            return new Item() { Title = "Spectre", ImageUrl = "https://pbs.twimg.com/profile_images/525273754591432704/uJvGGkCt_400x400.jpeg" };
        }
    }
}