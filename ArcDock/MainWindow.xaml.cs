using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Drawing;
using System.Drawing.Imaging;
using System.Drawing.Printing;
using System.IO;
using System.Runtime.CompilerServices;
using System.Windows;
using Newtonsoft.Json;
using System.Windows.Controls;
using System.Windows.Data;
using System.Windows.Documents;
using System.Windows.Forms;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using System.Windows.Navigation;
using System.Windows.Shapes;
using System.Xml;
using ArcDock.Data;
using ArcDock.Data.Json;
using ArcDock.Data.UI;
using CefSharp;
using CefSharp.DevTools.Page;
using MessageBox = System.Windows.MessageBox;
using Binding = System.Windows.Data.Binding;
using Color = System.Drawing.Color;
using Image = System.Drawing.Image;
using PixelFormat = System.Drawing.Imaging.PixelFormat;

namespace ArcDock
{
    /// <summary>
    /// MainWindow.xaml 的交互逻辑
    /// </summary>
    public partial class MainWindow : Window, INotifyPropertyChanged
    {
        private Config config;
        private string templateHtml;
        private StructuredText structuredText;
        private Image printImage;

        public event PropertyChangedEventHandler PropertyChanged;

        public Config Config
        {
            get
            {
                return config;
            }
            set
            {
                config = value;
                if (PropertyChanged != null) PropertyChanged(this, new PropertyChangedEventArgs("Config"));
            }
        }

        public int BrowserWidth
        {
            get => config == null ? 10 : config.Settings.Width;
        }

        public int BroserHeight
        {
            get => config == null ? 10 : config.Settings.Height;
        }

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
            Config = JsonConvert.DeserializeObject<Config>(configNode.InnerText);
            structuredText = new StructuredText(config.ConfigItemList, templateHtml);
        }

        private void SetChildren()
        {
            SetBinding();
            foreach (var configItem in config.ConfigItemList)
            {
                StMain.Children.Add(new InputArea(configItem,Browser,ChangeHtml));
            }
        }

        private void SetBinding()
        {
            GdBrowser.SetBinding(WidthProperty, new Binding("Config.Settings.Width") { Source = this });
            GdBrowser.SetBinding(HeightProperty, new Binding("Config.Settings.Height") { Source = this });
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

        private async void SaveImage()
        {
            var browserHost = Browser.GetBrowserHost();
            var pageClient = Browser.GetBrowser().GetDevToolsClient().Page;
            browserHost.Invalidate(PaintElementType.View);
            var viewport = new Viewport() { Height = config.Settings.Height, Width = config.Settings.Width };
            var buffer = await pageClient.CaptureScreenshotAsync(CaptureScreenshotFormat.Jpeg, 100, viewport);
            //var buffer = await pageClient.CaptureScreenshotAsync();

            if (buffer.Data != null)
            {

                MemoryStream ms = new MemoryStream(buffer.Data);
                ms.Write(buffer.Data, 0, buffer.Data.Length);

                SaveFileDialog dialog = new SaveFileDialog(); 
                dialog.Filter = "JPEG图片 (*.jpg)|*.jpg";
                //dialog.Filter = "PNG图片 (*.png)|*.png";
                DialogResult dresult = dialog.ShowDialog();
                if (dresult == System.Windows.Forms.DialogResult.OK)
                {
                    string path = dialog.FileName;
                    try
                    {
                        File.WriteAllBytes(path, buffer.Data);
                        MessageBox.Show(path + "保存成功。");
                    }
                    catch (Exception e)
                    {
                        MessageBox.Show(path + "保存失败！错误信息：" + e.Message);
                    }
                }
            }
        }

        private async void BtnPrint_OnClick(object sender, RoutedEventArgs e)
        {
            var browserHost = Browser.GetBrowserHost();
            var pageClient = Browser.GetBrowser().GetDevToolsClient().Page;
            browserHost.Invalidate(PaintElementType.View);
            var viewport = new Viewport() { Height = config.Settings.Height, Width = config.Settings.Width };
            var imageRawBuffer = await pageClient.CaptureScreenshotAsync(CaptureScreenshotFormat.Png, 100, viewport);
            if (imageRawBuffer.Data != null)
            {
                using (var stream = new MemoryStream(imageRawBuffer.Data))
                {
                    printImage = System.Drawing.Image.FromStream(stream, false, true);
                }
                PrintDocument pd = new PrintDocument();
                //pd.PrinterSettings.PrinterName = Config.Settings.Printer;
                var ps = new PageSettings();
                ps.Margins = new Margins(0, 0, 0, 0);
                ps.PaperSize = new PaperSize("Card", Config.Settings.Width, Config.Settings.Height);
                pd.DefaultPageSettings = ps;
                pd.PrintPage += PdOnPrintPage;
                pd.Print();
            }
        }

        private void PdOnPrintPage(object sender, PrintPageEventArgs e)
        {
            e.Graphics.DrawImageUnscaled(printImage, 0, 0);
            e.HasMorePages = false;
        }
    }
}
