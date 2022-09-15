using Newtonsoft.Json;

namespace ArcDock.Data.Json
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
