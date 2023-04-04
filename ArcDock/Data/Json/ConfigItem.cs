using System.Collections.Generic;
using System.ComponentModel;
using System.Runtime;
using ArcDock.Properties;
using Newtonsoft.Json;

namespace ArcDock.Data.Json
{
    /// <summary>
    /// 预留值配置
    /// </summary>
    [JsonObject]
    public class ConfigItem: INotifyPropertyChanged
    {
        public event PropertyChangedEventHandler PropertyChanged;
        private string id, name, type, @default, rules, defaultHeaderNode, defaultContentNode;
        private int optionType, maxFlow;
        private List<string> option;
        private List<OptionItem> optionItemList;

        /// <summary>
        /// 预留值Id
        /// 以此对应预留值配置和预留值，预留值在模板中用{{}}包裹
        /// </summary>
        [JsonProperty("id")]
        public string Id {
            get => id;
            set
            {
                id = value;
                if (PropertyChanged != null)
                {
                    PropertyChanged(this, new PropertyChangedEventArgs("Id"));
                }
            }
        }

        /// <summary>
        /// 预留值友好名称，显示在填写值旁边的提示信息
        /// </summary>
        [JsonProperty("name")]
        public string Name {
            get => name;
            set
            {
                name = value;
                if (PropertyChanged != null)
                {
                    PropertyChanged(this, new PropertyChangedEventArgs("Name"));
                }
            }

        }

        /// <summary>
        /// 控件类型
        /// 包括input文本框、richinput多行文本框、autoinput智能文本框
        /// </summary>
        [JsonProperty("type")]
        public string Type {
            get => type;
            set
            {
                type = value;
                if (PropertyChanged != null)
                {
                    PropertyChanged(this, new PropertyChangedEventArgs("Type"));
                }
            }
        }

        /// <summary>
        /// 补全选项类型：
        /// 0.无自动补全；
        /// 1.单框文本补全，系统检索Option的内容；
        /// 2.多框联动补全，系统检测OptionItemList的内容；
        /// </summary>
        [JsonProperty("opt_type")]
        public int OptionType {
            get => optionType;
            set
            {
                optionType = value;
                if (PropertyChanged != null)
                {
                    PropertyChanged(this, new PropertyChangedEventArgs("OptionType"));
                }
            }
        }

        /// <summary>
        /// 单框文本补全内容
        /// 提示文本以字符串数组存储
        /// </summary>
        [JsonProperty("option")]
        public List<string> Option {
            get => option;
            set
            {
                option = value;
                if (PropertyChanged != null)
                {
                    PropertyChanged(this, new PropertyChangedEventArgs("Option"));
                }
            }
        }

        /// <summary>
        /// 多框联动补全内容
        /// 根据本框的Content自动选择OptionItem的内容填写其他文本框
        /// </summary>
        [JsonProperty("option_exc")]
        public List<OptionItem> OptionItemList {
            get => optionItemList;
            set
            {
                optionItemList = value;
                if (PropertyChanged != null)
                {
                    PropertyChanged(this, new PropertyChangedEventArgs("OptionItemList"));
                }
            }
        }

        /// <summary>
        /// 默认值
        /// </summary>
        [JsonProperty("default")]
        public string Default {
            get => @default;
            set
            {
                @default = value;
                if (PropertyChanged != null)
                {
                    PropertyChanged(this, new PropertyChangedEventArgs("Default"));
                }
            }
        }

        /// <summary>
        /// 正则表达式规则，检测失败时阻止打印
        /// </summary>
        [JsonProperty("rules")]
        public string Rules {
            get => rules;
            set
            {
                rules = value;
                if (PropertyChanged != null)
                {
                    PropertyChanged(this, new PropertyChangedEventArgs("Rules"));
                }
            }
        }

        /// <summary>
        /// flowtable的默认html表头样式
        /// </summary>
        [JsonProperty("header_node")]
        public string DefaultHeaderNode {
            get => defaultHeaderNode;
            set
            {
                defaultHeaderNode = value;
                if (PropertyChanged != null)
                {
                    PropertyChanged(this, new PropertyChangedEventArgs("DefaultHeaderNode"));
                }
            }
        }

        /// <summary>
        /// flowtable的默认html内容样式
        /// </summary>
        [JsonProperty("content_node")]
        public string DefaultContentNode {
            get => defaultHeaderNode;
            set
            {
                defaultContentNode = value;
                if (PropertyChanged != null)
                {
                    PropertyChanged(this, new PropertyChangedEventArgs("DefaultContentNode"));
                }
            }
        }

        /// <summary>
        /// flowtable一页最多显示的数据数量
        /// </summary>
        [JsonProperty("max_flow")]
        public int MaxFlow {
            get => maxFlow;
            set
            {
                maxFlow = value;
                if (PropertyChanged != null)
                {
                    PropertyChanged(this, new PropertyChangedEventArgs("MaxFlow"));
                }
            }
        }
    }
}
