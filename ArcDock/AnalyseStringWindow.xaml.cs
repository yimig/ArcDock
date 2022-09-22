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
    /// AnalyseStringWindow.xaml 的交互逻辑
    /// </summary>
    public partial class AnalyseStringWindow : Window
    {
        public bool IsHasContent { get; set; }

        public string PatientName { get; set; }

        public string InPatientNo { get; set; }

        public string BedNo { get; set; }

        public string PatientDept { get; set; }

        public AnalyseStringWindow()
        {
            IsHasContent = false;
            InitProp();
            InitializeComponent();
        }

        private void InitProp()
        {
            PatientName = String.Empty;
            InPatientNo = String.Empty;
            BedNo = String.Empty;
            PatientDept = String.Empty;
        }

        private void BtnAnalyse_OnClick(object sender, RoutedEventArgs e)
        {
            IsHasContent = true;
            if(AnalyseString(TbAnalyse.Text))this.Close();
        }

        private void AnalyseExecuteInfo(string text)
        {
            try
            {
                var strArray = text.Split(new char[] { '\n' });
                PatientName = strArray[0].Trim();
                InPatientNo = strArray.Length >= 2 ? strArray[2].Trim() : String.Empty;
                BedNo = strArray.Length >= 5 ? strArray[5].Trim() : String.Empty;
            }
            catch (Exception e)
            {
                throw new Exception("解析执行医嘱信息失败，字符串格式错误【"+e.Message+"】");
            }

        }
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
                throw new Exception("解析诊疗信息失败，字符串格式错误【"+e.Message+"】");
            }

        }

        private bool AnalyseString(string text)
        {
            var res = true;
            try
            {
                if (text.Any(c => c.Equals('\n'))) AnalyseExecuteInfo(text);
                else if (text.Any(c => c.Equals('/'))) AnalyseTreatmentInfo(text);
                else
                {
                    throw new Exception("仅可从执行医嘱列表或诊疗界面标题复制文本信息");
                }
            }
            catch (Exception e)
            {
                MessageBox.Show("解析失败：" + e.Message);
                res = false;
            }

            return res;
        }

        private void BtnCancel_OnClick(object sender, RoutedEventArgs e)
        {
            IsHasContent = false;
            this.Close();
        }
    }
}
