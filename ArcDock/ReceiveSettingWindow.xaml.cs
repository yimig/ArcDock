using ArcDock.Data;
using ArcDock.Data.Json;
using log4net;
using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.IO;
using System.Linq;
using System.Net;
using System.Net.Sockets;
using System.Runtime.InteropServices;
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
using static Org.BouncyCastle.Math.EC.ECCurve;
using System.Xml;

namespace ArcDock
{
    /// <summary>
    /// Interaction logic for ReceiveSettingWindow.xaml
    /// </summary>
    public partial class ReceiveSettingWindow : Window, INotifyPropertyChanged
    {
        public event PropertyChangedEventHandler PropertyChanged;
        private string status;
        private int isConnected;
        private Socket globalSocket;
        public string IP { get; set; }
        public string Status
        {
            get => status;
            set
            {
                status = value;
                if (PropertyChanged != null)
                {
                    PropertyChanged(this, new PropertyChangedEventArgs("Status"));
                }
            }
        }
        public int IsConnected
        {
            get => isConnected;
            set
            {
                isConnected = value;
                if (PropertyChanged != null)
                {
                    PropertyChanged(this, new PropertyChangedEventArgs("IsConnected"));
                }
            }
        }

        private string token;

        private const int VERSION = 1;
        private const int PORT = 3052;
        private static ILog log = LogManager.GetLogger("SocketClient");
        private int packageSize;
        private string fileName;
        private const string FILE_PATH = @"template\";

        public ReceiveSettingWindow()
        {
            InitializeComponent();
            this.DataContext = this;
            SetStatus(0, "就绪");
        }

        private void SetStatus(int icon, string status)
        {
            IsConnected = icon;
            Status = status;
            log.Info(status);
        }

        private NetAsync CreateRequestPackage()
        {
            return new NetAsync()
            {
                Version = VERSION,
                Code = "200",
                Message = "",
                Token = ""
            };
        }

        private NetAsync CreateRefusePackage()
        {
            return new NetAsync()
            {
                Version = VERSION,
                Code = "505",
                Message = "客户端拒绝接收文件",
                Token = ""
            };
        }

        private void StartClient()
        {
            globalSocket = new Socket(AddressFamily.InterNetwork, SocketType.Stream, ProtocolType.Tcp);
            SetStatus(1, "客户端初始化完毕，等待连接服务端");
            try
            {
                BeginReceive();
            } catch (Exception ex)
            {
                MessageBox.Show(ex.Message);
                log.Error("Socket服务异常：", ex);
            }

        }

        private async void BeginReceive() 
        {
            var endPoint = new IPEndPoint(IPAddress.Parse(IP), PORT);
            await globalSocket.ConnectAsync(endPoint);
            SetStatus(0, "服务端已连接");

            var requestStr = JsonConvert.SerializeObject(CreateRequestPackage());
            var sendBytes = Encoding.UTF8.GetBytes(requestStr);
            await globalSocket.SendAsync(new ArraySegment<byte>(sendBytes), 0);

            var buffer = new ArraySegment<byte>(new byte[1024]);
            var received = await globalSocket.ReceiveAsync(buffer, SocketFlags.None);
            var response = Encoding.UTF8.GetString(buffer.ToArray(), 0, received);
            log.Debug("收到来自" + globalSocket.RemoteEndPoint.ToString() + "的报文：\n" + response);
            NetAsync receiveObj = JsonConvert.DeserializeObject<NetAsync>(response);
            if (receiveObj.Code == "200")
            {

                SetStatus(0, "与服务端认证完毕");
                if(MessageBox.Show("是否接收配置文件："+receiveObj.Message+"?","接受确认",MessageBoxButton.OKCancel) == MessageBoxResult.OK)
                {
                    packageSize = receiveObj.Size;
                    fileName = receiveObj.Message;

                    await globalSocket.SendAsync(buffer, 0);
                    var fileBuffer = new ArraySegment<byte>(new byte[packageSize]);
                    var receivedFile = await globalSocket.ReceiveAsync(fileBuffer, SocketFlags.None);
                    var responseFile = Encoding.UTF8.GetString(fileBuffer.ToArray(), 0, receivedFile);
                    log.Debug("收到来自" + globalSocket.RemoteEndPoint.ToString() + "的报文：\n" + responseFile);
                    NetAsync fileObj = JsonConvert.DeserializeObject<NetAsync>(responseFile);
                    SetStatus(0, "成功接收到配置");

                    if (fileObj.GlobalSetting != null)
                    {
                        if(MessageBox.Show("目标服务同时发送了全局配置，是否覆盖本地全局配置？","替换全局配置",MessageBoxButton.OKCancel)==MessageBoxResult.OK)
                        {
                            Properties.Settings.Default.IsEnableRules = fileObj.GlobalSetting.IsEnableCheck;
                            Properties.Settings.Default.UserPrintApi = fileObj.GlobalSetting.PrintApi;
                            Properties.Settings.Default.Save();
                            new PythonEnvironment().UpdateCode(fileObj.GlobalSetting.Code);

                        }
                    }
                    SaveConfig(fileObj.Message);
                }
                else
                {
                    var refuseBytes = Encoding.UTF8.GetBytes(JsonConvert.SerializeObject(CreateRefusePackage()));
                    await globalSocket.SendAsync(new ArraySegment<byte>(refuseBytes), 0);
                    SetStatus(2, "用户选择了拒绝接收配置");
                    return;
                }
            }
        }

        private void SaveConfig(string text)
        {
            TextWriter tWriter = new StreamWriter(new FileStream(FILE_PATH+fileName, FileMode.Create));
            tWriter.Write(text);
            tWriter.Close();
            MessageBox.Show("配置已保存,将重启程序以应用修改。");
            System.Windows.Forms.Application.Restart();
            Environment.Exit(0);
        }

        private void BtnConnect_Click(object sender, RoutedEventArgs e)
        {
            StartClient();
        }
    }
}
