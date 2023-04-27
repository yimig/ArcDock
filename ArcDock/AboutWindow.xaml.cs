using System.Reflection;
using System.Windows;

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
