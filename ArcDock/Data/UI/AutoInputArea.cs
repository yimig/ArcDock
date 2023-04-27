using ArcDock.Data.Json;
using AutoCompleteTextBox.Editors;
using CefSharp;
using CefSharp.Wpf;
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;
using TextChangedEventArgs = AutoCompleteTextBox.Editors.TextChangedEventArgs;

namespace ArcDock.Data.UI
{
    /// <summary>
    /// 智能文本框控件
    /// </summary>
    internal class AutoInputArea : CustomArea, INotifyPropertyChanged
    {
        private string content;
        private ChromiumWebBrowser browser;
        private Action<string, string, ConfigItem> onContentChanged;
        private SearchData searchDataSet;
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
        /// 控件流面板
        /// </summary>
        public ControlDock MainDock { get; set; }

        /// <summary>
        /// 新建一个智能文本框
        /// </summary>
        /// <param name="config">配置项ConfigItem</param>
        /// <param name="browser">CEF实例</param>
        /// <param name="onContentChanged">内容改变时触发函数</param>
        public AutoInputArea(ConfigItem config, ChromiumWebBrowser browser, Action<string, string, ConfigItem> onContentChanged)
        {
            this.config = config;
            this.browser = browser;
            this.onContentChanged = onContentChanged;
            searchDataSet = new SearchData(config);
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
            AutoCompleteTextBox.Editors.AutoCompleteTextBox textBox = new AutoCompleteTextBox.Editors.AutoCompleteTextBox();
            textBox.LoadingContent = new TextBlock() { Margin = new Thickness(5), Text = "请稍后...", Height = 25 };
            textBox.SetBinding(AutoCompleteTextBox.Editors.AutoCompleteTextBox.TextProperty, new Binding("Content") { Source = this });
            textBox.TextChanged += TextBoxOnTextChanged;
            textBox.SelectionChanged += TextBox_SelectionChanged;
            textBox.Delay = 100;
            textBox.Provider = new SuggestionProvider(str =>
            {
                List<Panel> panels = new List<Panel>();
                App.Current.Dispatcher.Invoke((Action)(() =>
                {
                    var result = SearchItem.GetSearchResult(searchDataSet, str);
                    foreach (var item in result)
                    {
                        panels.Add(item);
                    }
                }));
                return panels;
            });
            var grid = new Grid();
            grid.Children.Add(textBox);
            this.InputControl = grid;
        }

        /// <summary>
        /// 自动提示内容选择时的事件处理
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void TextBox_SelectionChanged(object sender, TextChangedEventArgs e)
        {
            if (config.OptionType == 2)
            {
                var executeItems = config.OptionItemList.Single(option => option.Content.Equals(e.Text)).ExecutionItemList;
                foreach (var execItem in executeItems)
                {
                    MainDock.SetChildrenContentValue(execItem.Key, execItem.Content);
                }
            }
            TextBoxOnTextChanged(sender, e);
        }

        /// <summary>
        /// 文本内容改变时的事件处理
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void TextBoxOnTextChanged(object sender, TextChangedEventArgs e)
        {
            var textbox = sender as AutoCompleteTextBox.Editors.AutoCompleteTextBox;
            onContentChanged(this.Id, e.ToString(), this.config);
            if (browser.IsBrowserInitialized && e.ToString() != null)
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
