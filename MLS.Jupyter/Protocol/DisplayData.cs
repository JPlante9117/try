﻿using Newtonsoft.Json;
using Newtonsoft.Json.Linq;

namespace Microsoft.DotNet.Try.Jupyter.Protocol
{
    public class DisplayData
    {
        [JsonProperty("source", NullValueHandling = NullValueHandling.Ignore)]
        public string Source { get; set; }

        [JsonProperty("data", NullValueHandling = NullValueHandling.Ignore)]
        public object Data { get; set; }

        [JsonProperty("metadata", NullValueHandling = NullValueHandling.Ignore)]
        public object MetaData { get; } = new JObject();

        [JsonProperty("transient", NullValueHandling = NullValueHandling.Ignore)]
        public object Transient { get; set; }
    }
}
