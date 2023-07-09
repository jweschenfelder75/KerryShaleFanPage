namespace KerryShaleFanPage.Client.Objects
{
    public class TileContentItem
    {
        public string? Name { get; set; }

        public string? Url { get; set; }

        public string? ImagePath { get; set; }

        public TileContentItem() 
        { }

        public TileContentItem(string? name, string? url, string? imagePath) 
        {
            Name = name;
            Url = url;
            ImagePath = imagePath;
        }
    }
}
