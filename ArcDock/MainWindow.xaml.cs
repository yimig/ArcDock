using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Drawing;
using System.IO;
using System.Runtime.CompilerServices;
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
using System.Xml;
using ArcDock.Data;
using ArcDock.Data.Json;
using ArcDock.Data.UI;

namespace ArcDock
{
    /// <summary>
    /// MainWindow.xaml 的交互逻辑
    /// </summary>
    public partial class MainWindow : Window
    {
        private Config config;
        private string templateHtml;
        private StructuredText structuredText;

        public MainWindow()
        {
            InitializeComponent();
            LoadConfig();
            SetChildren();
            Browser.Address = Environment.CurrentDirectory + "\\temp.html";
        }

        private void LoadConfig()
        {
            TextReader tReader = new StreamReader(new FileStream(@"template/template.html", FileMode.Open));
            templateHtml = tReader.ReadToEnd();
            tReader.Close();
            XmlDocument xDoc = new XmlDocument();
            xDoc.LoadXml(templateHtml);
            XmlNode configNode = xDoc.SelectSingleNode("//script[@type=\"config/json\"]");
            config = JsonConvert.DeserializeObject<Config>(configNode.InnerText);
            structuredText = new StructuredText(config.ConfigItemList, templateHtml);
        }

        private void SetChildren()
        {
            foreach (var configItem in config.ConfigItemList)
            {
                StMain.Children.Add(new InputArea(configItem,Browser,ChangeHtml));
            }
        }

        private void ChangeHtml(string id, string content)
        {
            structuredText[id] = content;
            TextWriter tw = new StreamWriter(new FileStream(@"temp.html", FileMode.Create));
            tw.Write(structuredText);
            tw.Close();
        }

        private void SaveHtml()
        {
            foreach (var Child in StMain.Children)
            {
                var InputItem = Child as InputArea;
                structuredText[InputItem.Id] = InputItem.Content;
            }

            TextWriter tw = new StreamWriter(new FileStream(@"temp.html", FileMode.Create));
            tw.Write(structuredText);
            tw.Close();
        }

        private void BtnPrint_OnClick(object sender, RoutedEventArgs e)
        {
            SaveHtml();
        }
    }
}
