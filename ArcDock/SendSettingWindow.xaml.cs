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
using System.Net;
using System.Net.Sockets;
using System.Net.NetworkInformation;
using System.ComponentModel;
using ArcDock.Data.Json;
using System.Security.Cryptography;
using Newtonsoft.Json;
using System.IO;
using Path = System.IO.Path;
using log4net;
using ArcDock.Data;
using Microsoft.Scripting.Hosting.Shell;
using System.Runtime.InteropServices;
using static IronPython.Modules._ast;

namespace ArcDock
{
    /// <summary>
    /// Interaction logic for SendSettingWindow.xaml
    /// </summary>
    public partial class SendSettingWindow : Window, INotifyPropertyChanged
    {
        public event PropertyChangedEventHandler PropertyChanged;
        private string status;
        private int isConnected;
        private Socket globalSocket;
        public string IP { get; set; }
        public string Status 
        {   get=>status;
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
            get=>isConnected;
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
        private Config config;
        private string fileJson;

        private const int VERSION = 1;
        private const int PORT = 3052;
        private static ILog log = LogManager.GetLogger("SocketServer");

        public SendSettingWindow(Config config)
        {
            InitializeComponent();
            this.DataContext = this;
            SetStatus(2, "正在初始化网络传输组件");
            StartServer();
            this.config = config;
        }

        private string GetToken()
        {
            byte[] rndSeries = new byte[64];
            var rng = new RNGCryptoServiceProvider();
            rng.GetBytes(rndSeries);
            return BitConverter.ToInt64(rndSeries,0).ToString("X");
        }

        private string GetTokenJson()
        {
            token = GetToken();
            fileJson = CreateFileJson();
            var netJson = new NetAsync()
            {
                Version = VERSION,
                Code = "200",
                Message = Path.GetFileName(config.FilePath),
                Token = token,
                Size = Encoding.UTF8.GetByteCount(fileJson)
            };
            return JsonConvert.SerializeObject(netJson);
        }

        private string CreateFileJson()
        {
            TextReader tReader = new StreamReader(new FileStream(config.FilePath, FileMode.Open));
            var templateHtml = tReader.ReadToEnd();
            tReader.Close();
            var resultObj = new NetAsync()
            {
                Version = VERSION,
                Code = "200",
                Message = templateHtml,
                Token = token,
            };
            if(CbGlobal.IsChecked == true)
            {
                resultObj.GlobalSetting = new GlobalSettingJson()
                {
                    IsEnableCheck = Properties.Settings.Default.IsEnableRules,
                    PrintApi = Properties.Settings.Default.UserPrintApi,
                    Code = PythonEnvironment.GetPythonCode()
                };
            }
            return JsonConvert.SerializeObject(resultObj);
        }

        private string GetFileJson()
        {
            return fileJson;
        }

        private string CheckVersionReceive(NetAsync receive)
        {
            NetAsync resultObj = new NetAsync()
            {
                Version = VERSION,
                Code = "500",
                Message = "封包错误",
                Token = ""
            };
            if (receive != null)
            {
                if(receive.Code == "200")
                {
                    if (receive.Version != VERSION) resultObj = new NetAsync()
                    {
                        Version = VERSION,
                        Code = "505",
                        Message = "客户端版本不匹配，请升级客户端",
                        Token = ""
                    };
                    else resultObj = new NetAsync() { Code = "200" };
                }
            }
            return resultObj.Code == "200" ? GetTokenJson() : JsonConvert.SerializeObject(resultObj);
        }

        private string CheckTokenReceive(NetAsync receive)
        {
            NetAsync resultObj = new NetAsync()
            {
                Version = VERSION,
                Code = "500",
                Message = "封包错误",
                Token = ""
            };
            if (receive != null)
            {
                if (receive.Code == "200")
                {
                    if (receive.Token != token) resultObj = new NetAsync()
                    {
                        Version = VERSION,
                        Code = "511",
                        Message = "鉴权失败，无权访问",
                        Token = ""
                    };
                    else resultObj = new NetAsync() { Code = "200" };
                }
            }
            return resultObj.Code == "200" ? GetFileJson() : JsonConvert.SerializeObject(resultObj);
        }

        private void SetStatus(int icon, string status)
        {
            IsConnected = icon;
            Status = status;
            log.Info(status);
        }

        private string GetLocalIP()
        {
            var ip = "0.0.0.0";
            if (!NetworkInterface.GetIsNetworkAvailable()) throw new Exception("无可用网络");
            else
            {
                var host = Dns.GetHostEntry(Dns.GetHostName());
                ip = host.AddressList.FirstOrDefault(addr => addr.AddressFamily == AddressFamily.InterNetwork).ToString();
            }
            return ip;
        }

        private void StartServer()
        {
            try
            {
                IP = GetLocalIP();
                IPEndPoint ipEndPoint = new IPEndPoint(IPAddress.Parse(IP), PORT);
                Socket listener = new Socket(
                    ipEndPoint.AddressFamily,
                    SocketType.Stream,
                    ProtocolType.Tcp);
                globalSocket = listener;
                listener.Bind(ipEndPoint);
                BeginListen();
            } catch (Exception ex)
            {
                MessageBox.Show(ex.Message);
                log.Error("Socket服务器启动失败",ex);
                this.Close();
            }

        }

        private async void BeginListen()
        {
            globalSocket.Listen(100);
            SetStatus(0,"Socket服务启动成功，等待其他客户端连接");
            try
            {
                var handler = await globalSocket.AcceptAsync();
                SetStatus(0, "Socket服务已连接");
                // Receive message.
                for (int i = 0; i < 2; i++)
                {
                    var buffer = new ArraySegment<byte>(new byte[1024]);
                    var received = await handler.ReceiveAsync(buffer, SocketFlags.None);
                    byte[] a = buffer.Select(x => x).ToArray();
                    var response = Encoding.UTF8.GetString(buffer.ToArray(), 0, received);
                    log.Debug("收到来自" + handler.RemoteEndPoint.ToString() + "的报文：\n" + response);
                    NetAsync receiveObj = JsonConvert.DeserializeObject<NetAsync>(response);
                    if(i == 0)
                    {
                        var sendStr = CheckVersionReceive(receiveObj);
                        if (receiveObj.Code == "200")
                        {
                            var echoBytes = Encoding.UTF8.GetBytes(sendStr);
                            var echoBuffer = new ArraySegment<byte>(echoBytes);
                            await handler.SendAsync(echoBuffer, 0);
                            SetStatus(0, "与客户端认证完毕");
                        }
                        else {
                            SetStatus(2, "由于对方请求，服务已断开：" + receiveObj.Message + " 识别代码：" + receiveObj.Code);
                            break;
                        }
                    } else if(i==1)
                    {
                        var sendStr = CheckTokenReceive(receiveObj);
                        if (receiveObj.Code == "200")
                        {
                            var echoBytes = Encoding.UTF8.GetBytes(sendStr);
                            var echoBuffer = new ArraySegment<byte>(echoBytes);
                            await handler.SendAsync(echoBuffer, 0);
                            SetStatus(0, "配置数据已成功发送，传送服务停止监听");
                        }
                        else {
                            SetStatus(2, "由于对方请求，服务已断开：" + receiveObj.Message + " 识别代码：" + receiveObj.Code);
                            break; 
                        }
                    }
                    log.Info("第"+(i+1)+"轮传输结束");
                }
            }
            catch (SocketException ex)
            {
                SetStatus(2, "挂载Socket监听服务时发生异常：" + ex.Message);
            }
        }

        private void Window_Closing(object sender, CancelEventArgs e)
        {
            if (globalSocket != null)
            {
                try
                {
                    globalSocket.Shutdown(SocketShutdown.Both);
                }
                catch (Exception ex)
                {
                    log.Error("关闭Socket服务时发生异常：",ex);
                }
                finally
                {
                    globalSocket.Close();
                    globalSocket = null;
                    log.Info("Socket服务已关闭");
                }
            }
        }
    }
}
