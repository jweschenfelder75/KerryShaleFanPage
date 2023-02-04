using System;
using System.Collections.Generic;
using Newtonsoft.Json;

namespace KerryShaleFanPage.Shared.Objects.ToDo.Twitter
{
    [Serializable]
    [Obsolete("Obsolete: We will not use Twitter API anymore!")]
    public class Tweets
    {
        [JsonProperty("data")]
        public IList<TweetData>? Data { get; set; }

        [JsonProperty("includes")]
        public Includes? Includes { get; set; }

        [JsonProperty("meta")]
        public Meta? Meta { get; set; }
    }
}
