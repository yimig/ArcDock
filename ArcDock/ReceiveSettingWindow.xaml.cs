using ArcDock.Data;
using ArcDock.Data.Json;
using log4net;
using Newtonsoft.Json;
using System;
using System.ComponentModel;
using System.IO;
using System.Linq;
using System.Net;
using System.Net.Sockets;
using System.Text;
using System.Windows;

namespace ArcDock
{
    /// <summary>
    /// 接收配置窗口
    /// </summary>
    public partial class ReceiveSettingWindow : Window, INotifyPropertyChanged
    {

        #region 字段属性和事件

        public event PropertyChangedEventHandler PropertyChanged;
        private string status;
        private int isConnected;
        private Socket globalSocket;
        private int packageSize;
        private string fileName;

        /// <summary>
        /// 服务端IP
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
        /// 指示目前是否已连接到服务器
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
        /// Token验证值
        /// </summary>
        private string token;

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
        private static ILog log = LogManager.GetLogger("SocketClient");

        /// <summary>
        /// 文件存放路径
        /// </summary>
        private const string FILE_PATH = @"template\";

        #endregion

        #region 初始化

        /// <summary>
        /// 接收设置窗口
        /// </summary>
        public ReceiveSettingWindow()
        {
            InitializeComponent();
            this.DataContext = this;
            SetStatus(0, "就绪");
        }

        #endregion

        #region 功能解耦

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
        /// 创建初始访问数据包
        /// </summary>
        /// <returns></returns>
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

        /// <summary>
        /// 创建拒绝接收数据包
        /// </summary>
        /// <returns></returns>
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

        /// <summary>
        /// 启动客户端
        /// </summary>
        private void StartClient()
        {
            globalSocket = new Socket(AddressFamily.InterNetwork, SocketType.Stream, ProtocolType.Tcp);
            SetStatus(1, "客户端初始化完毕，等待连接服务端");
            try
            {
                BeginReceive();
            }
            catch (Exception ex)
            {
                MessageBox.Show(ex.Message);
                log.Error("Socket服务异常：", ex);
            }

        }

        /// <summary>
        /// 客户端收发流程
        /// </summary>
        private async void BeginReceive()
        {
            var endPoint = new IPEndPoint(IPAddress.Parse(IP), PORT);
            await globalSocket.ConnectAsync(endPoint); //尝试连接服务器
            SetStatus(0, "服务端已连接");

            var requestStr = JsonConvert.SerializeObject(CreateRequestPackage());
            var sendBytes = Encoding.UTF8.GetBytes(requestStr);
            await globalSocket.SendAsync(new ArraySegment<byte>(sendBytes), 0); //发送初始访问数据包

            var buffer = new ArraySegment<byte>(new byte[1024]);
            var received = await globalSocket.ReceiveAsync(buffer, SocketFlags.None); //接收服务器的回复，该回复应该包括下次报文的长度
            var response = Encoding.UTF8.GetString(buffer.ToArray(), 0, received);
            log.Debug("收到来自" + globalSocket.RemoteEndPoint.ToString() + "的报文：\n" + response);
            NetAsync receiveObj = JsonConvert.DeserializeObject<NetAsync>(response);
            if (receiveObj.Code == "200")
            {

                SetStatus(0, "与服务端认证完毕");
                if (MessageBox.Show("是否接收配置文件：" + receiveObj.Message + "?", "接受确认", MessageBoxButton.OKCancel) == MessageBoxResult.OK)
                {
                    packageSize = receiveObj.Size;
                    fileName = receiveObj.Message;

                    await globalSocket.SendAsync(buffer, 0); //将上次接收到的数据包再发送给服务器，表示确认接收
                    var fileBuffer = new ArraySegment<byte>(new byte[packageSize]);
                    var receivedFile = await globalSocket.ReceiveAsync(fileBuffer, SocketFlags.None); //接收配置文件
                    var responseFile = Encoding.UTF8.GetString(fileBuffer.ToArray(), 0, receivedFile);
                    log.Debug("收到来自" + globalSocket.RemoteEndPoint.ToString() + "的报文：\n" + responseFile);
                    NetAsync fileObj = JsonConvert.DeserializeObject<NetAsync>(responseFile);
                    SetStatus(0, "成功接收到配置");

                    if (fileObj.GlobalSetting != null)
                    {
                        if (MessageBox.Show("目标服务同时发送了全局配置，是否覆盖本地全局配置？", "替换全局配置", MessageBoxButton.OKCancel) == MessageBoxResult.OK)
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
                    await globalSocket.SendAsync(new ArraySegment<byte>(refuseBytes), 0); //用户拒绝接收配置，发送拒绝报文
                    SetStatus(2, "用户选择了拒绝接收配置");
                    return;
                }
            }
        }

        /// <summary>
        /// 保存配置文件
        /// </summary>
        /// <param name="text">配置文件内容</param>
        private void SaveConfig(string text)
        {
            TextWriter tWriter = new StreamWriter(new FileStream(FILE_PATH + fileName, FileMode.Create));
            tWriter.Write(text);
            tWriter.Close();
            MessageBox.Show("配置已保存,将重启程序以应用修改。");
            System.Windows.Forms.Application.Restart();
            Environment.Exit(0);
        }

        #endregion

        #region 事件处理

        /// <summary>
        /// 按下连接按钮的事件处理
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void BtnConnect_Click(object sender, RoutedEventArgs e)
        {
            StartClient();
        }

        #endregion
    }
}
