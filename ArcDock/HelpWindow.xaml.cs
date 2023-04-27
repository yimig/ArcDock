using System;
using System.Windows;

namespace ArcDock
{
    /// <summary>
    /// Interaction logic for HelpWindow.xaml
    /// </summary>
    public partial class HelpWindow : Window
    {
        public HelpWindow()
        {
            InitializeComponent();
            Browser.Address = Environment.CurrentDirectory + @".\help\index.html";
        }
    }
}
