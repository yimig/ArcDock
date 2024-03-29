﻿using Newtonsoft.Json;
using System;
using System.ComponentModel;

namespace ArcDock.Data.Json
{
    /// <summary>
    /// 操作指示类，多框联动补全时填入别的框内容
    /// </summary>
    [JsonObject]
    public class ExecutionItem : INotifyPropertyChanged, ICloneable
    {
        public event PropertyChangedEventHandler PropertyChanged;
        private string key, content;

        /// <summary>
        /// 预留值ID
        /// </summary>
        [JsonProperty("key")]
        public string Key
        {
            get => key;
            set
            {
                key = value;
                if (PropertyChanged != null)
                {
                    PropertyChanged(this, new PropertyChangedEventArgs("Key"));
                }
            }
        }
        /// <summary>
        /// 对应自动填充内容
        /// </summary>
        [JsonProperty("content")]
        public string Content
        {
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
        /// 深克隆本对象
        /// </summary>
        /// <returns></returns>
        public object Clone()
        {
            return this.MemberwiseClone();
        }
    }
}
