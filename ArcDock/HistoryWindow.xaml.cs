using ArcDock.Data.Database;
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;
using System.Windows.Input;

namespace ArcDock
{
    /// <summary>
    /// 查看历史纪录窗口
    /// </summary>
    public partial class HistoryWindow : Window, INotifyPropertyChanged
    {
        #region 属性字段和事件

        /// <summary>
        /// 历史记录数据库连接类
        /// </summary>
        private History history;

        /// <summary>
        /// 查询结果
        /// </summary>
        private List<SortResult> results;

        /// <summary>
        /// 绑定事件触发
        /// </summary>
        public event PropertyChangedEventHandler PropertyChanged;

        /// <summary>
        /// 是否搜索状态标志
        /// </summary>
        private bool isSearchFlag = false;

        /// <summary>
        /// 目前页码
        /// </summary>
        private int nowPage;

        /// <summary>
        /// 最大页码
        /// </summary>
        private string maxPage;

        /// <summary>
        /// 查询结果
        /// </summary>
        public List<SortResult> Results
        {
            get => results;
            set
            {
                results = value;
                if (PropertyChanged != null) PropertyChanged(this, new PropertyChangedEventArgs("Results"));
            }
        }

        /// <summary>
        /// 目前页码
        /// </summary>
        public int NowPage
        {
            get => nowPage;
            set
            {
                nowPage = value;
                if (PropertyChanged != null) PropertyChanged(this, new PropertyChangedEventArgs("NowPage"));
            }
        }

        /// <summary>
        /// 最大页码
        /// </summary>
        public string MaxPage
        {
            get => maxPage;
            set
            {
                maxPage = value;
                if (PropertyChanged != null) PropertyChanged(this, new PropertyChangedEventArgs("MaxPage"));
            }
        }

        #endregion

        #region 初始化

        public HistoryWindow(History history)
        {
            this.history = history;
            this.nowPage = 1;
            InitializeComponent();
            Results = history.GetSortPage(1);
            ResetMaxPage();
            LvHistory.SetBinding(ListView.ItemsSourceProperty, new Binding("Results") { Source = this });
            TbNowPage.SetBinding(TextBlock.TextProperty, new Binding("NowPage") { Source = this });
            TbMaxPage.SetBinding(TextBlock.TextProperty, new Binding("MaxPage") { Source = this });
        }

        #endregion

        #region 功能解耦

        /// <summary>
        /// 重置最大页码
        /// </summary>
        private void ResetMaxPage()
        {
            var pageNum = 1;
            if (!isSearchFlag) pageNum = history.MaxPage;
            MaxPage = pageNum.ToString();
        }

        /// <summary>
        /// 搜索历史
        /// </summary>
        private void SearchItem()
        {
            if (TbSearch.Text.Equals(String.Empty))
            {
                isSearchFlag = false;
                Results = history.GetSortPage(1);
                ResetMaxPage();
            }
            else
            {
                isSearchFlag = true;
                Results = history.GetSortQueryResult(TbSearch.Text);
                MaxPage = "1";
                NowPage = 1;
            }
        }

        #endregion

        #region 事件处理

        /// <summary>
        /// 上一页按钮事件处理
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void BtnForwardPage_OnClick(object sender, RoutedEventArgs e)
        {
            if (NowPage - 1 >= 1)
            {
                NowPage--;
                Results = history.GetSortPage(NowPage);
            }
        }

        /// <summary>
        /// 下一页按钮事件处理
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void BtnNextPage_OnClick(object sender, RoutedEventArgs e)
        {
            if (NowPage + 1 <= Convert.ToInt32(MaxPage))
            {
                NowPage++;
                Results = history.GetSortPage(NowPage);

            }
        }

        /// <summary>
        /// 搜索按钮事件处理
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void BtnSearch_OnClick(object sender, RoutedEventArgs e)
        {
            SearchItem();
        }

        /// <summary>
        /// 搜索栏输入事件处理
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void TbSearch_OnKeyDown(object sender, KeyEventArgs e)
        {
            if (e.Key == Key.Enter)
            {
                SearchItem();
            }
        }

        /// <summary>
        /// 双击搜索项事件处理
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void OnListViewItemDoubleClick(object sender, MouseButtonEventArgs e)
        {
            var selectedId = ((sender as ListViewItem).Content as SortResult).ItemId;
            var results = history.GetFullItemData(selectedId);
            (new ItemDataWindow(results)).ShowDialog();
        }

        #endregion

    }
}
