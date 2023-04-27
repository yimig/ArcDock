using Newtonsoft.Json;
using System.Collections.Generic;

namespace ArcDock.Data.Json
{
    /// <summary>
    /// 进程间数据传输的对象
    /// </summary>
    [JsonObject]
    public class ProcessData
    {
        /// <summary>
        /// 此数据是否已经被处理
        /// </summary>
        [JsonProperty("handled")]
        public bool IsHandled { get; set; }

        /// <summary>
        /// 是否静默打印
        /// </summary>
        [JsonProperty("silent")]
        public bool IsSilent { get; set; }

        /// <summary>
        /// 是否启用联想（未实现，默认全是）
        /// </summary>
        [JsonProperty("associate")]
        public bool IsAssociate { get; set; }

        /// <summary>
        /// 模板文件名称
        /// </summary>
        [JsonProperty("template")]
        public string TemplateName { get; set; }

        /// <summary>
        /// 数据参数，键是配置ID
        /// </summary>
        [JsonProperty("arguments")]
        public Dictionary<string, string> Arguments { get; set; }

    }
}
