using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ArcDock.Data.Database
{
    /// <summary>
    /// 经过重排整理的数据库结果
    /// </summary>
    public class SortResult
    {
        private DateTime rawPrintDate;

        /// <summary>
        /// 项目ID
        /// </summary>
        public int ItemId { get; set; }

        /// <summary>
        /// 患者姓名
        /// </summary>
        public string PatientName { get; set; }

        /// <summary>
        /// 住院号
        /// </summary>
        public string PatientNo { get; set; }

        /// <summary>
        /// 患者所在科室
        /// </summary>
        public string PatientDept { get; set; }

        /// <summary>
        /// 床号
        /// </summary>
        public string PatientBed { get; set; }

        /// <summary>
        /// 药品名
        /// </summary>
        public string MedicamentName { get; set; }

        /// <summary>
        /// 药品数量
        /// </summary>
        public string MedicamentNum { get; set; }

        /// <summary>
        /// 打印时间，DateTime格式
        /// </summary>
        public DateTime RawPrintDate
        {
            set => rawPrintDate = value;
        }

        /// <summary>
        /// 打印时间字符串
        /// </summary>
        public string PrintDate
        {
            get => rawPrintDate.ToString();
        }
    }
}
