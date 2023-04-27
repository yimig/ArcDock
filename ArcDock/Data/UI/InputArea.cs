using ArcDock.Data.Json;
using CefSharp;
using CefSharp.Wpf;
using System;
using System.ComponentModel;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;

namespace ArcDock.Data.UI
{
    /// <summary>
    /// 普通文本框控件
    /// </summary>
    public class InputArea : CustomArea, INotifyPropertyChanged
    {
        private string content;
        private ChromiumWebBrowser browser;
        private Action<string, string, ConfigItem> onContentChanged;

        public event PropertyChangedEventHandler PropertyChanged;

        /// <summary>
        /// 文本内容
        /// </summary>
        public string Content
        {
            get
            {
                return content;
            }
            set
            {
                content = value;
                if (PropertyChanged != null) PropertyChanged(this, new PropertyChangedEventArgs("Content"));
            }
        }

        /// <summary>
        /// 新建一个普通文本框
        /// </summary>
        /// <param name="config">配置项ConfigItem</param>
        /// <param name="browser">CEF实例</param>
        /// <param name="onContentChanged">内容改变时触发函数</param>
        public InputArea(ConfigItem config, ChromiumWebBrowser browser, Action<string, string, ConfigItem> onContentChanged)
        {
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
            SetTextBox();
            SetGap(25);
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
            TextBox textBox = new TextBox();
            textBox.VerticalContentAlignment = VerticalAlignment.Center;
            textBox.SetBinding(TextBox.TextProperty, new Binding("Content") { Source = this });
            textBox.TextChanged += TextBoxOnTextChanged;
            var grid = new Grid();
            grid.Children.Add(textBox);
            this.InputControl = grid;
        }

        /// <summary>
        /// 文本内容改变时的事件处理
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void TextBoxOnTextChanged(object sender, TextChangedEventArgs e)
        {
            var textbox = sender as TextBox;
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
