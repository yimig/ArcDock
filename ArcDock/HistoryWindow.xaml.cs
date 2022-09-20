using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;
using System.Runtime.CompilerServices;
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
using ArcDock.Data;
using ArcDock.Data.Database;

namespace ArcDock
{
    /// <summary>
    /// HistoryWindow.xaml 的交互逻辑
    /// </summary>
    public partial class HistoryWindow : Window, INotifyPropertyChanged
    {
        private History history;
        private List<DataResult> results;
        public event PropertyChangedEventHandler PropertyChanged;
        private bool isSearchFlag = false;
        private int nowPage;


        public List<DataResult> Results
        {
            get => results;
            set
            {
                results = value;
                if (PropertyChanged != null) PropertyChanged(this, new PropertyChangedEventArgs("Results"));
            }
        }

        public int NowPage
        {
            get => nowPage;
            set
            {
                nowPage = value;
                if (PropertyChanged != null) PropertyChanged(this, new PropertyChangedEventArgs("Results"));
            }
        }

        public string MaxPage
        {
            get
            {
                var pageNum = 1;
                if (!isSearchFlag) pageNum = history.MaxPage;
                return pageNum.ToString();
            }
        }

        public HistoryWindow(History history)
        {
            this.history = history;
            this.nowPage = 1;
            InitializeComponent();
            Results = history.GetPage(1);
            LvHistory.SetBinding(ListView.ItemsSourceProperty, new Binding() {Source = Results});
            TbNowPage.SetBinding(TextBox.TextProperty, new Binding("MaxPage"));
        }
    }
}
