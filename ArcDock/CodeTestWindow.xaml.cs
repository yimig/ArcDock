using ArcDock.Data;
using Microsoft.Scripting.Interpreter;
using System;
using System.Windows;

namespace ArcDock
{
    /// <summary>
    /// 代码测试运行窗口
    /// </summary>
    public partial class CodeTestWindow : Window
    {
        #region 字段属性和事件

        private string code;

        #endregion

        #region 初始化

        /// <summary>
        /// 初始化代码测试运行窗口
        /// </summary>
        /// <param name="code">要运行的代码</param>
        public CodeTestWindow(string code)
        {
            this.code = code;
            InitializeComponent();
        }

        #endregion

        #region 事件处理

        /// <summary>
        /// 按下运行按钮的事件处理
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void BtnRunCode_Click(object sender, RoutedEventArgs e)
        {
            var res = "";
            try
            {
                var dict = PythonEnvironment.RunPython(code, TbSource.Text);
                foreach (var item in dict)
                {
                    res += "{" + item.Key + ":\"" + item.Value + "\"}\r\n";
                }
            }
            catch (Exception ex)
            {
                var debugInfo = ((InterpretedFrameInfo[])ex.Data[typeof(InterpretedFrameInfo)])[0].DebugInfo;
                MessageBox.Show("[Line={" + debugInfo.StartLine + ":" + debugInfo.EndLine + "},Index=" + debugInfo.Index + "]:" + ex.Message, ex.Message);
            }

            TbResult.Text = res;
        }

        #endregion
    }
}
