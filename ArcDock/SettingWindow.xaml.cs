using ArcDock.Data;
using ArcDock.Data.Json;
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

namespace ArcDock
{
    /// <summary>
    /// 设置窗口
    /// </summary>
    public partial class SettingWindow : Window, INotifyPropertyChanged
    {
        #region 属性和字段

        private bool isEnableRules;
        private Config config;

        /// <summary>
        /// 目前选中的打印API
        /// </summary>
        public int PrintApi { get; set; }

        public bool IsEnableRules
        {
            get => isEnableRules;
            set
            {
                isEnableRules = value;
                if (PropertyChanged != null) PropertyChanged(this, new PropertyChangedEventArgs("IsEnableRules"));
            }
        }

        /// <summary>
        /// 绑定事件触发
        /// </summary>
        public event PropertyChangedEventHandler PropertyChanged;

        #endregion

        #region 初始化

        public SettingWindow(Config config,int printApi, bool isEnableRules)
        {
            this.config = config;
            PrintApi = printApi;
            IsEnableRules = isEnableRules;
            DataContext = this;
            InitializeComponent();
            InitPrintApi();
        }

        /// <summary>
        /// 初始化打印API选项
        /// </summary>
        private void InitPrintApi()
        {
            if (PrintApi == 0) RbPrintDocument.IsChecked = true;
            else if (PrintApi == 1) RbCefPrint.IsChecked = true;
            else if (PrintApi == 2) RbClodop.IsChecked = true;
            else if (PrintApi == 3) RbPdf.IsChecked = true;
            else if (PrintApi == 4) RbSpire.IsChecked = true;
        }

        #endregion

        #region 功能解耦

        /// <summary>
        /// 检查目前选中的打印API
        /// </summary>
        private void CheckPrintApi()
        {
            if ((bool)RbPrintDocument.IsChecked) PrintApi = 0;
            else if ((bool)RbCefPrint.IsChecked) PrintApi = 1;
            else if ((bool)RbClodop.IsChecked) PrintApi = 2;
            else if ((bool)RbPdf.IsChecked) PrintApi = 3;
            else if ((bool)RbSpire.IsChecked) PrintApi = 4;
        }

        #endregion

        #region 事件处理

        /// <summary>
        /// 按下任意打印API单选按钮的事件处理
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void RbPrint_OnChecked(object sender, RoutedEventArgs e)
        {
            CheckPrintApi();
        }

        #endregion

        private void BtnCodeEditer_Click(object sender, RoutedEventArgs e)
        {
            new CodeEditerWindow(config).ShowDialog();
        }

        private void BtnTestCode_Click(object sender, RoutedEventArgs e)
        {
            new CodeTestWindow(PythonEnvironment.GetPythonCode()).ShowDialog(); 
        }
    }
}
