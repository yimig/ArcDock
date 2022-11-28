using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;
using System.Windows.Documents;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using System.Windows.Shapes;

namespace ArcDock
{
    /// <summary>
    /// 分析患者信息窗口
    /// </summary>
    public partial class AnalyseStringWindow : Window
    {
        #region 字段和属性

        /// <summary>
        /// 是否输入内容
        /// </summary>
        public bool IsHasContent { get; set; }

        /// <summary>
        /// 药品名
        /// </summary>
        public string MedicamentName { get; set; }

        /// <summary>
        /// 药品数量
        /// </summary>
        public string MedicamentNum { get; set; }

        /// <summary>
        /// 患者姓名
        /// </summary>
        public string PatientName { get; set; }

        /// <summary>
        /// 住院号
        /// </summary>
        public string InPatientNo { get; set; }

        /// <summary>
        /// 床号
        /// </summary>
        public string BedNo { get; set; }

        /// <summary>
        /// 患者科室
        /// </summary>
        public string PatientDept { get; set; }

        #endregion

        #region 初始化
        public AnalyseStringWindow()
        {
            IsHasContent = false;
            InitProp();
            InitializeComponent();
        }

        /// <summary>
        /// 初始化属性值
        /// </summary>
        private void InitProp()
        {
            PatientName = String.Empty;
            InPatientNo = String.Empty;
            BedNo = String.Empty;
            PatientDept = String.Empty;
            MedicamentName = String.Empty;
            MedicamentNum = String.Empty;
        }

        #endregion

        #region 功能解耦

        /// <summary>
        /// 分析字符串
        /// </summary>
        private void StartAnalyse()
        {
            IsHasContent = true;
            if (AnalyseString(TbAnalyse.Text)) this.Close();
        }

        /// <summary>
        /// 分析执行医嘱界面复制的字符串内容
        /// </summary>
        /// <param name="text">患者信息</param>
        /// <exception cref="Exception"></exception>
        private void AnalyseExecuteInfo(string text)
        {
            try
            {
                var strArray = text.Split(new char[] { '\n' });
                PatientName = strArray[0].Trim();
                InPatientNo = strArray.Length >= 2 ? strArray[2].Trim() : String.Empty;
                BedNo = strArray.Length >= 5 ? strArray[5].Trim() : String.Empty;
                MedicamentName = strArray.Length >= 8 ? GetMedicamentName(strArray[8].Trim()) : String.Empty;
                PatientDept = strArray.Length >= 9 ? strArray[9].Trim() : String.Empty;
                MedicamentNum = strArray.Length >= 10 ? strArray[10].Trim() : String.Empty;
            }
            catch (Exception e)
            {
                throw new Exception("解析执行医嘱信息失败，字符串格式错误【" + e.Message + "】");
            }

        }

        /// <summary>
        /// 统一化药品名称（括号统一为英文括号，去除末尾规格）
        /// </summary>
        /// <param name="text">原始药品名称</param>
        /// <returns>统一化处理后的药品名称</returns>
        private string GetMedicamentName(string text)
        {
            var engBracketStr = text.Replace('（', '(').Replace('）', ')');
            if (engBracketStr.EndsWith(")"))
            {
                var removeStartPos = engBracketStr.LastIndexOf('(');
                engBracketStr = engBracketStr.Substring(0,removeStartPos);
            }

            return engBracketStr;
        }

        /// <summary>
        /// 分析诊疗界面复制的字符串内容
        /// </summary>
        /// <param name="text">患者信息</param>
        /// <exception cref="Exception"></exception>
        private void AnalyseTreatmentInfo(string text)
        {
            try
            {
                var strArray = text.Split(new char[] { ' ' });
                PatientName = strArray[0].Replace('/', '\t').Trim();
                BedNo = strArray.Length >= 3 ? strArray[3].Remove(0, 3).Trim() : String.Empty;
                InPatientNo = strArray.Length >= 4 ? strArray[4].Remove(0, 4).Trim() : String.Empty;
                PatientDept = strArray.Length >= 10 ? strArray[10].Remove(0, 5).Trim() : String.Empty;
            }
            catch (Exception e)
            {
                throw new Exception("解析诊疗信息失败，字符串格式错误【" + e.Message + "】");
            }

        }

        /// <summary>
        /// 根据字符串特征分析患者信息
        /// </summary>
        /// <param name="rawText">患者信息</param>
        /// <returns>分析是否成功</returns>
        private bool AnalyseString(string rawText)
        {
            var text = rawText.Trim();
            var res = true;
            try
            {
                AnalyseExecuteInfo(text);
                // if (text.Any(c => c.Equals('/'))) AnalyseTreatmentInfo(text);
                // else if (text.Any(c => c.Equals('\n'))) AnalyseExecuteInfo(text);
                // else
                // {
                //     throw new Exception("仅可从执行医嘱列表或诊疗界面标题复制文本信息");
                // }
            }
            catch (Exception e)
            {
                MessageBox.Show("解析失败：" + e.Message);
                res = false;
                TbAnalyse.Text = "";
            }

            return res;
        }

        #endregion

        #region 事件处理

        /// <summary>
        /// 分析按钮事件处理
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void BtnAnalyse_OnClick(object sender, RoutedEventArgs e)
        {
            StartAnalyse();
        }

        /// <summary>
        /// 取消按钮事件处理
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void BtnCancel_OnClick(object sender, RoutedEventArgs e)
        {
            IsHasContent = false;
            this.Close();
        }

        #endregion
    }
}
