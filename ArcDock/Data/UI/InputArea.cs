﻿using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;
using ArcDock.Data.Json;
using CefSharp;
using CefSharp.DevTools.Security;
using CefSharp.Wpf;

namespace ArcDock.Data.UI
{
    public class InputArea: DockPanel,INotifyPropertyChanged
    {
        private ConfigItem config;
        private string content;
        private ChromiumWebBrowser browser;
        private Action<string,string> onContentChanged;

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

        public InputArea(ConfigItem config, ChromiumWebBrowser browser, Action<string,string> onContentChanged)
        {
            this.config = config;
            this.browser = browser;
            this.onContentChanged = onContentChanged;
            SetChildren();
            SetDefaultValue();
        }

        private void SetChildren()
        {
            SetLabel();
            SetTextBox();
        }

        private void SetLabel()
        {
            Label label = new Label();
            label.Content = config.Name + ": ";
            DockPanel.SetDock(label,Dock.Left);
            this.Children.Add(label);
        }

        private void SetTextBox()
        {
            TextBox textBox = new TextBox();
            textBox.SetBinding(TextBox.TextProperty, new Binding("Content"){Source = this});
            textBox.TextChanged += TextBoxOnTextChanged;
            this.Children.Add(textBox);
        }

        private void TextBoxOnTextChanged(object sender, TextChangedEventArgs e)
        {
            var textbox = sender as TextBox;
            onContentChanged(this.Id,textbox.Text);
            if (browser.IsBrowserInitialized&&textbox.Text != null)
            {
                browser.Reload();
            }
        }

        private void SetDefaultValue()
        {
            if (config.Default != String.Empty) Content = config.Default;
        }
    }
}
