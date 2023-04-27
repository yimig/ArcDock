using ArcDock.Data;
using ArcDock.Data.Json;
using log4net;
using Newtonsoft.Json;
using System;
using System.ComponentModel;
using System.IO;
using System.Linq;
using System.Net;
using System.Net.NetworkInformation;
using System.Net.Sockets;
using System.Security.Cryptography;
using System.Text;
using System.Windows;
using Path = System.IO.Path;

namespace ArcDock
{
    /// <summary>
    /// 发送配置窗口
    /// </summary>
    public partial class SendSettingWindow : Window, INotifyPropertyChanged
    {
        #region 字段属性和事件
        
        public event PropertyChangedEventHandler PropertyChanged;
        private string status;
        private int isConnected;
        private Socket globalSocket;
        private string token;
        private Config config;
        private string fileJson;

        /// <summary>
        /// 本机IP
        /// </summary>
        public string IP { get; set; }
        /// <summary>
        /// 当前交互状态
        /// </summary>
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
        /// <summary>
        /// 指示目前是否已连接到客户端
        /// </summary>
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

        /// <summary>
        /// 传输版本，只有服务器与客户端相等才会连接，升级后若与旧版不兼容可修改值
        /// </summary>
        private const int VERSION = 1;

        /// <summary>
        /// 服务器端口号
        /// </summary>
        private const int PORT = 3052;

        /// <summary>
        /// 当前页面的日志控制对象
        /// </summary>
        private static ILog log = LogManager.GetLogger("SocketServer");

        #endregion

        #region 初始化

        /// <summary>
        /// 发送配置窗口
        /// </summary>
        /// <param name="config"></param>
        public SendSettingWindow(Config config)
        {
            InitializeComponent();
            this.DataContext = this;
            SetStatus(2, "正在初始化网络传输组件");
            StartServer();
            this.config = config;
        }

        /// <summary>
        /// 启动服务器
        /// </summary>
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
            }
            catch (Exception ex)
            {
                MessageBox.Show(ex.Message);
                log.Error("Socket服务器启动失败", ex);
                this.Close();
            }

        }

        #endregion

        #region 功能解耦


        /// <summary>
        /// 创建Token
        /// </summary>
        /// <returns>Token</returns>
        private string GetToken()
        {
            byte[] rndSeries = new byte[64];
            var rng = new RNGCryptoServiceProvider();
            rng.GetBytes(rndSeries);
            return BitConverter.ToInt64(rndSeries, 0).ToString("X");
        }

        /// <summary>
        /// 获取初次回复的报文
        /// </summary>
        /// <returns></returns>
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

        /// <summary>
        /// 获取包含配置文件的报文
        /// </summary>
        /// <returns></returns>
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
            if (CbGlobal.IsChecked == true)
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

        /// <summary>
        /// 检查客户端的报文版本是否匹配
        /// </summary>
        /// <param name="receive"></param>
        /// <returns></returns>
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
                if (receive.Code == "200")
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

        /// <summary>
        /// 检查客户端的Token是否匹配
        /// </summary>
        /// <param name="receive"></param>
        /// <returns></returns>
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
            return resultObj.Code == "200" ? fileJson : JsonConvert.SerializeObject(resultObj);
        }

        /// <summary>
        /// 设置状态提示内容
        /// </summary>
        /// <param name="icon">图标：0-成功，1-查找中，2-失败</param>
        /// <param name="status">提示文本</param>
        private void SetStatus(int icon, string status)
        {
            IsConnected = icon;
            Status = status;
            log.Info(status);
        }

        /// <summary>
        /// 获取本机IP
        /// </summary>
        /// <returns></returns>
        /// <exception cref="Exception"></exception>
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

        /// <summary>
        /// 服务器收发流程
        /// </summary>
        private async void BeginListen()
        {
            globalSocket.Listen(100);
            SetStatus(0, "Socket服务启动成功，等待其他客户端连接");
            try
            {
                var handler = await globalSocket.AcceptAsync();//监听到了客户端连接
                SetStatus(0, "Socket服务已连接");
                // Receive message.
                for (int i = 0; i < 2; i++)
                {
                    var buffer = new ArraySegment<byte>(new byte[1024]);
                    var received = await handler.ReceiveAsync(buffer, SocketFlags.None);//获取客户端发来的报文，i为会话次数
                    byte[] a = buffer.Select(x => x).ToArray();
                    var response = Encoding.UTF8.GetString(buffer.ToArray(), 0, received);
                    log.Debug("收到来自" + handler.RemoteEndPoint.ToString() + "的报文：\n" + response);
                    NetAsync receiveObj = JsonConvert.DeserializeObject<NetAsync>(response);
                    if (i == 0) //开始第一次会话
                    {
                        var sendStr = CheckVersionReceive(receiveObj);
                        if (receiveObj.Code == "200")
                        {
                            var echoBytes = Encoding.UTF8.GetBytes(sendStr);
                            var echoBuffer = new ArraySegment<byte>(echoBytes);
                            await handler.SendAsync(echoBuffer, 0); //客户端版本认证完毕，发送Token、下次报文的大小
                            SetStatus(0, "与客户端认证完毕");
                        }
                        else
                        {
                            SetStatus(2, "由于对方请求，服务已断开：" + receiveObj.Message + " 识别代码：" + receiveObj.Code);
                            break;
                        }
                    }
                    else if (i == 1) //开始第二次会话
                    {
                        var sendStr = CheckTokenReceive(receiveObj);
                        if (receiveObj.Code == "200")
                        {
                            var echoBytes = Encoding.UTF8.GetBytes(sendStr);
                            var echoBuffer = new ArraySegment<byte>(echoBytes);
                            await handler.SendAsync(echoBuffer, 0); //客户端确认接收，发送配置文件报文
                            SetStatus(0, "配置数据已成功发送，传送服务停止监听");
                        }
                        else
                        {
                            SetStatus(2, "由于对方请求，服务已断开：" + receiveObj.Message + " 识别代码：" + receiveObj.Code);
                            break;
                        }
                    }
                    log.Info("第" + (i + 1) + "轮传输结束");
                }
            }
            catch (SocketException ex)
            {
                SetStatus(2, "挂载Socket监听服务时发生异常：" + ex.Message);
            }
        }

        #endregion

        #region 事件处理

        /// <summary>
        /// 窗口关闭时的事件处理
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
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
                    log.Error("关闭Socket服务时发生异常：", ex);
                }
                finally
                {
                    globalSocket.Close();
                    globalSocket = null;
                    log.Info("Socket服务已关闭");
                }
            }
        }

        #endregion
    }
}
