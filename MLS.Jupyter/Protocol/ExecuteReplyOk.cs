﻿using System.Collections.Generic;
using Newtonsoft.Json;

namespace Microsoft.DotNet.Try.Jupyter.Protocol
{
    public class ExecuteReplyOk : ExecuteReply
    {
        public ExecuteReplyOk()
        {
            Status = StatusValues.Ok;
        }

        [JsonProperty("payload")]
        public List<Dictionary<string,string>> Payload { get; set; }

        [JsonProperty("user_expressions")]
        public Dictionary<string,string> UserExpressions { get; set; }
    }
}
