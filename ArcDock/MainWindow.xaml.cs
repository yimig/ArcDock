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
using System.Text.RegularExpressions;
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
using System.Windows.Interop;
using ArcDock.Data.Database;
using PDFtoPrinter;
using Spire.Pdf;
using Path = System.IO.Path;

namespace ArcDock
{
    /// <summary>
    /// 主窗口
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

        /// <summary>
        /// 根据模板动态生成的UI区域
        /// </summary>
        private ControlDock controlDock;

        /// <summary>
        /// 配置列表，对应模板解析后的配置
        /// </summary>
        private List<Config> configList;

        /// <summary>
        /// 模板的原文列表
        /// </summary>
        private List<string> templateHtmlList;

        /// <summary>
        /// 模板文件路径（相对）
        /// </summary>
        private string[] templateFiles;

        /// <summary>
        /// 历史记录
        /// </summary>
        private History history;

        /// <summary>
        /// 最大打印页数
        /// </summary>
        private int MaxPrintPage { get; set; }
        
        /// <summary>
        /// 目前正在打印的页数
        /// </summary>
        private int NowPrintPage { get; set; }

        /// <summary>
        /// 选择打印Api
        /// 0=DocumentPrint，1=Cef原生print()函数，2=调用CLodop JS函数，3=调用PDFToPrinter API转印，4=调用Spire.PDF API转印
        /// </summary>
        private int PrintApi
        {
            get => Properties.Settings.Default.UserPrintApi;
            set
            {
                Properties.Settings.Default.UserPrintApi = value;
                Properties.Settings.Default.Save();
            }
        }

        private bool IsEnableRules
        {
            get => Properties.Settings.Default.IsEnableRules;
            set
            {
                Properties.Settings.Default.IsEnableRules = value;
                Properties.Settings.Default.Save();
            }
        }

        #endregion

        #region 初始化

        public MainWindow()
        {
            configList = new List<Config>();
            templateHtmlList = new List<string>();
            history = new History();
            MaxPrintPage = 1;
            NowPrintPage = 1;
            PrintApi = Properties.Settings.Default.UserPrintApi;
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

        /// <summary>
        /// 初始化动态UI区域
        /// </summary>
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

        /// <summary>
        /// 初始化模板选择下拉列表框
        /// </summary>
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

        /// <summary>
        /// 将目前预览保存为HTML
        /// </summary>
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

        /// <summary>
        /// 切换配置文件
        /// </summary>
        /// <param name="index">配置文件顺序</param>
        private void ChangeConfig(int index)
        {
            Config = configList[index];
            templateHtml = templateHtmlList[index];
            structuredText = new StructuredText(Config.ConfigItemList, templateHtml);
            SetControlDock();
        }

        /// <summary>
        /// 打印预览标签
        /// </summary>
        private void PrintWeb()
        {
            if (!IsEnableRules || CheckRules())
            { 
                if (PrintApi == 0) PrintWebApi();
                else if (PrintApi == 1) PrintWebJs();
                else if (PrintApi == 2) PrintWebClodop();
                else if (PrintApi == 3) PrintWebPdf();
                else if (PrintApi == 4) PrintWebPdfSprie();
            }
        }

        /// <summary>
        /// 使用原生DocumentPrint打印
        /// </summary>
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
                SetPrinter(ps,null);
                ps.Color = false;
                pd.PrintPage += PdOnPrintPage;
                pd.Print();
            }
        }

        /// <summary>
        /// 设置打印机，配置文件内容为空时，使用默认打印机
        /// </summary>
        /// <param name="pageSettings">使用DocumentPrint模式的打印机配置，非该模式输入null</param>
        /// <param name="printerSettings">使用Spire.PDF API的打印机配置，非该模式输入null</param>
        private void SetPrinter(PageSettings pageSettings,Spire.Pdf.Print.PdfPrintSettings printerSettings)
        {
            if (!config.Settings.Printer.Equals(String.Empty))
            {
                if (PrinterSettings.InstalledPrinters.Cast<string>().Any(str => str == config.Settings.Printer))
                {
                    if (pageSettings != null) pageSettings.PrinterSettings.PrinterName = config.Settings.Printer;
                    else printerSettings.PrinterName = config.Settings.Printer;
                }
            }
        }

        /// <summary>
        /// 使用CEF附带print()方法打印，无法静默打印。
        /// </summary>
        private void PrintWebJs()
        {
            Browser.GetMainFrame().ExecuteJavaScriptAsync("window.print()");
        }

        /// <summary>
        /// 使用Clodop插件打印，必须安装插件，插件IE渲染打印效果，可能与预览效果不同
        /// </summary>
        private void PrintWebClodop()
        {
            Browser.GetMainFrame().ExecuteJavaScriptAsync("var oHead = document.getElementsByTagName('HEAD').item(0);"+
                                                          "var oScript= document.createElement(\"script\");"+
                                                          "oScript.type = \"text/javascript\";"+
                                                          "oScript.src=\"http://localhost:8000/CLodopfuncs.js\";"+
                                                          "oScript.async = \"async\""+
                                                          "oHead.appendChild(oScript);" +
                                                          "CLODOP.PRINT_INIT('ArcProject');"+
                                                          "CLODOP.ADD_PRINT_HTM(0,0,\"100%\",\"100%\",document.getElementsByTagName('html')[0].innerHTML);"+
                                                          "CLODOP.PRINT();");
        }

        /// <summary>
        /// 使用PrintToPDF API打印模板
        /// </summary>
        private async void PrintWebPdf()
        {
            var ppw = new PrintProgressWindow();
            ppw.Show();
            ppw.ChangeStatue(30, "转储PDF...");
            await Browser.WebBrowser.PrintToPdfAsync(@"temp.pdf", new PdfPrintSettings
                {
                    HeaderFooterTitle = null,
                    HeaderFooterUrl = null,
                    MarginType = CefPdfPrintMarginType.Custom,
                    ScaleFactor = 99,
                    HeaderFooterEnabled = false,
                    SelectionOnly = false,
                    MarginTop = 0,
                    MarginBottom = 0,
                    MarginLeft = 0,
                    MarginRight = 0,
                    PageHeight = Config.Settings.PrintHeight * 10000,
                    PageWidth = Config.Settings.PrintWidth * 10000,
                    Landscape = false
                }
            );
            ppw.ChangeStatue(90, "交付打印...");
            var printer = new PDFtoPrinterPrinter();
            await printer.Print(new PrintingOptions(config.Settings.Printer, @"temp.pdf"));
            ppw.Close();
        }

        /// <summary>
        /// 使用Spire.PDF API打印，免费版PDF最多一次载入十页
        /// </summary>
        private async void PrintWebPdfSprie()
        {
            await Browser.WebBrowser.PrintToPdfAsync(@"temp.pdf", new PdfPrintSettings
                {
                    HeaderFooterTitle = null,
                    HeaderFooterUrl = null,
                    MarginType = CefPdfPrintMarginType.Custom,
                    ScaleFactor = 0,
                    HeaderFooterEnabled = false,
                    SelectionOnly = false,
                    MarginTop = 0,
                    MarginBottom = 0,
                    MarginLeft = 0,
                    MarginRight = 0,
                    PageHeight = Config.Settings.PrintHeight * 10000,
                    PageWidth = Config.Settings.PrintWidth * 10000,
                    Landscape = false
                }
            );
            PdfDocument doc = new PdfDocument();
            doc.PrintSettings.Landscape = false;
            doc.PrintSettings.SetPaperMargins(0, 0, 0, 0);
            doc.LoadFromFile(@"temp.pdf");
            doc.PrintSettings.SelectPageRange(1, 1);

            // SetPrinter(null,doc.PrintSettings);
            // doc.PrintSettings.PrintController = new StandardPrintController();
            // doc.Print();
        }

        /// <summary>
        /// 将当前填入值保存进数据库
        /// </summary>
        /// <param name="printType">打印类型：Manual=手动单次打印，Batch=批量打印，Html=保存为网页文件，Image=保存为图像文件</param>
        private void SaveHistory(string printType)
        {
            var fileName = Path.GetFileName(templateFiles[cbTemplate.SelectedIndex]);
            var result = new HistoryResult(fileName, printType);
            foreach (var configItem in Config.ConfigItemList)
            {
                var resItem = new HistoryResultItem();
                resItem.Name = configItem.Name;
                resItem.Id = configItem.Id;
                resItem.Content = structuredText[configItem.Id];
                result.ResultItems.Add(resItem);
            }
            history.AddHistory(result,DateTime.Now);
        }

        /// <summary>
        /// 清除动态UI区域
        /// </summary>
        private void ClearDock()
        {
            controlDock.ResetChildrenContent();
        }

        /// <summary>
        /// 修改动态UI区域的控件值
        /// </summary>
        /// <param name="id">控件的预留值ID</param>
        /// <param name="content">控件值</param>
        private void CheckAndFill(string id, string content)
        {
            if (config.ConfigItemList.Any(item => item.Id.Equals(id)) && content != String.Empty)
            {
                controlDock.SetChildrenContentValue(id, content);
                AutoFillContent(id,content);
            }
        }

        /// <summary>
        /// 触发自动填充其他符合模板预留值的内容
        /// </summary>
        /// <param name="id">预留值ID</param>
        /// <param name="content">预留值内容</param>
        private void AutoFillContent(string id, string content)
        {
            var temp_config = Config.ConfigItemList.Single(item => item.Id == id);
            if (temp_config.OptionType == 2)
            {
                try
                {
                    var executeItems = temp_config.OptionItemList.Single(option => option.Content.Equals(content)).ExecutionItemList;
                    foreach (var execItem in executeItems)
                    {
                        controlDock.SetChildrenContentValue(execItem.Key, execItem.Content);
                    }
                } catch(InvalidOperationException) {}

            }
        }

        /// <summary>
        /// 检查模板预设规则
        /// </summary>
        /// <returns>是否全部通过规则</returns>
        private bool CheckRules()
        {
            foreach (var configItem in Config.ConfigItemList)
            {
                if (configItem.Rules != String.Empty)
                {
                    var text = structuredText[configItem.Id];
                    var patten = configItem.Rules;
                    var res = Regex.IsMatch(structuredText[configItem.Id], configItem.Rules, RegexOptions.IgnoreCase);
                    if (!Regex.IsMatch(structuredText[configItem.Id], configItem.Rules,RegexOptions.Singleline))
                    {
                        MessageBox.Show("输入项【" + configItem.Name + "】不满足模板预设规则，请重新输入。");
                        var ca = controlDock.CustomAreas.Single(area => area.Id == configItem.Id);
                        ca.InputControl.Focus();
                        return false;
                    }
                }
            }

            return true;
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

        /// <summary>
        /// 刷新模板预览按钮的事件处理
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void BtnRefurbish_OnClick(object sender, RoutedEventArgs e)
        {
            Browser.Reload();
        }

        /// <summary>
        /// 显示模板控制台的事件处理
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void BtnConsole_OnClick(object sender, RoutedEventArgs e)
        {
            var wininfo = new WindowInfo();
            wininfo.SetAsPopup(new WindowInteropHelper(this).Handle, "DevTools");
            Browser.ShowDevTools(wininfo);
        }

        /// <summary>
        /// 修改模板下拉列表的事件处理
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void CbTemplate_OnSelectionChanged(object sender, SelectionChangedEventArgs e)
        {
            if(cbTemplate.SelectedIndex != -1) ChangeConfig(cbTemplate.SelectedIndex);
        }

        /// <summary>
        /// 显示历史按钮的事件处理
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void BtnHistory_OnClick(object sender, RoutedEventArgs e)
        {
            new HistoryWindow(history).Show();
        }

        /// <summary>
        /// 关闭主窗口的事件处理
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void MainWindow_OnClosing(object sender, CancelEventArgs e)
        {
            history.RemoveOutDateHistory();
        }

        /// <summary>
        /// 新建项目按钮的事件处理
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void BtnNew_OnClick(object sender, RoutedEventArgs e)
        {
            ClearDock();
        }

        /// <summary>
        /// 工具栏新建项目按钮的事件处理
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void BtnToolNew_OnClick(object sender, RoutedEventArgs e)
        {
            if (MessageBox.Show("受否新建项目？已经填入的信息将被删除！", "提示", MessageBoxButton.OKCancel) == MessageBoxResult.OK)
            {
                ClearDock();
            }
        }
        
        /// <summary>
        /// 工具栏分析患者信息按钮的事件处理
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
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
                    { "medicament_name", analyseWindow.MedicamentName },
                    { "medicament_num", analyseWindow.MedicamentNum },
                };
                foreach (var pair in checkDict)
                {
                    CheckAndFill(pair.Key, pair.Value);
                }
            }
        }

        /// <summary>
        /// 工具栏多页批量打印按钮的事件处理
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
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

        /// <summary>
        /// 菜单导出为HTML按钮的事件处理
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void MiOutputHtml_OnClick(object sender, RoutedEventArgs e)
        {
            SaveHtml();
            SaveHistory("Html");
        }

        /// <summary>
        /// 菜单输出为图像按钮的事件处理
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void MiOutputImage_OnClick(object sender, RoutedEventArgs e)
        {
            SaveImage();
            SaveHistory("Image");
        }

        /// <summary>
        /// 菜单查看软件信息按钮的事件处理
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void MiSoftwareInfo_OnClick(object sender, RoutedEventArgs e)
        {
            (new AboutWindow()).Show();
        }

        /// <summary>
        /// 菜单全局设置按钮的事件处理
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void MiGlobalSetting_OnClick(object sender, RoutedEventArgs e)
        {
            var settingWindow = new SettingWindow(PrintApi, IsEnableRules);
            settingWindow.ShowDialog();
            this.PrintApi = settingWindow.PrintApi;
            this.IsEnableRules = settingWindow.IsEnableRules;
        }
        private void MiTemplateSetting_OnClick(object sender, RoutedEventArgs e)
        {
            var tsWindow = new TemplateWindow();
            tsWindow.ShowDialog();
        }

        #endregion
    }
}
