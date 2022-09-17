using System.Collections.Generic;
using Newtonsoft.Json;

namespace ArcDock.Data.Json
{
    /// <summary>
    /// 多框联动时的自动补全内容
    /// </summary>
    [JsonObject]
    public class OptionItem
    {
        /// <summary>
        /// 提示内容，也是联动补全条件
        /// </summary>
        [JsonProperty("content")]
        public string Content { get; set; }

        /// <summary>
        /// 别的框操作指示
        /// </summary>
        [JsonProperty("exec")]
        public List<ExecutionItem> ExecutionItemList { get; set; }
    }
}
