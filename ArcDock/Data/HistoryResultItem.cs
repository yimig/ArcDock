using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ArcDock.Data
{
    /// <summary>
    /// 历史记录条目
    /// </summary>
    public class HistoryResultItem
    {
        /// <summary>
        /// 预留值ID
        /// </summary>
        public string Id { get; set; }

        /// <summary>
        /// 预留值友好名称
        /// </summary>
        public string Name { get; set; }

        /// <summary>
        /// 预留值内容
        /// </summary>
        public string Content { get; set; }
    }
}
