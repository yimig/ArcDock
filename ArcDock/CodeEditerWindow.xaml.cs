using ArcDock.Data;
using ArcDock.Data.Json;
using Microsoft.Scripting.Utils;
using System;
using System.Collections.Generic;
using System.ComponentModel;
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
    /// Interaction logic for CodeEditerWindow.xaml
    /// </summary>
    public partial class CodeEditerWindow : Window, INotifyPropertyChanged
    {
        public event PropertyChangedEventHandler PropertyChanged;
        private string code;
        private string optionKey;

        public string Code
        {
            get => code;
            set
            {
                code = value;
                if(PropertyChanged!= null)
                {
                    PropertyChanged(this, new PropertyChangedEventArgs("Code"));
                }
            }
        }
        public string OptionKey
        {
            get => optionKey;
            set
            {
                optionKey = value;
                if (PropertyChanged != null)
                {
                    PropertyChanged(this, new PropertyChangedEventArgs("OptionKey"));
                }
            }
        }
        private Config config;
        public CodeEditerWindow(Config config)
        {
            this.Resources.Add("Config", config);
            InitializeComponent();
            this.config = config;
            var pyCode = PythonEnvironment.GetPythonCode();
            Code = pyCode == "" ? GetNavigateTips() : pyCode;
            this.DataContext = this;
            CbIndex.ItemsSource = config.ConfigItemList.Select(i => i.Name).ToList();
            ICSharpCode.AvalonEdit.Search.SearchPanel.Install(TextEditor);
        }

        private string GetNavigateTips()
        {
            var res = "# 输入值：字符串类型 source\t\n" +
                      "# 输出值：字符串字典 result\t\n" +
                      "# 示例：\t\n" +
                      "# result['val1'] = source.split(',')[0] #将输入值按照,进行拆分，将拆分后第一个值赋给val1 \t\n" +
                      "# result['val2'] = source.split(',')[1] #将输入值按照,进行拆分，将拆分后第二个值赋给val2 \t\n" +
                      "# 当输入值为“abc,1234”时，将得到字典{'val1':'abc','val2':'1234'}\t\n" +
                      "# ==========以下是当前配置的输出值参考==========\t\n";
            foreach (var id in config.ConfigItemList.Select(i => i.Id))
            {
                res += "# result['" + id + "']\n";
            }
            res += "# 您可以根据实际情况选择其中的项目进行填充。\t\n";
            return res;
        }

        private void BtnSave_Click(object sender, RoutedEventArgs e)
        {
            try
            {
                (new PythonEnvironment()).UpdateCode(Code);
            } catch (Exception ex)
            {
                MessageBox.Show(ex.Message);
            }

        }

        private void BtnTestCode_Click(object sender, RoutedEventArgs e)
        {
            new CodeTestWindow(Code).ShowDialog();
        }

        private void BtnAddCode_Click(object sender, RoutedEventArgs e)
        {
            Code += "result['" + OptionKey + "']";
        }
    }
}
