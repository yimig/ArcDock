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
    /// 选择打印页数窗口
    /// </summary>
    public partial class SelectPrintNumWindow : Window, INotifyPropertyChanged
    {
        #region 属性和字段

        /// <summary>
        /// 打印页数
        /// </summary>
        private int pageNumber;

        /// <summary>
        /// 绑定触发事件
        /// </summary>
        public event PropertyChangedEventHandler PropertyChanged;

        /// <summary>
        /// 打印页数
        /// </summary>
        public int PageNumber
        {
            get => pageNumber;
            set
            {
                pageNumber = value;
                if (PropertyChanged != null) PropertyChanged(this, new PropertyChangedEventArgs("PageNumber"));
            }
        }

        /// <summary>
        /// 窗口关闭后是否应打印
        /// </summary>
        public bool IsPrint { get; set; }

        #endregion

        #region 初始化

        public SelectPrintNumWindow()
        {
            this.DataContext = this;
            InitializeComponent();
            this.PageNumber = 1;
            this.IsPrint = false;
        }

        #endregion

        #region 事件处理

        /// <summary>
        /// 增加打印页码按钮事件处理
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void BtnAdd_OnClick(object sender, RoutedEventArgs e)
        {
            this.PageNumber++;
        }

        /// <summary>
        /// 减少打印页码按钮事件处理
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void BtnSub_OnClick(object sender, RoutedEventArgs e)
        {
            PageNumber = PageNumber == 1 ? 1 : PageNumber - 1;
        }

        /// <summary>
        /// 取消打印按钮事件处理
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void BtnCancel_OnClick(object sender, RoutedEventArgs e)
        {
            this.Close();
        }

        /// <summary>
        /// 确认打印按钮事件处理
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void BtnOk_OnClick(object sender, RoutedEventArgs e)
        {
            this.IsPrint = true;
            this.Close();
        }

        #endregion

    }
}
