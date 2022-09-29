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
    /// 打印准备窗口
    /// </summary>
    public partial class PrintProgressWindow : Window, INotifyPropertyChanged
    {
        private int progressValue;
        private string displayText;

        public event PropertyChangedEventHandler PropertyChanged;

        public string DisplayText
        {
            get => displayText;
            set{
                displayText = value;
                if (PropertyChanged != null) PropertyChanged(this, new PropertyChangedEventArgs("DisplayText"));
            }
        }
        public PrintProgressWindow()
        {
            this.DisplayText = "正在准备打印...";
            this.DataContext = this;
            InitializeComponent();
        }

        public void ChangeStatue(int progressValue,string displayText)
        {
            PbState.IsIndeterminate = false;
            PbState.Value = progressValue;
            this.DisplayText = displayText;
        }
    }
}
