using System.Collections.Generic;
using System.Windows.Controls;

namespace ArcDock.Data.UI
{
    /// <summary>
    /// 智能文本框联想时下拉菜单单项的控件
    /// </summary>
    public class SearchItem : DockPanel
    {
        private TextBlock textBlock;

        /// <summary>
        /// 联想文本内容
        /// </summary>
        public string Text
        {
            get => textBlock.Text;
            set => textBlock.Text = value;
        }

        /// <summary>
        /// 搜索结果
        /// </summary>
        public SearchDataItem DataItem { get; set; }

        /// <summary>
        /// 新建一个联想下拉菜单项控件
        /// </summary>
        /// <param name="text"></param>
        /// <param name="dataItem"></param>
        public SearchItem(string text, SearchDataItem dataItem)
        {
            this.textBlock = new TextBlock() { Text = text };
            this.DataItem = dataItem;
            DockPanel.SetDock(textBlock, Dock.Left);
            this.Children.Add(textBlock);
        }

        public override string ToString()
        {
            return this.Text;
        }

        /// <summary>
        /// 获得联想搜索结果
        /// </summary>
        /// <param name="data"></param>
        /// <param name="keyword"></param>
        /// <returns></returns>
        public static List<SearchItem> GetSearchResult(SearchData data, string keyword)
        {
            List<SearchItem> result = new List<SearchItem>();
            foreach (var item in data.GetResult(keyword))
            {
                result.Add(new SearchItem(item.Text, item));
            }

            return result;

        }
    }
}
