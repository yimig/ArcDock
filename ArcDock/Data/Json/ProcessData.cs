using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ArcDock.Data.Json
{
    [JsonObject]
    public class ProcessData
    {
        [JsonProperty("handled")]
        public bool IsHandled { get; set; }

        [JsonProperty("silent")]
        public bool IsSilent { get; set; }

        [JsonProperty("associate")]
        public bool IsAssociate { get; set; }

        [JsonProperty("template")]
        public string TemplateName { get; set; }

        [JsonProperty("arguments")]
        public Dictionary<string, string> Arguments { get; set; }

    }
}
