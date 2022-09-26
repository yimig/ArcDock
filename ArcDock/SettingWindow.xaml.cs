using System;
using System.Collections.Generic;
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
    /// SettingWindow.xaml 的交互逻辑
    /// </summary>
    public partial class SettingWindow : Window
    {
        public int PrintApi { get; set; }
        public SettingWindow(int printApi)
        {
            PrintApi = printApi;
            InitializeComponent();
            InitPrintApi();
        }

        private void CheckPrintApi()
        {
            if ((bool)RbPrintDocument.IsChecked) PrintApi = 0;
            else if ((bool)RbCefPrint.IsChecked) PrintApi = 1;
            else if ((bool)RbClodop.IsChecked) PrintApi = 2;
            else if ((bool)RbPdf.IsChecked) PrintApi = 3;
            else if ((bool)RbSpire.IsChecked) PrintApi = 4;
        }

        private void InitPrintApi()
        {
            if (PrintApi == 0) RbPrintDocument.IsChecked = true;
            else if (PrintApi == 1) RbCefPrint.IsChecked = true;
            else if (PrintApi == 2) RbClodop.IsChecked = true;
            else if (PrintApi == 3) RbPdf.IsChecked = true;
            else if (PrintApi == 4) RbSpire.IsChecked = true;
        }

        private void RbPrint_OnChecked(object sender, RoutedEventArgs e)
        {
            CheckPrintApi();
        }
    }
}
