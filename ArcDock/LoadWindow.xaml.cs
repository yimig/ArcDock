using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Reflection;
using System.Windows.Controls;
using System.Windows.Data;
using System.Windows.Documents;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using System.Windows.Shapes;

namespace ArcDock
{
    /// <summary>
    /// Interaction logic for LoadWindow.xaml
    /// </summary>
    public partial class LoadWindow : Window, INotifyPropertyChanged
    {
        public event PropertyChangedEventHandler PropertyChanged;

        private string info;
        public string Info
        {
            get => info;
            set
            {
                info = value;
                if (PropertyChanged != null) PropertyChanged(this, new PropertyChangedEventArgs("Info"));
            }
        }

        public string Version
        {
            get=> Assembly.GetExecutingAssembly().GetName().Version.ToString();
        }

        public LoadWindow()
        {
            InitializeComponent();
            this.DataContext = this;
        }

        public void ChangedInfo(string info)
        {
            this.Info = info;
        }
    }
}
