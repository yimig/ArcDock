using System.ComponentModel;
using System.Windows;

namespace ArcDock.Data.UI.SubWindow
{
    /// <summary>
    /// 输入框窗口
    /// </summary>
    public partial class InputWindow : Window, INotifyPropertyChanged
    {
        public event PropertyChangedEventHandler PropertyChanged;
        private string data;

        /// <summary>
        /// 输入内容
        /// </summary>
        public string Data
        {
            get => data;
            set
            {
                data = value;
                if (PropertyChanged != null)
                {
                    PropertyChanged(this, new PropertyChangedEventArgs("Data"));
                }
            }
        }

        /// <summary>
        /// 用户是否点击确认
        /// </summary>
        public bool IsCheck { get; set; }

        public InputWindow(string data)
        {
            InitializeComponent();
            IsCheck = false;
            Data = data;
            this.DataContext = this;
        }

        /// <summary>
        /// 用户按下确认的事件处理
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void BtnCheck_Click(object sender, RoutedEventArgs e)
        {
            IsCheck = true;
            this.Close();
        }
    }
}
