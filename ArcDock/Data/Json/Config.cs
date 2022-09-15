using System.Collections.Generic;
using Newtonsoft.Json;

namespace ArcDock.Data.Json
{
    [JsonObject]
    public class Config
    {
        [JsonProperty("settings")]
        public Setting Settings { get; set; }

        [JsonProperty("data")]
        public List<ConfigItem> ConfigItemList { get; set; }
    }
}
