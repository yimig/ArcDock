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

namespace ArcDock
{
    /// <summary>
    /// SelectPrintNumWindow.xaml 的交互逻辑
    /// </summary>
    public partial class SelectPrintNumWindow : Window, INotifyPropertyChanged
    {
        private int pageNumber;
        public event PropertyChangedEventHandler PropertyChanged;
        public int PageNumber
        {
            get => pageNumber;
            set
            {
                pageNumber = value;
                if (PropertyChanged != null) PropertyChanged(this, new PropertyChangedEventArgs("PageNumber"));
            }
        }

        public bool IsPrint { get; set; }

        public SelectPrintNumWindow()
        {
            this.DataContext = this;
            InitializeComponent();
            this.PageNumber = 1;
            this.IsPrint = false;
        }

        private void BtnAdd_OnClick(object sender, RoutedEventArgs e)
        {
            this.PageNumber++;
        }

        private void BtnSub_OnClick(object sender, RoutedEventArgs e)
        {
            PageNumber = PageNumber == 1 ? 1 : PageNumber - 1;
        }

        private void BtnCancel_OnClick(object sender, RoutedEventArgs e)
        {
            this.Close();
        }

        private void BtnOk_OnClick(object sender, RoutedEventArgs e)
        {
            this.IsPrint = true;
            this.Close();
        }
    }
}
