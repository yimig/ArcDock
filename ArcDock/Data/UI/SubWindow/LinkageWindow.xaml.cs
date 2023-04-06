using ArcDock.Data.Json;
using ArcDock.Data.UI.Converter;
using System;
using System.Collections.Generic;
using System.ComponentModel;
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

namespace ArcDock.Data.UI.SubWindow
{
    /// <summary>
    /// Interaction logic for LinkageWindow.xaml
    /// </summary>
    public partial class LinkageWindow : Window, INotifyPropertyChanged
    {
        public event PropertyChangedEventHandler PropertyChanged;
        private ExecutionItem executionItem;
        private OptionItem optionItem;
        private Config config;
        private bool isCreate;
        public ExecutionItem ExecutionItem
        {
            get => executionItem;
            set
            {
                executionItem = value;
                if (PropertyChanged != null)
                {
                    PropertyChanged(this, new PropertyChangedEventArgs("ExecutionItem"));
                }
            }
        }

        public bool IsCheck { get; set; }
        public LinkageWindow(Config config, OptionItem optionItem, ExecutionItem executionItem, bool isCreate)
        {
            this.Resources.Add("Config", config);
            InitializeComponent();
            IsCheck = false;
            this.isCreate = isCreate;
            this.optionItem = optionItem;
            this.config = config;
            ExecutionItem = executionItem.Clone() as ExecutionItem;
            this.DataContext = this;
            CbIndex.ItemsSource = config.ConfigItemList.Select(i=>i.Name).ToList();
        }

        private bool CheckOptionRule()
        {
            if(ExecutionItem.Key == null)
            {
                MessageBox.Show("标题不可为空，请修改");
                return false;
            } else if (optionItem.ExecutionItemList.Any(i => i.Key == ExecutionItem.Key))
            {
                var optionIndexDict = config.ConfigItemList.ToDictionary(i => i.Id, i => i.Name);
                MessageBox.Show("当前联动值目录已包含标题【" + optionIndexDict[ExecutionItem.Key] + "】,不允许重复添加。如要修改，请双击需要修改的项目。");
                return false;
            }
            return true;
        }

        private void btnCheck_Click(object sender, RoutedEventArgs e)
        {
            if(!isCreate||CheckOptionRule())
            {
                IsCheck = true;
                this.Close();
            } 

        }
    }
}
