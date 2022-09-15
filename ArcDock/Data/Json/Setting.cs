using Newtonsoft.Json;

namespace ArcDock.Data.Json
{
    [JsonObject]
    public class Setting
    {
        [JsonProperty("printer")]
        public string Printer { get; set; }

        [JsonProperty("height")]
        public int Height { get; set; }

        [JsonProperty("width")]
        public int Width { get; set; }
    }
}
