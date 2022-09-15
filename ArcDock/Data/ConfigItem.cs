using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Newtonsoft.Json;

namespace ArcDock.Data
{
    [JsonObject]
    public class ConfigItem
    {
        [JsonProperty("id")]
        public string Id { get; set; }

        [JsonProperty("name")]
        public string Name { get; set; }

        [JsonProperty("opt_type")]
        public int OptionType { get; set; }

        [JsonProperty("option")]
        public List<string> Option { get; set; }

        [JsonProperty("option_exc")]
        public List<OptionItem> OptionItemList { get; set; }

        [JsonProperty("rules")]
        public string Rules { get; set; }
    }
}
