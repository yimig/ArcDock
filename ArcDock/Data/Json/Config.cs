using System.Collections.Generic;
using Newtonsoft.Json;

namespace ArcDock.Data.Json
{
    /// <summary>
    /// 主模板配置类
    /// </summary>
    [JsonObject]
    public class Config
    {
        /// <summary>
        /// 全局配置项
        /// </summary>
        [JsonProperty("settings")]
        public Setting Settings { get; set; }

        /// <summary>
        /// 预留值配置集合
        /// </summary>
        [JsonProperty("data")]
        public List<ConfigItem> ConfigItemList { get; set; }

        [JsonIgnore]
        public string FilePath { get; set; }
    }
}
