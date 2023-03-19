using System;
using System.Collections.Generic;

namespace KerryShaleFanPage.Shared.Configuration
{
    [Serializable]
    public class GallerySettings
    {
        public IList<GalleryItem> GalleryItems { get; set; } = new List<GalleryItem>();
    }

    /// <summary>
    /// TODO: Put classes below in extra files. Let ReSharper do that job.
    /// </summary>

    [Serializable]
    public class GalleryItem
    {
        public string? ImageSrc { get; set; }
        public string? ImageAltEn { get; set; }
        public string? ImageAltDe { get; set; }
        public string? ImageCredits { get; set; }
    }
}
