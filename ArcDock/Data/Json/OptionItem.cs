using System.Collections.Generic;
using Newtonsoft.Json;

namespace ArcDock.Data.Json
{
    [JsonObject]
    public class OptionItem
    {
        [JsonProperty("content")]
        public string Content { get; set; }

        [JsonProperty("exec")]
        public List<ExecutionItem> ExecutionItemList { get; set; }
    }
}
