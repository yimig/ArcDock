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
using CefSharp;
using CefSharp.Wpf;
using WpfControls;

namespace ArcDock.Data.UI
{
    internal class AutoInputArea :CustomArea, INotifyPropertyChanged
    {
        private ConfigItem config;
        private string content;
        private ChromiumWebBrowser browser;
        private Action<string, string> onContentChanged;

        public event PropertyChangedEventHandler PropertyChanged;

        public string Id => config.Id;

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

        public AutoInputArea(ConfigItem config, ChromiumWebBrowser browser, Action<string, string> onContentChanged)
        {
            this.config = config;
            this.browser = browser;
            this.onContentChanged = onContentChanged;
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
            AutoCompleteTextBox textBox = new AutoCompleteTextBox();
            textBox.LoadingContent = new TextBlock() { Margin = new Thickness(5), Text = "请稍后...", Height = 25 };
            textBox.Delay = 100;
            if (config.OptionType == 1)
                textBox.Provider = new SuggestionProvider(str =>
                {
                    List<Panel> panels = new List<Panel>();
                    foreach (var optStr in config.Option)
                    {
                        App.Current.Dispatcher.Invoke((Action)(() =>
                        {
                            DockPanel dp = new DockPanel();
                            TextBox tb = new TextBox() { Text = optStr };
                            DockPanel.SetDock(tb, Dock.Left);
                            dp.Children.Add(tb);
                            panels.Add(dp);
                        }));
                    }

                    return panels;
                });
            this.InputControl = textBox;
            textBox.SetBinding(AutoCompleteTextBox.TextProperty, new Binding("Content") { Source = this });
            //textBox.Editor.TextChanged += TextBoxOnTextChanged;
            textBox.KeyDown += TextBoxOnKeyDown;
        }

        private void TextBoxOnKeyDown(object sender, KeyEventArgs e)
        {
            var textbox = sender as TextBox;
            onContentChanged(this.Id, textbox.Text);
            if (browser.IsBrowserInitialized && textbox.Text != null)
            {
                browser.Reload();
            }
        }

        private void TextBoxOnTextChanged(object sender, TextChangedEventArgs e)
        {
            var textbox = sender as TextBox;
            onContentChanged(this.Id, textbox.Text);
            if (browser.IsBrowserInitialized && textbox.Text != null)
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
