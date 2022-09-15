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

namespace ArcDock.Data.UI
{
    public class InputArea: DockPanel,INotifyPropertyChanged
    {
        private ConfigItem config;
        private string content;

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

        public InputArea(ConfigItem config)
        {
            this.config = config;
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
            this.Children.Add(textBox);
        }

        private void SetDefaultValue()
        {
            if (config.Default != String.Empty) Content = config.Default;
        }
    }
}
