using Newtonsoft.Json;

namespace ArcDock.Data.Json
{
    /// <summary>
    /// Socket传输全局配置所用的JSON解析类
    /// </summary>
    [JsonObject]
    public class GlobalSettingJson
    {
        /// <summary>
        /// 是否启用模板检查规则
        /// </summary>
        [JsonProperty("enable_check")]
        public bool IsEnableCheck { get; set; }

        /// <summary>
        /// 打印API
        /// </summary>
        [JsonProperty("print_api")]
        public int PrintApi { get; set; }

        /// <summary>
        /// 解析字符串的Python代码
        /// </summary>
        [JsonProperty("code")]
        public string Code { get; set; }
    }
}
