using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Drawing;
using System.Drawing.Drawing2D;
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
using System.Windows.Interop;
using ArcDock.Data.Database;
using Path = System.IO.Path;
using TextChangedEventArgs = AutoCompleteTextBox.Editors.TextChangedEventArgs;

namespace ArcDock
{
    /// <summary>
    /// MainWindow.xaml 的交互逻辑
    /// </summary>
    public partial class MainWindow : Window, INotifyPropertyChanged
    {
        #region 字段属性和事件

        /// <summary>
        /// 当前使用的配置
        /// </summary>
        private Config config;

        /// <summary>
        /// 模板HTML原文
        /// </summary>
        private string templateHtml;

        /// <summary>
        /// 结构化文本，操作模板的预留值
        /// </summary>
        private StructuredText structuredText;

        /// <summary>
        /// 点击打印时生成的网页截图
        /// </summary>
        private Image printImage;

        /// <summary>
        /// 绑定前台配置（网页视口大小）
        /// </summary>
        public event PropertyChangedEventHandler PropertyChanged;

        /// <summary>
        /// 当前使用的配置属性
        /// </summary>
        public Config Config
        {
            get { return config; }
            set
            {
                config = value;
                if (PropertyChanged != null) PropertyChanged(this, new PropertyChangedEventArgs("Config"));
            }
        }

        private ControlDock controlDock;

        private List<Config> configList;

        private List<string> templateHtmlList;

        private string[] templateFiles;

        private History history;

        private int MaxPrintPage { get; set; }
        private int NowPrintPage { get; set; }

        #endregion

        #region 初始化

        public MainWindow()
        {
            configList = new List<Config>();
            templateHtmlList = new List<string>();
            history = new History();
            MaxPrintPage = 1;
            NowPrintPage = 1;
            InitializeComponent();
            LoadConfig(); //载入配置文件
            SetChildren(); //初始化UI
            Browser.Address = Environment.CurrentDirectory + "\\temp.html"; //初始化浏览器导航地址
        }

        /// <summary>
        /// 载入配置文件
        /// </summary>
        private void LoadConfig()
        {
            templateFiles = Directory.GetFiles(@"template", "*.html");
            foreach (var file in templateFiles)
            {
                TextReader tReader = new StreamReader(new FileStream(file, FileMode.Open));
                templateHtml = tReader.ReadToEnd();
                tReader.Close();
                XmlDocument xDoc = new XmlDocument();
                xDoc.LoadXml(templateHtml);
                XmlNode configNode = xDoc.SelectSingleNode("//script[@type=\"config/json\"]");
                templateHtmlList.Add(templateHtml);
                configList.Add(JsonConvert.DeserializeObject<Config>(configNode.InnerText));
            }

            ChangeConfig(0);
        }

        /// <summary>
        /// 初始化UI
        /// </summary>
        private void SetChildren()
        {
            controlDock = new ControlDock();
            SetBinding();
            SetControlDock();
            GdMain.Children.Add(controlDock);
            SetCheckBoxTemplate();
        }

        private void SetControlDock()
        {
            if (controlDock != null)
            {
                controlDock.ClearChildren();
                foreach (var configItem in Config.ConfigItemList)
                {
                    var area = CustomArea.GetCustomArea(configItem, Browser, ChangeHtml);
                    if (configItem.OptionType == 2)
                    {
                        var areaAutoFill = area as AutoInputArea;
                        areaAutoFill.MainDock = controlDock;
                    }
                    controlDock.AddArea(area);
                }
            }
        }

        private void SetCheckBoxTemplate()
        {
            cbTemplate.ItemsSource = templateFiles.Select(file => Path.GetFileName(file));
            cbTemplate.SelectedIndex = 0;
        }

        /// <summary>
        /// MVVM绑定
        /// </summary>
        private void SetBinding()
        {
            GdBrowser.SetBinding(WidthProperty, new Binding("Config.Settings.Width") { Source = this });
            GdBrowser.SetBinding(HeightProperty, new Binding("Config.Settings.Height") { Source = this });
        }

        #endregion

        #region 功能解耦

        /// <summary>
        /// 修改模板预留值
        /// </summary>
        /// <param name="id">预留值ID</param>
        /// <param name="content">修改预留值内容</param>
        private void ChangeHtml(string id, string content)
        {
            structuredText[id] = content;
            TextWriter tw = new StreamWriter(new FileStream(@"temp.html", FileMode.Create));
            tw.Write(structuredText);
            tw.Close();
        }

        /// <summary>
        /// 保存目前浏览器视口截图文件
        /// </summary>
        private async void SaveImage()
        {
            //截图配置
            var browserHost = Browser.GetBrowserHost();
            var pageClient = Browser.GetBrowser().GetDevToolsClient().Page;
            browserHost.Invalidate(PaintElementType.View);
            var viewport = new Viewport() { Height = Config.Settings.Height, Width = Config.Settings.Width };
            var buffer = await pageClient.CaptureScreenshotAsync(CaptureScreenshotFormat.Jpeg, 100, viewport);
            if (buffer.Data != null)
            {
                //选取保存文件
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

        private void SaveHtml()
        {
            SaveFileDialog dialog = new SaveFileDialog();
            dialog.Filter = "HTML页面 (*.html)|*.html";
            DialogResult dresult = dialog.ShowDialog();
            if (dresult == System.Windows.Forms.DialogResult.OK)
            {
                string path = dialog.FileName;
                try
                {
                    File.Copy(@"temp.html", path, true);
                    MessageBox.Show(path + "保存成功。");
                }
                catch (Exception e)
                {
                    MessageBox.Show(path + "保存失败！错误信息：" + e.Message);
                }
            }
        }

        private void ChangeConfig(int index)
        {
            Config = configList[index];
            templateHtml = templateHtmlList[index];
            structuredText = new StructuredText(Config.ConfigItemList, templateHtml);
            SetControlDock();
        }

        private void PrintWeb()
        {
            PrintWebClodop();
        }

        private async void PrintWebApi()
        {
            //截图配置，触发截图操作时会强制销毁打印机视图Graphics，故不能再打印事件中进行截图，先截好存起来
            var browserHost = Browser.GetBrowserHost();
            var pageClient = Browser.GetBrowser().GetDevToolsClient().Page;
            browserHost.Invalidate(PaintElementType.View);
            var viewport = new Viewport() { Height = Config.Settings.Height, Width = Config.Settings.Width };
            var imageRawBuffer =
                await pageClient.CaptureScreenshotAsync(CaptureScreenshotFormat.Png, 100, viewport); //截图
            if (imageRawBuffer.Data != null)
            {
                using (var stream = new MemoryStream(imageRawBuffer.Data))
                {
                    printImage = System.Drawing.Image.FromStream(stream, false, true); //暂存截图
                }

                // 初始化打印机信息
                PrintDocument pd = new PrintDocument();
                //pd.PrinterSettings.PrinterName = Config.Settings.Printer;
                var ps = new PageSettings();
                ps.Margins = new Margins(0, 0, 0, 0);
                ps.PaperSize = new PaperSize("Card", Config.Settings.Width, Config.Settings.Height);
                pd.DefaultPageSettings = ps;
                SetPrinter(ps);
                ps.Color = false;
                pd.PrintPage += PdOnPrintPage;
                pd.Print();
            }
        }

        private void SetPrinter(PageSettings ps)
        {
            if (!config.Settings.Printer.Equals(String.Empty))
            {
                if (PrinterSettings.InstalledPrinters.Cast<string>().Any(str => str == config.Settings.Printer))
                {
                    ps.PrinterSettings.PrinterName = config.Settings.Printer;
                }
            }
        }

        private void PrintWebJs()
        {
            Browser.GetMainFrame().ExecuteJavaScriptAsync("window.print()");
        }

        private void PrintWebClodop()
        {
            Browser.GetMainFrame().ExecuteJavaScriptAsync("var oHead = document.getElementsByTagName('HEAD').item(0);"+
                                                          "var oScript= document.createElement(\"script\");"+
                                                          "oScript.type = \"text/javascript\";"+
                                                          "oScript.src=\"http://192.168.56.1:8000/CLodopfuncs.js\";"+
                                                          "oHead.appendChild(oScript); +" +
                                                          "CLODOP.PRINT_INIT('ArcProject');"+
                                                          "CLODOP.ADD_PRINT_HTM(0,0,\"100%\",\"100%\",document.getElementsByTagName('html')[0].innerHTML);"+
                                                          "CLODOP.PRINT();");
        }

        private void SaveHistory(string printType)
        {
            var fileName = Path.GetFileName(templateFiles[cbTemplate.SelectedIndex]);
            var result = new TemplateResult(fileName, printType);
            foreach (var configItem in Config.ConfigItemList)
            {
                var resItem = new TemplateResultItem();
                resItem.Name = configItem.Name;
                resItem.Id = configItem.Id;
                resItem.Content = structuredText[configItem.Id];
                result.ResultItems.Add(resItem);
            }
            history.AddHistory(result,DateTime.Now);
        }

        private void ClearDock()
        {
            controlDock.ResetChildrenContent();
        }


        private void CheckAndFill(string id, string content)
        {
            if (config.ConfigItemList.Any(item => item.Id.Equals(id)) && content != String.Empty)
            {
                controlDock.SetChildrenContentValue(id, content);
            }
        }

        #endregion

        #region 事件处理

        /// <summary>
        /// 打印按钮按下处理事件，截图并初始化打印机信息
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void BtnPrint_OnClick(object sender, RoutedEventArgs e)
        {
            PrintWeb();
            SaveHistory("Manual");
        }

        /// <summary>
        /// 打印处理事件
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void PdOnPrintPage(object sender, PrintPageEventArgs e)
        {
            //设置打印精度，全部调为最高
            e.Graphics.Clear(Color.White);
            e.Graphics.PageUnit = GraphicsUnit.Millimeter;
 
            e.Graphics.CompositingMode = CompositingMode.SourceCopy;
            e.Graphics.SmoothingMode = SmoothingMode.HighQuality;
            e.Graphics.CompositingQuality = CompositingQuality.HighQuality;
            e.Graphics.PixelOffsetMode = PixelOffsetMode.None; //不偏移像素，否则模糊
            e.Graphics.InterpolationMode = InterpolationMode.HighQualityBicubic;
            e.Graphics.DrawImageUnscaled(printImage, 0, 0,Config.Settings.Width,Config.Settings.PrintHeight); //渲染打印图片
            if (NowPrintPage < MaxPrintPage)
            {
                NowPrintPage++;
                e.HasMorePages = true;
            }
            else
            {
                NowPrintPage = 1;
                MaxPrintPage = 1;
                e.HasMorePages = false; //没有后续打印页
            }
        }

        private void BtnRefurbish_OnClick(object sender, RoutedEventArgs e)
        {
            Browser.Reload();
        }

        private void BtnConsole_OnClick(object sender, RoutedEventArgs e)
        {
            var wininfo = new WindowInfo();
            wininfo.SetAsPopup(new WindowInteropHelper(this).Handle, "DevTools");
            Browser.ShowDevTools(wininfo);
        }

        private void CbTemplate_OnSelectionChanged(object sender, SelectionChangedEventArgs e)
        {
            if(cbTemplate.SelectedIndex != -1) ChangeConfig(cbTemplate.SelectedIndex);
        }

        private void BtnHistory_OnClick(object sender, RoutedEventArgs e)
        {
            new HistoryWindow(history).Show();
        }

        private void MainWindow_OnClosing(object sender, CancelEventArgs e)
        {
            history.RemoveOutDateHistory();
        }

        private void BtnNew_OnClick(object sender, RoutedEventArgs e)
        {
            ClearDock();
        }

        private void BtnToolNew_OnClick(object sender, RoutedEventArgs e)
        {
            if (MessageBox.Show("受否新建项目？已经填入的信息将被删除！", "提示", MessageBoxButton.OKCancel) == MessageBoxResult.OK)
            {
                ClearDock();
            }
        }

        private void BtnToolAnalyse_OnClick(object sender, RoutedEventArgs e)
        {
            var analyseWindow = new AnalyseStringWindow();
            analyseWindow.ShowDialog();
            if (analyseWindow.IsHasContent)
            {
                var checkDict = new Dictionary<string, string>()
                {
                    { "patient_name", analyseWindow.PatientName },
                    { "patient_bed", analyseWindow.BedNo },
                    { "patient_no", analyseWindow.InPatientNo },
                    { "patient_dept", analyseWindow.PatientDept },
                };
                foreach (var pair in checkDict)
                {
                    CheckAndFill(pair.Key, pair.Value);
                }
            }
        }

        private void BtnToolMultiPrint_OnClick(object sender, RoutedEventArgs e)
        {
            var spWindow = new SelectPrintNumWindow();
            spWindow.ShowDialog();
            if (spWindow.IsPrint)
            {
                MaxPrintPage = spWindow.PageNumber;
                PrintWeb();
                SaveHistory("Batch");
            }
        }

        private void MiOutputHtml_OnClick(object sender, RoutedEventArgs e)
        {
            SaveHtml();
            SaveHistory("Html");
        }

        private void MiOutputImage_OnClick(object sender, RoutedEventArgs e)
        {
            SaveImage();
            SaveHistory("Image");
        }

        private void MiSoftwareInfo_OnClick(object sender, RoutedEventArgs e)
        {
            (new AboutWindow()).Show();
        }

        #endregion
    }
}
