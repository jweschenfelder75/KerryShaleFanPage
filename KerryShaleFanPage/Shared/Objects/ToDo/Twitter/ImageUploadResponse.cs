using System;
using Newtonsoft.Json;

namespace KerryShaleFanPage.Shared.Objects.ToDo.Twitter
{
    [Serializable]
    [Obsolete("Obsolete: We will not use Twitter API anymore!")]
    public class ImageUploadResponse
    {
        [JsonProperty("image_type")]
        public long? MediaId { get; set; }

        [JsonProperty("media_id_string")]
        public string? MediaIdString { get; set; }

        [JsonProperty("media_key")]
        public string? MediaKey { get; set; }

        [JsonProperty("size")]
        public int? Size { get; set; }

        [JsonProperty("expires_after_secs")]
        public int? ExpiresAfterSecs { get; set; }

        [JsonProperty("image")]
        public Image? Image { get; set; }
    }
}
