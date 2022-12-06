using System.Collections.Generic;
using Newtonsoft.Json;

namespace ArcDock.Data.Json
{
    /// <summary>
    /// 预留值配置
    /// </summary>
    [JsonObject]
    public class ConfigItem
    {
        /// <summary>
        /// 预留值Id
        /// 以此对应预留值配置和预留值，预留值在模板中用{{}}包裹
        /// </summary>
        [JsonProperty("id")]
        public string Id { get; set; }

        /// <summary>
        /// 预留值友好名称，显示在填写值旁边的提示信息
        /// </summary>
        [JsonProperty("name")]
        public string Name { get; set; }

        /// <summary>
        /// 控件类型
        /// 包括input文本框、richinput多行文本框、dateinput时间文本框
        /// </summary>
        [JsonProperty("type")]
        public string Type { get; set; }

        /// <summary>
        /// 补全选项类型：
        /// 0.无自动补全；
        /// 1.单框文本补全，系统检索Option的内容；
        /// 2.多框联动补全，系统检测OptionItemList的内容；
        /// </summary>
        [JsonProperty("opt_type")]
        public int OptionType { get; set; }

        /// <summary>
        /// 单框文本补全内容
        /// 提示文本以字符串数组存储
        /// </summary>
        [JsonProperty("option")]
        public List<string> Option { get; set; }

        /// <summary>
        /// 多框联动补全内容
        /// 根据本框的Content自动选择OptionItem的内容填写其他文本框
        /// </summary>
        [JsonProperty("option_exc")]
        public List<OptionItem> OptionItemList { get; set; }

        /// <summary>
        /// 默认值
        /// </summary>
        [JsonProperty("default")]
        public string Default { get; set; }

        /// <summary>
        /// 正则表达式规则，检测失败时阻止打印
        /// </summary>
        [JsonProperty("rules")]
        public string Rules { get; set; }

        /// <summary>
        /// flowtable的默认html表头样式
        /// </summary>
        [JsonProperty("header_node")]
        public string DefaultHeaderNode { get; set; }

        /// <summary>
        /// flowtable的默认html内容样式
        /// </summary>
        [JsonProperty("content_node")]
        public string DefaultContentNode { get; set; }

        /// <summary>
        /// flowtable一页最多显示的数据数量
        /// </summary>
        [JsonProperty("max_flow")]
        public int MaxFlow { get; set; }
    }
}
