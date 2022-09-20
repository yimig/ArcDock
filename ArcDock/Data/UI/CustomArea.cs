﻿using ArcDock.Data.Json;
using CefSharp;
using CefSharp.Wpf;
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;

namespace ArcDock.Data.UI
{
    public abstract class CustomArea : INotifyPropertyChanged
    {
        private ConfigItem config;
        private string content;
        private ChromiumWebBrowser browser;
        private Action<string, string> onContentChanged;
        private Label label;
        private Control inputControl;

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

        public Label Label
        {
            get => this.label;
            set => this.label = value;
        }

        public Control InputControl
        {
            get => this.inputControl;
            set => this.inputControl = value;
        }

        public abstract void SetChildren();

        private void SetLabel()
        {
            Label label = new Label();
            label.Content = config.Name + ": ";
            DockPanel.SetDock(label, Dock.Left);
            this.label = label;
        }

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

        public abstract void SetDefaultValue();


        public static CustomArea GetCustomArea(ConfigItem config, ChromiumWebBrowser browser,
            Action<string, string> onContentChanged)
        {
            CustomArea customArea = null;
            if (config.Type.Equals("input")) customArea = new InputArea(config, browser, onContentChanged);
            else if (config.Type.Equals("richinput")) customArea = new RichInputArea(config, browser, onContentChanged);
            else if (config.Type.Equals("autoinput")) customArea = new AutoInputArea(config, browser, onContentChanged);
            return customArea;
        }
    }
}
