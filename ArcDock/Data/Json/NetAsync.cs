using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ArcDock.Data.Json
{
    [JsonObject]
    public class NetAsync
    {
        [JsonProperty("version")]
        public int Version { get; set; }

        [JsonProperty("code")]
        public string Code { get; set; }

        [JsonProperty("message")]
        public string Message { get; set; }

        [JsonProperty("token")]
        public string Token { get; set; }

        [JsonProperty("size")]
        public int Size { get; set; }

        [JsonProperty("global_setting")]
        public GlobalSettingJson GlobalSetting { get; set; }
    }
}
