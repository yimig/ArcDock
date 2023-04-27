using ArcDock.Data.Json;
using System.ComponentModel;
using System.Linq;
using System.Windows;

namespace ArcDock.Data.UI.SubWindow
{
    /// <summary>
    /// Interaction logic for LinkageWindow.xaml
    /// </summary>
    public partial class LinkageWindow : Window, INotifyPropertyChanged
    {
        public event PropertyChangedEventHandler PropertyChanged;
        /// <summary>
        /// 当前配置项
        /// </summary>
        private ExecutionItem executionItem;
        /// <summary>
        /// 当前配置的填充内容
        /// </summary>
        private OptionItem optionItem;
        /// <summary>
        /// 当前配置
        /// </summary>
        private Config config;
        /// <summary>
        /// 是否为新建属性
        /// </summary>
        private bool isCreate;
        /// <summary>
        /// 当前配置的动态填充属性
        /// </summary>
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
        /// <summary>
        /// 用户是否点击确认
        /// </summary>
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
            CbIndex.ItemsSource = config.ConfigItemList.Select(i => i.Name).ToList();
        }
        /// <summary>
        /// 检查用户选中配置标题是否重复
        /// </summary>
        /// <returns></returns>
        private bool CheckOptionRule()
        {
            if (ExecutionItem.Key == null)
            {
                MessageBox.Show("标题不可为空，请修改");
                return false;
            }
            else if (optionItem.ExecutionItemList.Any(i => i.Key == ExecutionItem.Key))
            {
                var optionIndexDict = config.ConfigItemList.ToDictionary(i => i.Id, i => i.Name);
                MessageBox.Show("当前联动值目录已包含标题【" + optionIndexDict[ExecutionItem.Key] + "】,不允许重复添加。如要修改，请双击需要修改的项目。");
                return false;
            }
            return true;
        }
        /// <summary>
        /// 点击确认的事件处理
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void btnCheck_Click(object sender, RoutedEventArgs e)
        {
            if (!isCreate || CheckOptionRule())
            {
                IsCheck = true;
                this.Close();
            }

        }
    }
}
