using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ArcDock.Data.Json
{
    [JsonObject]
    public class GlobalSettingJson
    {
        [JsonProperty("enable_check")]
        public bool IsEnableCheck { get; set; }

        [JsonProperty("print_api")]
        public int PrintApi { get; set; }

        [JsonProperty("code")]
        public string Code { get; set; }
    }
}
