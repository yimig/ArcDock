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

        [JsonProperty("print_unit")]
        public string PrintUnit { get; set; }

        [JsonProperty("print_height")]
        public int PrintHeight { get; set; }

        [JsonProperty("print_width")]
        public int PrintWidth { get; set; }
    }
}
