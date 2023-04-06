using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;
using System.Windows.Documents;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using System.Windows.Shapes;

namespace ArcDock.Data.UI.SubWindow
{
    /// <summary>
    /// Interaction logic for InputWindow.xaml
    /// </summary>
    public partial class InputWindow : Window, INotifyPropertyChanged
    {
        public event PropertyChangedEventHandler PropertyChanged;
        private string data;
        public string Data
        {
            get => data;
            set
            {
                data = value;
                if (PropertyChanged != null)
                {
                    PropertyChanged(this, new PropertyChangedEventArgs("Data"));
                }
            }
        }

        public bool IsCheck{ get;set; }

        public InputWindow(string data)
        {
            InitializeComponent();
            IsCheck = false;
            Data = data;
            this.DataContext = this;
        }

        private void BtnCheck_Click(object sender, RoutedEventArgs e)
        {
            IsCheck = true;
            this.Close();
        }
    }
}
