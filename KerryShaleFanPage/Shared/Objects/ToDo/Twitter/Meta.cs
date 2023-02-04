using Newtonsoft.Json;
using System;

namespace KerryShaleFanPage.Shared.Objects.ToDo.Twitter
{
    [Serializable]
    [Obsolete("Obsolete: We will not use Twitter API anymore!")]
    public class Meta
    {
        [JsonProperty("next_token")]
        public string? NextToken { get; set; }

        [JsonProperty("result_count")]
        public int? ResultCount { get; set; }

        [JsonProperty("newest_id")]
        public string? NewestId { get; set; }

        [JsonProperty("oldest_id")]
        public string? OldestId { get; set; }
    }
}
