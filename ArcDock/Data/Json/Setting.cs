using Newtonsoft.Json;

namespace ArcDock.Data.Json
{
    /// <summary>
    /// 全局配置类
    /// 模板内容可以替换config.json的内容
    /// </summary>
    [JsonObject]
    public class Setting
    {
        /// <summary>
        /// 默认打印机名称
        /// </summary>
        [JsonProperty("printer")]
        public string Printer { get; set; }

        /// <summary>
        /// 纵向像素数量
        /// </summary>
        [JsonProperty("height")]
        public int Height { get; set; }

        /// <summary>
        /// 横向像素数量
        /// </summary>
        [JsonProperty("width")]
        public int Width { get; set; }

        /// <summary>
        /// 打印单位
        /// </summary>
        [JsonProperty("print_unit")]
        public string PrintUnit { get; set; }

        /// <summary>
        /// 打印高度
        /// </summary>
        [JsonProperty("print_height")]
        public int PrintHeight { get; set; }

        /// <summary>
        /// 打印宽度
        /// </summary>
        [JsonProperty("print_width")]
        public int PrintWidth { get; set; }

        [JsonProperty("zoom")]
        public double Zoom { get; set; }
    }
}
