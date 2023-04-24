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
using ArcDock.Data.Json;
using CefSharp;
using ICSharpCode.AvalonEdit;
using ICSharpCode.AvalonEdit.Highlighting;
using Microsoft.Xaml.Behaviors;
using System.Media;
using System.Windows.Media;

namespace ArcDock.Data.UI
{
    internal class CodeInputArea : CustomArea, INotifyPropertyChanged
    {
        private ChromiumWebBrowser browser;
        private Action<string, string, ConfigItem> onContentChanged;
        AvalonEditBehaviour behaviour;
        private DockPanel dockPanel;

        public event PropertyChangedEventHandler PropertyChanged;

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

        public override void SetChildren()
        {
            SetLabel();
            SetButton();
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


        private void SetButton()
        {
            var btn = new Button();
            btn.Content = "导入文件";
            btn.Click += Btn_Click;
            DockPanel.SetDock(btn, Dock.Right);
            dockPanel.Children.Add(btn);
        }

        private void Btn_Click(object sender, RoutedEventArgs e)
        {
            var wndTable = new TableMatchWindow(this.config);
            wndTable.ShowDialog();
            if(wndTable.IsCheck)
            {
                this.Content = wndTable.ResultJson;
            }
        }

        private void TextBoxOnTextChanged(object sender, EventArgs e)
        {
            var textbox = sender as TextEditor;
            onContentChanged(this.Id, textbox.Text, this.config);
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
