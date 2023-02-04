using System;
using Newtonsoft.Json;

namespace KerryShaleFanPage.Shared.Objects.ToDo.Twitter
{
    [Serializable]
    [Obsolete("Obsolete: We will not use Twitter API anymore!")]
    public class Image
    {
        [JsonProperty("image_type")]
        public string? ImageType { get; set; }

        [JsonProperty("w")]
        public int? W { get; set; }

        [JsonProperty("h")]
        public int? H { get; set; }
    }
}
