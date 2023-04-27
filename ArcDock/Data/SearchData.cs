using ArcDock.Data.Json;
using ArcDock.Tools;
using System.Collections.Generic;
using System.Linq;

namespace ArcDock.Data
{
    /// <summary>
    /// 自动补全搜索类
    /// </summary>
    public class SearchData
    {
        #region 字段和属性

        /// <summary>
        /// 自动补全条目集合
        /// </summary>
        private List<SearchDataItem> searchItems;

        #endregion

        #region 初始化
        public SearchData(ConfigItem configItem)
        {
            searchItems = new List<SearchDataItem>();
            if (configItem.OptionType == 1)
            {
                foreach (var item in configItem.Option)
                {
                    searchItems.Add(new SearchDataItem(item));
                }
            }
            else if (configItem.OptionType == 2)
            {
                foreach (var item in configItem.OptionItemList)
                {
                    searchItems.Add(new SearchDataItem(item.Content, item.ExecutionItemList));
                }
            }
        }

        #endregion

        #region 公共方法

        /// <summary>
        /// 搜索补全内容
        /// </summary>
        /// <param name="keyword">关键词</param>
        /// <returns>搜索结果集</returns>
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

        #endregion

    }
}
