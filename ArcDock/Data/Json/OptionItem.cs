using System.Collections.Generic;
using System.ComponentModel;
using System.Runtime;
using ArcDock.Properties;
using Newtonsoft.Json;

namespace ArcDock.Data.Json
{
    /// <summary>
    /// 多框联动时的自动补全内容
    /// </summary>
    [JsonObject]
    public class OptionItem : INotifyPropertyChanged
    {
        public event PropertyChangedEventHandler PropertyChanged;
        private string content;
        private List<ExecutionItem> executionItemList;

        /// <summary>
        /// 提示内容，也是联动补全条件
        /// </summary>
        [JsonProperty("content")]
        public string Content {
            get => content;
            set
            {
                content = value;
                if (PropertyChanged != null)
                {
                    PropertyChanged(this, new PropertyChangedEventArgs("Content"));
                }
            }
        }

        /// <summary>
        /// 别的框操作指示
        /// </summary>
        [JsonProperty("exec")]
        public List<ExecutionItem> ExecutionItemList {
            get => executionItemList;
            set
            {
                executionItemList = value;
                if (PropertyChanged != null)
                {
                    PropertyChanged(this, new PropertyChangedEventArgs("ExecutionItemList"));
                }
            }
        }
    }
}
