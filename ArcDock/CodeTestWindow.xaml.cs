using ArcDock.Data;
using Microsoft.Scripting.Interpreter;
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
    /// Interaction logic for CodeTestWindow.xaml
    /// </summary>
    public partial class CodeTestWindow : Window
    {
        private string code;
        public CodeTestWindow(string code)
        {
            this.code = code;
            InitializeComponent();
        }

        private void BtnRunCode_Click(object sender, RoutedEventArgs e)
        {
            var res = "";
            try
            {
                var dict = PythonEnvironment.RunPython(code, TbSource.Text);
                foreach (var item in dict)
                {
                    res += "{"+item.Key + ":\"" + item.Value + "\"}\r\n";
                }
            } catch(Exception ex) {
                var debugInfo = ((InterpretedFrameInfo[])ex.Data[typeof(InterpretedFrameInfo)])[0].DebugInfo;
                MessageBox.Show("[Line={" + debugInfo.StartLine + ":" + debugInfo.EndLine + "},Index=" + debugInfo.Index + "]:"+ex.Message,ex.Message);
            }

            TbResult.Text = res;
        }
    }
}
