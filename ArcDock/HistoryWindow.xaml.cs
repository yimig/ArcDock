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
        private string maxPage;


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
                if (PropertyChanged != null) PropertyChanged(this, new PropertyChangedEventArgs("NowPage"));
            }
        }

        public string MaxPage
        {
            get => maxPage;
            set
            {
                maxPage = value;
                if (PropertyChanged != null) PropertyChanged(this, new PropertyChangedEventArgs("MaxPage"));
            }
        }

        public HistoryWindow(History history)
        {
            this.history = history;
            this.nowPage = 1;
            InitializeComponent();
            Results = history.GetPage(1);
            ResetMaxPage();
            LvHistory.SetBinding(ListView.ItemsSourceProperty, new Binding("Results") {Source = this});
            TbNowPage.SetBinding(TextBlock.TextProperty, new Binding("NowPage"){Source = this});
            TbMaxPage.SetBinding(TextBlock.TextProperty, new Binding("MaxPage"){Source = this});
        }

        private void ResetMaxPage()
        {
            var pageNum = 1;
            if (!isSearchFlag) pageNum = history.MaxPage;
            MaxPage = pageNum.ToString();
        }

        private void SearchItem()
        {
            if (TbSearch.Text.Equals(String.Empty))
            {
                isSearchFlag = false;
                Results = history.GetPage(1);
                ResetMaxPage();
            }
            else
            {
                isSearchFlag = true;
                Results = history.GetQueryResult(TbSearch.Text);
                MaxPage = "1";
                NowPage = 1;
            }
        }

        private void BtnForwardPage_OnClick(object sender, RoutedEventArgs e)
        {
            if (NowPage - 1 >= 1)
            {
                NowPage--;
                Results = history.GetPage(NowPage);
            }
        }

        private void BtnNextPage_OnClick(object sender, RoutedEventArgs e)
        {
            if (NowPage + 1 <= Convert.ToInt32(MaxPage))
            {
                NowPage++;
                Results = history.GetPage(NowPage);
                
            }
        }

        private void BtnSearch_OnClick(object sender, RoutedEventArgs e)
        {
            SearchItem();
        }

        private void TbSearch_OnKeyDown(object sender, KeyEventArgs e)
        {
            if (e.Key == Key.Enter)
            {
                SearchItem();
            }
        }

        private void OnListViewItemDoubleClick(object sender, MouseButtonEventArgs e)
        {
            var selectedId = ((sender as ListViewItem).Content as DataResult).ItemId;
            var results = history.GetFullItemData(selectedId);
            (new ItemDataWindow(results)).ShowDialog();
        }
    }
}
