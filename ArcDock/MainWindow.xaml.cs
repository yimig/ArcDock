using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Drawing;
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
using System.Xml;
using ArcDock.Data;
using ArcDock.Data.Json;
using ArcDock.Data.UI;
using System.Windows.Forms;
using WebBrowser = System.Windows.Forms.WebBrowser;
using Rectangle = System.Drawing.Rectangle;

namespace ArcDock
{
    /// <summary>
    /// MainWindow.xaml 的交互逻辑
    /// </summary>
    public partial class MainWindow : Window
    {
        private Config config;
        private string htmlText;

        public MainWindow()
        {
            InitializeComponent();
            LoadConfig();
            SetChildren();
        }

        private void LoadConfig()
        {
            TextReader tReader = new StreamReader(new FileStream(@"template/template.html", FileMode.Open));
            htmlText = tReader.ReadToEnd();
            tReader.Close();
            XmlDocument xDoc = new XmlDocument();
            xDoc.LoadXml(htmlText);
            XmlNode configNode = xDoc.SelectSingleNode("//script[@type=\"config/json\"]");
            config = JsonConvert.DeserializeObject<Config>(configNode.InnerText);
        }

        private void SetChildren()
        {
            foreach (var configItem in config.ConfigItemList)
            {
                StMain.Children.Add(new InputArea(configItem));
            }
        }

        private void BtnPrint_OnClick(object sender, RoutedEventArgs e)
        {
            foreach (var Child in StMain.Children)
            {
                var InputItem = Child as InputArea;
                htmlText = htmlText.Replace("{{" + InputItem.Id + "}}", InputItem.Content);
            }

            TextWriter tw = new StreamWriter(new FileStream(@"temp.html", FileMode.Create));
            tw.Write(htmlText);
            tw.Close();
            var browser = new WebBrowser();
            browser.Navigate(System.Environment.CurrentDirectory+"\\temp.html");
            browser.DocumentCompleted += BrowserOnDocumentCompleted;
        }

        private void BrowserOnDocumentCompleted(object sender, WebBrowserDocumentCompletedEventArgs e)
        {
            var browser = (WebBrowser)sender;
            if (browser.ReadyState == WebBrowserReadyState.Complete)
            {
                if (browser.Document != null)
                {
                    if (browser.Document.Body != null)
                    {
                        var height = config.Settings.Height;
                        var width = config.Settings.Width;
                        browser.Height = height;
                        browser.Width = width;
                        using (var bitmap = new Bitmap(width, height))
                        {
                            var rectangle = new Rectangle(0, 0, width, height);
                            browser.DrawToBitmap(bitmap, rectangle);
                            var dialog = new SaveFileDialog();
                            dialog.Filter = " JPEG (*.jpg)|*.jpg|PNG (*.png)|*.png ";
                            dialog.ShowDialog();
                            bitmap.Save(dialog.FileName);
                        }
                    }
                }
            }
        }
    }
}
