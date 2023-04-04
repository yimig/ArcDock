using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;
using System.Windows.Documents;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using System.Windows.Shapes;
using ArcDock.Data.Json;
using ArcDock.Data.UI.Converter;
using ArcDock.Properties;

namespace ArcDock
{
    /// <summary>
    /// TemplateWindow.xaml 的交互逻辑
    /// </summary>
    public partial class TemplateWindow : Window, INotifyPropertyChanged
    {
        public event PropertyChangedEventHandler PropertyChanged;
        private Config config;
        private ConfigItem currentConfigItem;
        private OptionItem currentOptionItem;
        public List<string> DisplayTextBoxTypeList
        {
            get
            {
                return new List<string>() { "普通文本框", "多行文本框", "智能文本框" };
            }
        }
        public List<string> DisplayFillTypeList
        {
            get
            {
                return new List<string>() { "无自动补全", "单框文本补全", "多框联动补全" };
            }
        }

        public ConfigItem CurrentConfigItem
        {
            get => currentConfigItem;
            set
            {
                currentConfigItem = value;
                if (PropertyChanged != null)
                {
                    PropertyChanged(this, new PropertyChangedEventArgs("CurrentConfigItem"));
                }
            }
        }

        public OptionItem CurrentOptionItem
        {
            get => currentOptionItem;
            set
            {
                currentOptionItem = value;
                if (PropertyChanged != null)
                {
                    PropertyChanged(this, new PropertyChangedEventArgs("CurrentOptionItem"));
                }
            }
        }

        public TemplateWindow(Config config)
        {
            this.config = config;
            this.Resources.Add("Config", config);
            InitializeComponent();
            LvConfig.SetBinding(ListView.ItemsSourceProperty,
                new Binding("ConfigItemList") { Source = this.config });
            CurrentConfigItem = config.ConfigItemList[0];
            this.DataContext = this;
            cbTypeTextBox.ItemsSource = DisplayTextBoxTypeList;
            cbTypeFill.ItemsSource = DisplayFillTypeList;
        }

        private void LvConfig_SelectionChanged(object sender, SelectionChangedEventArgs e)
        {
            if(e.AddedItems.Count > 0)
            {
                CurrentConfigItem = e.AddedItems[0] as ConfigItem;
                CurrentOptionItem = null;
            }
        }

        private void lvIndex_SelectionChanged(object sender, SelectionChangedEventArgs e)
        {
            if (e.AddedItems.Count > 0)
            {
                CurrentOptionItem = e.AddedItems[0] as OptionItem;
            }
        }
    }
}
