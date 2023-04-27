using ArcDock.Data.Json;
using CefSharp;
using CefSharp.Wpf;
using ICSharpCode.AvalonEdit;
using ICSharpCode.AvalonEdit.Highlighting;
using Microsoft.Xaml.Behaviors;
using Newtonsoft.Json;
using System;
using System.ComponentModel;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Media;

namespace ArcDock.Data.UI
{
    /// <summary>
    /// 代码编辑文本框控件
    /// </summary>
    internal class CodeInputArea : CustomArea, INotifyPropertyChanged
    {
        private ChromiumWebBrowser browser;
        private Action<string, string, ConfigItem> onContentChanged;
        AvalonEditBehaviour behaviour;
        private DockPanel dockPanel;

        public event PropertyChangedEventHandler PropertyChanged;

        /// <summary>
        /// 文本内容
        /// </summary>
        public string Content
        {
            get
            {
                return behaviour.CodeText;
            }
            set
            {
                behaviour.CodeText = value;
                if (PropertyChanged != null) PropertyChanged(this, new PropertyChangedEventArgs("Content"));
            }
        }

        /// <summary>
        /// 新建一个代码编辑文本框
        /// </summary>
        /// <param name="config">配置项ConfigItem</param>
        /// <param name="browser">CEF实例</param>
        /// <param name="onContentChanged">内容改变时触发函数</param>
        public CodeInputArea(ConfigItem config, ChromiumWebBrowser browser, Action<string, string, ConfigItem> onContentChanged)
        {
            behaviour = new AvalonEditBehaviour();
            dockPanel = new DockPanel();
            this.config = config;
            this.browser = browser;
            this.onContentChanged = onContentChanged;
            SetChildren();
            SetDefaultValue();
        }

        /// <summary>
        /// 初始化控件
        /// </summary>
        public override void SetChildren()
        {
            SetLabel();
            SetButton();
            SetTextBox();
            SetGap(65);
        }

        /// <summary>
        /// 初始化标题控件
        /// </summary>
        private void SetLabel()
        {
            Label label = new Label();
            label.Content = config.Name + ": ";
            DockPanel.SetDock(label, Dock.Left);
            this.Label = label;
        }

        /// <summary>
        /// 初始化文本框控件
        /// </summary>
        private void SetTextBox()
        {
            TextEditor textBox = new TextEditor();
            textBox.VerticalScrollBarVisibility = ScrollBarVisibility.Auto;
            textBox.HorizontalScrollBarVisibility = ScrollBarVisibility.Hidden;
            textBox.SyntaxHighlighting = HighlightingManager.Instance.GetDefinition("JavaScript");
            textBox.BorderThickness = new Thickness(1);
            textBox.BorderBrush = new SolidColorBrush(Colors.Blue);
            textBox.ShowLineNumbers = true;
            textBox.LineNumbersForeground = new SolidColorBrush(Colors.Red);
            Interaction.GetBehaviors(textBox).Add(behaviour);
            textBox.Height = 100;
            textBox.TextChanged += TextBoxOnTextChanged;
            dockPanel.Children.Add(textBox);
            this.InputControl = dockPanel;
        }

        /// <summary>
        /// 初始化按钮控件
        /// </summary>
        private void SetButton()
        {
            var btn = new Button();
            btn.Content = "导入文件";
            btn.Click += Btn_Click;
            DockPanel.SetDock(btn, Dock.Right);
            dockPanel.Children.Add(btn);
        }

        /// <summary>
        /// 选择文件按钮按下时的事件处理
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void Btn_Click(object sender, RoutedEventArgs e)
        {
            var wndTable = new TableMatchWindow(this.config);
            wndTable.ShowDialog();
            if (wndTable.IsCheck)
            {
                this.Content = JsonConvert.SerializeObject(wndTable.Result);
            }
        }

        /// <summary>
        /// 文本内容改变时的事件处理
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void TextBoxOnTextChanged(object sender, EventArgs e)
        {
            var textbox = sender as TextEditor;
            onContentChanged(this.Id, textbox.Text, this.config);
            if (browser.IsBrowserInitialized && textbox.Text != null)
            {
                browser.Reload();
            }
        }

        /// <summary>
        /// 重置控件内容
        /// </summary>
        public override void SetDefaultValue()
        {
            if (config.Default != String.Empty) Content = config.Default;
        }
    }
}
