using ArcDock.Data;
using Microsoft.Scripting.Interpreter;
using System;
using System.Collections.Generic;
using System.Windows;

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

        ///// <summary>
        ///// 药品名
        ///// </summary>
        //public string MedicamentName { get; set; }

        ///// <summary>
        ///// 药品数量
        ///// </summary>
        //public string MedicamentNum { get; set; }

        ///// <summary>
        ///// 患者姓名
        ///// </summary>
        //public string PatientName { get; set; }

        ///// <summary>
        ///// 住院号
        ///// </summary>
        //public string InPatientNo { get; set; }

        ///// <summary>
        ///// 床号
        ///// </summary>
        //public string BedNo { get; set; }

        ///// <summary>
        ///// 患者科室
        ///// </summary>
        //public string PatientDept { get; set; }

        public Dictionary<string, string> AnalyzeDict { get; set; }

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
            AnalyzeDict = new Dictionary<string, string>();
        }

        #endregion

        #region 功能解耦

        /// <summary>
        /// 分析字符串
        /// </summary>
        private void StartAnalyse()
        {
            IsHasContent = true;
            try
            {
                var code = PythonEnvironment.GetPythonCode();
                AnalyzeDict = PythonEnvironment.RunPython(code, TbAnalyse.Text);
                this.Close();
            }
            catch (Exception ex)
            {
                var debugInfo = ((InterpretedFrameInfo[])ex.Data[typeof(InterpretedFrameInfo)])[0].DebugInfo;
                MessageBox.Show("[Line={" + debugInfo.StartLine + ":" + debugInfo.EndLine + "},Index=" + debugInfo.Index + "]:" + ex.Message, ex.Message);
            }
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
