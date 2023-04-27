using ArcDock.Data;
using ArcDock.Data.Json;
using Microsoft.Scripting.Utils;
using System;
using System.ComponentModel;
using System.Linq;
using System.Windows;

namespace ArcDock
{
    /// <summary>
    /// 代码编辑窗口
    /// </summary>
    public partial class CodeEditerWindow : Window, INotifyPropertyChanged
    {
        #region 字段属性和事件

        public event PropertyChangedEventHandler PropertyChanged;
        private string code;
        private string optionKey;

        /// <summary>
        /// 代码文本
        /// </summary>
        public string Code
        {
            get => code;
            set
            {
                code = value;
                if (PropertyChanged != null)
                {
                    PropertyChanged(this, new PropertyChangedEventArgs("Code"));
                }
            }
        }

        /// <summary>
        /// 目前选中的配置ID名称
        /// </summary>
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

        #endregion

        #region 初始化

        /// <summary>
        /// 初始化代码编辑器窗口
        /// </summary>
        /// <param name="config"></param>
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

        #endregion

        #region 功能解耦

        /// <summary>
        /// 生成注释提示
        /// </summary>
        /// <returns></returns>
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

        #endregion

        #region 事件处理

        /// <summary>
        /// 保存按钮按下的事件处理
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void BtnSave_Click(object sender, RoutedEventArgs e)
        {
            try
            {
                (new PythonEnvironment()).UpdateCode(Code);
            }
            catch (Exception ex)
            {
                MessageBox.Show(ex.Message);
            }

        }

        /// <summary>
        /// 测试运行按钮按下的事件处理
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void BtnTestCode_Click(object sender, RoutedEventArgs e)
        {
            new CodeTestWindow(Code).ShowDialog();
        }

        /// <summary>
        /// 插入代码按钮按下的事件处理
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void BtnAddCode_Click(object sender, RoutedEventArgs e)
        {
            Code += "result['" + OptionKey + "']";
        }

        /// <summary>
        /// 添加提示注释按钮按下的事件处理
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void BtnAddComment_Click(object sender, RoutedEventArgs e)
        {
            Code += GetNavigateTips();
        }

        #endregion
    }
}
