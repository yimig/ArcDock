using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Web.UI.WebControls;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;
using System.Windows.Documents;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using System.Windows.Shapes;
using System.Xml;
using ArcDock.Data;
using ArcDock.Data.Json;
using ArcDock.Data.UI.Converter;
using ArcDock.Data.UI.SubWindow;
using ArcDock.Properties;
using Newtonsoft.Json;
using System.Collections.ObjectModel;
using System.IO.Packaging;

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
        private bool isChanged = false;
        public List<string> DisplayTextBoxTypeList
        {
            get
            {
                return new List<string>() { "普通文本框", "多行文本框", "智能文本框", "JSON对象" };
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

        private void SaveConfig()
        {
            TextReader tReader = new StreamReader(new FileStream(config.FilePath, FileMode.Open));
            var templateHtml = tReader.ReadToEnd();
            tReader.Close();
            XmlDocument xDoc = new XmlDocument();
            xDoc.LoadXml(templateHtml);
            XmlNode configNode = xDoc.SelectSingleNode("//script[@type=\"config/json\"]");
            configNode.InnerText = JsonConvert.SerializeObject(config);
            var text = xDoc.OuterXml;
            TextWriter tWriter = new StreamWriter(new FileStream(config.FilePath, FileMode.Create));
            tWriter.Write(text);
            tWriter.Close();
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

        private void BtnAddIndex_Click(object sender, RoutedEventArgs e)
        {
            var newItem = new OptionItem();
            newItem.ExecutionItemList = new ObservableCollection<ExecutionItem>();
            var wndInput = new InputWindow(newItem.Content);
            wndInput.ShowDialog();
            if(wndInput.IsCheck)
            {
                newItem.Content = wndInput.Data;
                CurrentConfigItem.OptionItemList.Add(newItem);
                isChanged = true;
            }

        }

        private void BtnRemoveIndex_Click(object sender, RoutedEventArgs e)
        {
            if(lvIndex.SelectedItem != null)
            {
                var target = lvIndex.SelectedItem as OptionItem;
                if(MessageBox.Show("确定要删除【" +target.Content+"】吗？","删除确认",MessageBoxButton.OKCancel,MessageBoxImage.Warning) == MessageBoxResult.OK)
                {
                    CurrentConfigItem.OptionItemList.Remove(target);
                    isChanged = true;
                }

            }
        }

        private void BtnAddLinkage_Click(object sender, RoutedEventArgs e)
        {
            if (lvIndex.SelectedItem != null)
            {
                var newItem = new ExecutionItem();
                var wndLinkage = new LinkageWindow(config, currentOptionItem, newItem, true);
                wndLinkage.ShowDialog();
                if (wndLinkage.IsCheck)
                {
                    CurrentOptionItem.ExecutionItemList.Add(wndLinkage.ExecutionItem);
                    isChanged = true;
                }
            } else {
                MessageBox.Show("请选中一个索引再添加联动值");
            }


        }

        private void BtnRemoveLinkage_Click(object sender, RoutedEventArgs e)
        {
            if (lvLinkage.SelectedItem != null)
            {
                var optionIndexDict = config.ConfigItemList.ToDictionary(i => i.Id, i => i.Name);
                var target = lvLinkage.SelectedItem as ExecutionItem;
                if (MessageBox.Show("确定要删除【" + optionIndexDict[target.Key] + "】吗？", "删除确认", MessageBoxButton.OKCancel, MessageBoxImage.Warning) == MessageBoxResult.OK)
                {
                    currentOptionItem.ExecutionItemList.Remove(target);
                    isChanged = true;
                }

            }
        }

        private void BtnAddNormalIndex_Click(object sender, RoutedEventArgs e)
        {
            var wndInput = new InputWindow("");
            wndInput.ShowDialog();
            if (wndInput.IsCheck)
            {
                CurrentConfigItem.Option.Add(wndInput.Data);
                isChanged = true;
            }
        }

        private void BtnRemoveNormalIndex_Click(object sender, RoutedEventArgs e)
        {
            if (lvNormalIndex.SelectedItem != null)
            {
                var target = lvNormalIndex.SelectedItem as String;
                if (MessageBox.Show("确定要删除【" + target + "】吗？", "删除确认", MessageBoxButton.OKCancel, MessageBoxImage.Warning) == MessageBoxResult.OK)
                {
                    CurrentConfigItem.Option.Remove(target);
                    isChanged = true;
                }

            }
        }

        private void cbTypeTextBox_SelectionChanged(object sender, SelectionChangedEventArgs e)
        {
            if (CurrentConfigItem!= null && !e.AddedItems[0].ToString().Equals("智能文本框"))
            {
                CurrentConfigItem.OptionType = 0;
            }
        }

        private void lvIndex_MouseDoubleClick(object sender, MouseButtonEventArgs e)
        {
            var optionItem = (e.Source as ListViewItem).Content as OptionItem;
            var wndInput = new InputWindow(optionItem.Content);
            wndInput.ShowDialog();
            if (wndInput.IsCheck)
            {
                optionItem.Content = wndInput.Data;
                isChanged = true;
            }
        }

        private void lvLinkage_MouseDoubleClick(object sender, MouseButtonEventArgs e)
        {
            var executionItem = (e.Source as ListViewItem).Content as ExecutionItem;
            var wndInput = new LinkageWindow(config, CurrentOptionItem, executionItem, false);
            wndInput.ShowDialog();
            if (wndInput.IsCheck)
            {
                executionItem.Key = wndInput.ExecutionItem.Key;
                executionItem.Content = wndInput.ExecutionItem.Content;
                isChanged = true;
            }
        }

        private void lvNormalIndex_MouseDoubleClick(object sender, MouseButtonEventArgs e)
        {
            var optStr = (e.Source as ListViewItem).Content as String;
            var wndInput = new InputWindow(optStr);
            wndInput.ShowDialog();
            if (wndInput.IsCheck)
            {
                var index = CurrentConfigItem.Option.IndexOf(optStr);
                CurrentConfigItem.Option[index] = wndInput.Data;
                isChanged = true;
            }
        }

        private void btnSave_Click(object sender, RoutedEventArgs e)
        {
            try
            {
                SaveConfig();
                MessageBox.Show("配置已保存,将重启程序以应用修改。");
                System.Windows.Forms.Application.Restart();
                Environment.Exit(0);
            } catch(Exception ex)
            {
                MessageBox.Show("文件操作失败："+ex.Message);
            }

        }

        private void Box_MouseDown(object sender, MouseButtonEventArgs e)
        {
            isChanged = true;
        }

        private void Window_Closing(object sender, CancelEventArgs e)
        {
            if (isChanged)
            {
                if (MessageBox.Show("数据已经发生了改变，要保存保存修改吗？", "提示", MessageBoxButton.OKCancel) == MessageBoxResult.OK)
                {
                    SaveConfig();
                }
                MessageBox.Show("此操作将会重启应用");
                System.Windows.Forms.Application.Restart();
                Environment.Exit(0);
            }
        }
    }
}
