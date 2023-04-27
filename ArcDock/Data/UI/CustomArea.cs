using ArcDock.Data.Json;
using CefSharp.Wpf;
using System;
using System.ComponentModel;
using System.Windows;
using System.Windows.Controls;

namespace ArcDock.Data.UI
{
    /// <summary>
    /// 可填入主页左侧控件流面板的自定义控件抽象类
    /// </summary>
    public abstract class CustomArea : INotifyPropertyChanged
    {
        public ConfigItem config;
        private string content;
        private ChromiumWebBrowser browser;
        private Action<string, string> onContentChanged;
        private Label label;
        private Panel inputControl;

        public event PropertyChangedEventHandler PropertyChanged;

        /// <summary>
        /// 控件所代表配置项的ID
        /// </summary>
        public string Id => config.Id;

        /// <summary>
        /// 标题控件
        /// </summary>
        public Label Label
        {
            get => this.label;
            set => this.label = value;
        }

        /// <summary>
        /// 文本框区域
        /// </summary>
        public Panel InputControl
        {
            get => this.inputControl;
            set => this.inputControl = value;
        }

        /// <summary>
        /// 初始化控件
        /// </summary>
        public abstract void SetChildren();

        /// <summary>
        /// 初始化标题控件
        /// </summary>
        private void SetLabel()
        {
            Label label = new Label();
            label.Content = config.Name + ": ";
            DockPanel.SetDock(label, Dock.Left);
            this.label = label;
        }

        /// <summary>
        /// 设置控件内容间距
        /// </summary>
        /// <param name="height"></param>
        internal void SetGap(double height)
        {
            this.Label.Height = height;
            this.label.HorizontalAlignment = HorizontalAlignment.Right;
            this.label.MinWidth = 40;
            this.label.Margin = new Thickness(0, 0, 0, 5);
            this.InputControl.Height = height;
            this.inputControl.MinWidth = 100;
            this.inputControl.Margin = new Thickness(0, 0, 0, 5);
        }

        /// <summary>
        /// 重置控件内容
        /// </summary>
        public abstract void SetDefaultValue();

        /// <summary>
        /// 根据配置项ID生成一个自定义控件
        /// </summary>
        /// <param name="config">配置项ConfigItem</param>
        /// <param name="browser">CEF实例</param>
        /// <param name="onContentChanged">控件内容改变的触发函数</param>
        /// <returns></returns>
        public static CustomArea GetCustomArea(ConfigItem config, ChromiumWebBrowser browser,
            Action<string, string, ConfigItem> onContentChanged)
        {
            CustomArea customArea = null;
            if (config.Type.Equals("input")) customArea = new InputArea(config, browser, onContentChanged);
            else if (config.Type.Equals("richinput")) customArea = new RichInputArea(config, browser, onContentChanged);
            else if (config.Type.Equals("autoinput")) customArea = new AutoInputArea(config, browser, onContentChanged);
            else if (config.Type.Equals("json")) customArea = new CodeInputArea(config, browser, onContentChanged);
            return customArea;
        }
    }
}
