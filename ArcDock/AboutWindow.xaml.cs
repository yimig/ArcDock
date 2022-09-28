using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
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
    /// 关于窗口
    /// </summary>
    public partial class AboutWindow : Window
    {
        #region 属性和字段

        /// <summary>
        /// 软件版本号
        /// </summary>
        public string Version
        {
            get => Assembly.GetExecutingAssembly().GetName().Version.ToString();
        }

        #endregion

        #region 初始化
        public AboutWindow()
        {
            this.DataContext = this;
            InitializeComponent();
        }

        #endregion

    }
}
