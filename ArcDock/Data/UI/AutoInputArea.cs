using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;
using System.Windows.Input;
using ArcDock.Data.Json;
using AutoCompleteTextBox.Editors;
using CefSharp;
using CefSharp.Wpf;
using TextChangedEventArgs = AutoCompleteTextBox.Editors.TextChangedEventArgs;

namespace ArcDock.Data.UI
{
    internal class AutoInputArea :CustomArea, INotifyPropertyChanged
    {
        private string content;
        private ChromiumWebBrowser browser;
        private Action<string, string> onContentChanged;
        private SearchDataSet searchDataSet;
        public event PropertyChangedEventHandler PropertyChanged;

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
        public ControlDock MainDock { get; set; }

        public AutoInputArea(ConfigItem config, ChromiumWebBrowser browser, Action<string, string> onContentChanged)
        {
            this.config = config;
            this.browser = browser;
            this.onContentChanged = onContentChanged;
            searchDataSet = new SearchDataSet(config);
            SetChildren();
            SetDefaultValue();
        }

        public override void SetChildren()
        {
            SetLabel();
            SetTextBox();
            SetGap(25);
        }

        private void SetLabel()
        {
            Label label = new Label();
            label.Content = config.Name + ": ";
            DockPanel.SetDock(label, Dock.Left);
            this.Label = label;
        }

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
            this.InputControl = textBox;
        }

        private void TextBox_SelectionChanged(object sender, TextChangedEventArgs e)
        {
            if (config.OptionType == 2)
            {
                var executeItems = config.OptionItemList.Single(option=>option.Content.Equals(e.Text)).ExecutionItemList;
                foreach (var execItem in executeItems)
                {
                    var customArea = MainDock.CustomAreas.Single(area => area.Id.Equals(execItem.Key));
                    if (customArea.config.Type.Equals("input")) (customArea as InputArea).Content = execItem.Content;
                    else if (customArea.config.Type.Equals("richinput")) (customArea as RichInputArea).Content = execItem.Content;
                    else if (customArea.config.Type.Equals("autoinput")) (customArea as AutoInputArea).Content = execItem.Content;
                }
            }
            TextBoxOnTextChanged(sender,e);
        }

        private void TextBoxOnTextChanged(object sender, TextChangedEventArgs e)
        {
            var textbox = sender as AutoCompleteTextBox.Editors.AutoCompleteTextBox;
            onContentChanged(this.Id, e.ToString());
            if (browser.IsBrowserInitialized && e.ToString() != null)
            {
                browser.Reload();
            }
        }

        public override void SetDefaultValue()
        {
            if (config.Default != String.Empty) Content = config.Default;
        }
    }
}
