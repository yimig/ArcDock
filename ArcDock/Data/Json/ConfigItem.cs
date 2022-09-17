using System.Collections.Generic;
using Newtonsoft.Json;

namespace ArcDock.Data.Json
{
    [JsonObject]
    public class ConfigItem
    {
        [JsonProperty("id")]
        public string Id { get; set; }

        [JsonProperty("name")]
        public string Name { get; set; }

        [JsonProperty("type")]
        public string Type { get; set; }

        [JsonProperty("opt_type")]
        public int OptionType { get; set; }

        [JsonProperty("option")]
        public List<string> Option { get; set; }

        [JsonProperty("option_exc")]
        public List<OptionItem> OptionItemList { get; set; }

        [JsonProperty("default")]
        public string Default { get; set; }

        [JsonProperty("rules")]
        public string Rules { get; set; }
    }
}
