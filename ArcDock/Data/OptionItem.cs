using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Newtonsoft.Json;

namespace ArcDock.Data
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
