using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using ArcDock.Data.Json;
using ArcDock.Tools;

namespace ArcDock.Data
{
    public class SearchDataSet
    {
        public ConfigItem sourceConfig;
        public List<SearchDataItem> searchItems;

        public SearchDataSet(ConfigItem configItem)
        {
            this.sourceConfig = configItem;
            searchItems = new List<SearchDataItem>();
            if (configItem.OptionType == 1)
            {
                foreach (var item in configItem.Option)
                {
                    searchItems.Add(new SearchDataItem(item));
                }
            } else if (configItem.OptionType == 2)
            {
                foreach (var item in configItem.OptionItemList)
                {
                    searchItems.Add(new SearchDataItem(item.Content,item.ExecutionItemList));
                }
            }
        }

        public List<SearchDataItem> GetResult(string keyword)
        {
            List<SearchDataItem> result;
            if (PinyinHelpers.IsAllChar(keyword))
            {
                result = searchItems.Where(py => py.PYHead.Contains(keyword.ToUpper())).ToList();
            }
            else
            {
                result = searchItems.Where(word => word.Text.Contains(keyword)).ToList();
            }

            return result;
        }
    }
}
