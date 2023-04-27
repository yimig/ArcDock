using ArcDock.Data.Json;
using ArcDock.Tools;
using System.Collections.ObjectModel;

namespace ArcDock.Data
{
    /// <summary>
    /// 自动填充搜索条目
    /// </summary>
    public class SearchDataItem
    {
        #region 属性和字段

        /// <summary>
        /// 文本和拼音头
        /// </summary>
        private string text, pyHead;

        /// <summary>
        /// 模板填充文本
        /// </summary>
        public string Text { get => text; }

        /// <summary>
        /// 填充文本的拼音头
        /// </summary>
        public string PYHead { get => pyHead; }

        /// <summary>
        /// 多框自动填充条目集合
        /// </summary>
        public ObservableCollection<ExecutionItem> ExecutionItems { get; set; }

        #endregion

        #region 初始化

        /// <summary>
        /// 单框文本补全的条目初始化
        /// </summary>
        /// <param name="text">提示文本</param>
        public SearchDataItem(string text)
        {
            this.text = text;
            pyHead = PinyinHelpers.GetFirstSpell(text);
        }

        /// <summary>
        /// 多框联动补全的条目初始化
        /// </summary>
        /// <param name="text">提示文本</param>
        /// <param name="executionItems">联动项目集合</param>
        public SearchDataItem(string text, ObservableCollection<ExecutionItem> executionItems)
        {
            this.text = text;
            pyHead = PinyinHelpers.GetFirstSpell(text);
            this.ExecutionItems = executionItems;
        }

        #endregion

    }
}
