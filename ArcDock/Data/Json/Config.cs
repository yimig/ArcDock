using System.Collections.Generic;
using System.ComponentModel;
using Newtonsoft.Json;

namespace ArcDock.Data.Json
{
    /// <summary>
    /// 主模板配置类
    /// </summary>
    [JsonObject]
    public class Config:INotifyPropertyChanged
    {
        public event PropertyChangedEventHandler PropertyChanged;
        private Setting settings;
        private List<ConfigItem> configItemList;
        private string filePath;

        /// <summary>
        /// 全局配置项
        /// </summary>
        [JsonProperty("settings")]
        public Setting Settings { 
            get=> settings;
            set
            {
                settings = value;
                if (PropertyChanged != null)
                {
                    PropertyChanged(this, new PropertyChangedEventArgs("Settings"));
                }
            }
        }

        /// <summary>
        /// 预留值配置集合
        /// </summary>
        [JsonProperty("data")]
        public List<ConfigItem> ConfigItemList {
            get => configItemList; 
            set
            {
                configItemList = value;
                if (PropertyChanged != null)
                {
                    PropertyChanged(this, new PropertyChangedEventArgs("ConfigItemList"));
                }
            }
        }

        [JsonIgnore]
        /// <summary>
        /// html模板文件地址
        /// </summary>
        public string FilePath { 
            get=>filePath;
            set
            {
                filePath = value;
                if (PropertyChanged != null)
                {
                    PropertyChanged(this, new PropertyChangedEventArgs("FilePath"));
                }
            }
        }
    }
}
