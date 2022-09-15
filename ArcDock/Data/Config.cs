using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Newtonsoft.Json;

namespace ArcDock.Data
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
