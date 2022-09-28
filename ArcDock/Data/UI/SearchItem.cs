using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Controls;
using ArcDock.Data.Json;

namespace ArcDock.Data.UI
{
    public class SearchItem: DockPanel
    {
        private TextBlock textBlock;

        public string Text
        {
            get => textBlock.Text;
            set => textBlock.Text = value;
        }

        public SearchDataItem DataItem { get; set; }

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

        public static List<SearchItem> GetSearchResult(SearchData data, string keyword)
        {
            List<SearchItem> result = new List<SearchItem>();
            foreach (var item in data.GetResult(keyword))
            {
                result.Add(new SearchItem(item.Text,item));
            }

            return result;

        }
    }
}
