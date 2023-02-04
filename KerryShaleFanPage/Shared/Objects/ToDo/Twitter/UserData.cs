using System;
using Newtonsoft.Json;

namespace KerryShaleFanPage.Shared.Objects.ToDo.Twitter
{
    [Serializable]
    [Obsolete("Obsolete: We will not use Twitter API anymore!")]
    public class UserData
    {
        [JsonProperty("id")]
        public string? Id { get; set; }

        [JsonProperty("created_at")]
        public DateTime? CreatedAt { get; set; }

        [JsonProperty("pinned_tweet_id")]
        public string? PinnedTweetId { get; set; }

        [JsonProperty("description")]
        public string? Description { get; set; }

        [JsonProperty("username")]
        public string? Username { get; set; }

        [JsonProperty("name")]
        public string? Name { get; set; }
    }
}
