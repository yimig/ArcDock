﻿using ArcDock.Data.Json;
using CefSharp;
using CefSharp.Wpf;
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Controls;
using System.Windows;
using System.Windows.Data;

namespace ArcDock.Data.UI
{
    internal class FlowTableArea : CustomArea
    {
        private string content;
        private ChromiumWebBrowser browser;
        private Action<string, string, ConfigItem> onContentChanged;

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

        public FlowTableArea(ConfigItem config, ChromiumWebBrowser browser, Action<string, string, ConfigItem> onContentChanged)
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
            SetGap(65);
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
            TextBox textBox = new TextBox();
            textBox.AcceptsReturn = true;
            textBox.TextWrapping = TextWrapping.Wrap;
            textBox.VerticalScrollBarVisibility = ScrollBarVisibility.Auto;
            textBox.Height = 75;
            textBox.SetBinding(TextBox.TextProperty, new Binding("Content") { Source = this });
            textBox.TextChanged += TextBoxOnTextChanged;
            this.InputControl = textBox;
        }

        private void TextBoxOnTextChanged(object sender, TextChangedEventArgs e)
        {
            var textbox = sender as TextBox;
            onContentChanged(this.Id, textbox.Text, this.config);
            if (browser.IsBrowserInitialized && textbox.Text != null)
            {
                browser.Reload();
            }
        }

        public override void SetDefaultValue()
        {
            if (config.Default != String.Empty) this.Content = config.Default;
        }
    }
}