using System;
using System.Collections.Generic;
using Newtonsoft.Json;

namespace KerryShaleFanPage.Shared.Objects.ToDo.Twitter
{
    [Serializable]
    [Obsolete("Obsolete: We will not use Twitter API anymore!")]
    public class Attachments
    {
        [JsonProperty("media_keys")]
        public IList<string>? MediaKeys { get; set; }
    }
}
