using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.IO;
using System.Windows;
using Newtonsoft.Json;
using System.Windows.Controls;
using System.Windows.Data;
using System.Windows.Documents;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using System.Windows.Navigation;
using System.Windows.Shapes;
using ArcDock.Data;

namespace ArcDock
{
    /// <summary>
    /// MainWindow.xaml 的交互逻辑
    /// </summary>
    public partial class MainWindow : Window
    {
        public MainWindow()
        {
            InitializeComponent();
            LoadConfig();

        }

        public void LoadConfig()
        {
            TextReader tReader = new StreamReader(new FileStream(@"template/temp.json", FileMode.Open));
            var text = tReader.ReadToEnd();
            tReader.Close();
            var config = JsonConvert.DeserializeObject<Config>(text);
            Console.ReadLine();
        }
    }
}
