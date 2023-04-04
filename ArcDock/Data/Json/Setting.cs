using ArcDock.Properties;
using Newtonsoft.Json;
using System.ComponentModel;

namespace ArcDock.Data.Json
{
    /// <summary>
    /// 全局配置类
    /// 模板内容可以替换config.json的内容
    /// </summary>
    [JsonObject]
    public class Setting : INotifyPropertyChanged
    {
        public event PropertyChangedEventHandler PropertyChanged;
        private string printer, printUnit;
        private int height, width, printHeight, printWidth;
        private double zoom;
        private bool fixedHeader;

        /// <summary>
        /// 默认打印机名称
        /// </summary>
        [JsonProperty("printer")]
        public string Printer {
            get => printer;
            set
            {
                printer = value;
                if (PropertyChanged != null)
                {
                    PropertyChanged(this, new PropertyChangedEventArgs("Printer"));
                }
            }
        }

        /// <summary>
        /// 纵向像素数量
        /// </summary>
        [JsonProperty("height")]
        public int Height {
            get => height;
            set
            {
                height = value;
                if (PropertyChanged != null)
                {
                    PropertyChanged(this, new PropertyChangedEventArgs("Height"));
                }
            }
        }

        /// <summary>
        /// 横向像素数量
        /// </summary>
        [JsonProperty("width")]
        public int Width {
            get => width;
            set
            {
                width = value;
                if (PropertyChanged != null)
                {
                    PropertyChanged(this, new PropertyChangedEventArgs("Width"));
                }
            }
        }

        /// <summary>
        /// 打印单位
        /// </summary>
        [JsonProperty("print_unit")]
        public string PrintUnit {
            get => printUnit;
            set
            {
                printUnit = value;
                if (PropertyChanged != null)
                {
                    PropertyChanged(this, new PropertyChangedEventArgs("PrintUnit"));
                }
            }
        }

        /// <summary>
        /// 打印高度
        /// </summary>
        [JsonProperty("print_height")]
        public int PrintHeight {
            get => printHeight;
            set
            {
                printHeight = value;
                if (PropertyChanged != null)
                {
                    PropertyChanged(this, new PropertyChangedEventArgs("PrintHeight"));
                }
            }
        }

        /// <summary>
        /// 打印宽度
        /// </summary>
        [JsonProperty("print_width")]
        public int PrintWidth {
            get => printWidth;
            set
            {
                printWidth = value;
                if (PropertyChanged != null)
                {
                    PropertyChanged(this, new PropertyChangedEventArgs("PrintWidth"));
                }
            }
        }

        /// <summary>
        /// 默认显示的放大系数
        /// </summary>
        [JsonProperty("zoom")]
        public double Zoom {
            get => zoom;
            set
            {
                zoom = value;
                if (PropertyChanged != null)
                {
                    PropertyChanged(this, new PropertyChangedEventArgs("Zoom"));
                }
            }
        }

        /// <summary>
        /// 是否每页都固定生成表头
        /// </summary>
        [JsonProperty("fixed_header")]
        public bool FixedHeader {
            get => fixedHeader;
            set
            {
                fixedHeader = value;
                if (PropertyChanged != null)
                {
                    PropertyChanged(this, new PropertyChangedEventArgs("FixedHeader"));
                }
            }
        }
    }
}
