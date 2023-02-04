using System;
using System.Collections.Generic;
using Newtonsoft.Json;

namespace KerryShaleFanPage.Shared.Objects.ToDo.Twitter
{
    [Serializable]
    [Obsolete("Obsolete: We will not use Twitter API anymore!")]
    public class TweetData
    {
        [JsonProperty("id")]
        public string? Id { get; set; }

        [JsonProperty("created_at")]
        public DateTime? CreatedAt { get; set; }

        [JsonProperty("edit_history_tweet_ids")]
        public IList<string>? EditHistoryTweetIds { get; set; }

        [JsonProperty("attachments")]
        public Attachments? Attachments { get; set; }

        [JsonProperty("text")]
        public string? Text { get; set; }
    }
}
