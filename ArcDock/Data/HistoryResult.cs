using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ArcDock.Data
{
    /// <summary>
    /// 历史记录数据
    /// </summary>
    public class HistoryResult
    {
        #region 字段和属性

        /// <summary>
        /// 模板文件名称
        /// </summary>
        public string TemplateFileName { get; set; }

        /// <summary>
        /// 打印类型
        /// Manual=手动单次打印，Batch=批量打印，Html=保存为网页文件，Image=保存为图像文件
        /// </summary>
        public string PrintType { get; set; }

        /// <summary>
        /// 历史记录条目集合
        /// </summary>
        public List<HistoryResultItem> ResultItems { get; set; }

        #endregion

        #region 初始化

        public HistoryResult()
        {
            ResultItems = new List<HistoryResultItem>();
        }

        /// <summary>
        /// 初始化历史记录数据
        /// </summary>
        /// <param name="fileName">模板文件名</param>
        /// <param name="printType">打印类型</param>
        public HistoryResult(string fileName, string printType)
        {
            ResultItems = new List<HistoryResultItem>();
            this.TemplateFileName = fileName;
            this.PrintType = printType;
        }

        #endregion

    }
}
