using System.ComponentModel;
using System.Windows;

namespace ArcDock
{
    /// <summary>
    /// 打印准备窗口
    /// </summary>
    public partial class PrintProgressWindow : Window, INotifyPropertyChanged
    {
        private int progressValue;
        private string displayText;

        public event PropertyChangedEventHandler PropertyChanged;

        /// <summary>
        /// 提示文本
        /// </summary>
        public string DisplayText
        {
            get => displayText;
            set
            {
                displayText = value;
                if (PropertyChanged != null) PropertyChanged(this, new PropertyChangedEventArgs("DisplayText"));
            }
        }

        /// <summary>
        /// 初始化打印进度窗口
        /// </summary>
        public PrintProgressWindow()
        {
            this.DisplayText = "正在准备打印...";
            this.DataContext = this;
            InitializeComponent();
        }

        /// <summary>
        /// 改变进度
        /// </summary>
        /// <param name="progressValue">进度值0-100</param>
        /// <param name="displayText">提示文本</param>
        public void ChangeStatue(int progressValue, string displayText)
        {
            PbState.IsIndeterminate = false;
            PbState.Value = progressValue;
            this.DisplayText = displayText;
        }
    }
}
