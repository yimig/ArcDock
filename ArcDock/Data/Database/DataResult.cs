using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ArcDock.Data.Database
{
    /// <summary>
    /// 数据库内存储的一条信息
    /// </summary>
    public class DataResult
    {
        /// <summary>
        /// 打印时间，DateTime格式
        /// </summary>
        private DateTime rawDateTime;

        /// <summary>
        /// 打印时间，DateTime格式
        /// </summary>
        public DateTime RawDatetime
        {
            set => rawDateTime = value;
        }

        /// <summary>
        /// 项目ID
        /// </summary>
        public int ItemId { get; set; }

        /// <summary>
        /// 模板文件名
        /// </summary>
        public string TemplateFileName { get; set; }

        /// <summary>
        /// 预留值Id
        /// </summary>
        public string TemplateId { get; set; }

        /// <summary>
        /// 预留值友好名称
        /// </summary>
        public string TemplateName { get; set; }

        /// <summary>
        /// 预留值填充内容
        /// </summary>
        public string TemplateContent { get; set; }

        /// <summary>
        /// 打印类型
        /// Manual=手动单次打印，Batch=批量打印，Html=保存为网页文件，Image=保存为图像文件
        /// </summary>
        public string PrintType { get; set; }

        /// <summary>
        /// 打印日期字符串
        /// </summary>
        public string PrintDate => rawDateTime.ToString();
    }
}
