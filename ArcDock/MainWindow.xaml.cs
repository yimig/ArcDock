﻿using System;
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

        #endregion

        #region 初始化

        public MainWindow()
        {
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
            TextReader tReader = new StreamReader(new FileStream(@"template/template.html", FileMode.Open));
            templateHtml = tReader.ReadToEnd();
            tReader.Close();
            XmlDocument xDoc = new XmlDocument();
            xDoc.LoadXml(templateHtml);
            XmlNode configNode = xDoc.SelectSingleNode("//script[@type=\"config/json\"]");
            Config = JsonConvert.DeserializeObject<Config>(configNode.InnerText);
            structuredText = new StructuredText(Config.ConfigItemList, templateHtml);
        }

        /// <summary>
        /// 初始化UI
        /// </summary>
        private void SetChildren()
        {
            SetBinding();
            foreach (var configItem in Config.ConfigItemList)
            {
                StMain.Children.Add(new InputArea(configItem,Browser,ChangeHtml));
            }
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
        /// 一次性保存所有修改
        /// </summary>
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

        #endregion

        #region 事件处理

        /// <summary>
        /// 打印按钮按下处理事件，截图并初始化打印机信息
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private async void BtnPrint_OnClick(object sender, RoutedEventArgs e)
        {
            //截图配置，触发截图操作时会强制销毁打印机视图Graphics，故不能再打印事件中进行截图，先截好存起来
            var browserHost = Browser.GetBrowserHost();
            var pageClient = Browser.GetBrowser().GetDevToolsClient().Page;
            browserHost.Invalidate(PaintElementType.View);
            var viewport = new Viewport() { Height = Config.Settings.Height, Width = Config.Settings.Width };
            var imageRawBuffer = await pageClient.CaptureScreenshotAsync(CaptureScreenshotFormat.Png, 100, viewport);//截图
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
                ps.Color = false;
                pd.PrintPage += PdOnPrintPage;
                pd.Print();
            }
        }

        /// <summary>
        /// 打印处理事件
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void PdOnPrintPage(object sender, PrintPageEventArgs e)
        {
            //设置打印精度，全部调为最高
            e.Graphics.CompositingMode = CompositingMode.SourceCopy;
            e.Graphics.CompositingQuality = CompositingQuality.HighQuality;
            e.Graphics.PixelOffsetMode = PixelOffsetMode.None; //不偏移像素，否则模糊
            e.Graphics.InterpolationMode = InterpolationMode.HighQualityBicubic;
            e.Graphics.DrawImageUnscaled(printImage, 0, 0); //渲染打印图片
            e.HasMorePages = false; //没有后续打印页
        }

        #endregion
    }
}
