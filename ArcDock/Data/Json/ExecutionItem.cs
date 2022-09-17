using Newtonsoft.Json;

namespace ArcDock.Data.Json
{
    /// <summary>
    /// 操作指示类，多框联动补全时填入别的框内容
    /// </summary>
    [JsonObject]
    public class ExecutionItem
    {
        /// <summary>
        /// 预留值ID
        /// </summary>
        [JsonProperty("key")]
        public string Key { get; set; }
        /// <summary>
        /// 对应自动填充内容
        /// </summary>
        [JsonProperty("content")]
        public string Content { get; set; }
    }
}
