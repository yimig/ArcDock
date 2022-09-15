﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Newtonsoft.Json;

namespace ArcDock.Data
{
    [JsonObject]
    public class ExecutionItem
    {
        [JsonProperty("key")]
        public string Key { get; set; }

        [JsonProperty("content")]
        public string Content { get; set; }
    }
}
