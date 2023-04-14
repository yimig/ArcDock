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

        private const int VERSION = 1;
        private const int PORT = 3052;

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
            var netJson = new NetAsync()
            {
                Version = VERSION,
                Code = "200",
                Message = Path.GetFileName(config.FilePath),
                Token = token
            };
            return JsonConvert.SerializeObject(netJson);
        }

        //private string GetFileJson()
        //{
        //    TextReader tReader = new StreamReader(new FileStream(config.FilePath, FileMode.Open));
        //    var templateHtml = tReader.ReadToEnd();
        //    tReader.Close();
        //    var globalSetting = new GlobalSettingJson()
        //    {
        //        IsEnableCheck = Setting
        //    };
        //    var resultObj = new NetAsync()
        //    {
        //        Version = VERSION,
        //        Code = "200",
        //        Message = templateHtml,
        //        Token = token
        //    };
        //}

        private string CheckReceiveJson(string receiveStr)
        {
            NetAsync resultObj = new NetAsync()
            {
                Version = VERSION,
                Code = "500",
                Message = "封包错误",
                Token = ""
            };
            if (receiveStr != null)
            {
                var receiveObj = JsonConvert.DeserializeObject<NetAsync>(receiveStr);
                if(receiveObj.Code == "200")
                {
                    if (receiveObj.Token != token) resultObj = new NetAsync()
                    {
                        Version = VERSION,
                        Code = "511",
                        Message = "鉴权失败，无权访问",
                        Token = ""
                    };
                    else if (receiveObj.Version != VERSION) resultObj = new NetAsync()
                    {
                        Version = VERSION,
                        Code = "505",
                        Message = "客户端版本不匹配，请升级客户端",
                        Token = ""
                    };
                    else {
                        TextReader tReader = new StreamReader(new FileStream(config.FilePath, FileMode.Open));
                        var templateHtml = tReader.ReadToEnd();
                        tReader.Close();
                        resultObj = new NetAsync()
                        {
                            Version = VERSION,
                            Code = "200",
                            Message = templateHtml,
                            Token = token
                        };
                    };
                }
            }
            return JsonConvert.SerializeObject(resultObj);
        }

        private void SetStatus(int icon, string status)
        {
            IsConnected = icon;
            Status = status;
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
            var socket = new Socket(AddressFamily.InterNetwork, SocketType.Stream, ProtocolType.Tcp);
            try
            {
                IP = GetLocalIP();
                var endPoint = new IPEndPoint(IPAddress.Parse(IP), PORT);
                socket.Bind(endPoint);
                socket.Listen(10);
                SetStatus(1, "初始化完成，正在等待接收方访问");
                globalSocket = socket;
                AcceptSocket(socket);
            } catch (Exception ex)
            {
                MessageBox.Show(ex.Message);
                this.Close();
            }

        }

        private void AcceptSocket(Socket socket)
        {
            var Result = socket.BeginAccept(asyncSocket =>
            {
                var client = socket.EndAccept(asyncSocket);
                SetStatus(0, client.RemoteEndPoint.ToString() + "已连接");
                SocketSendText(client, GetTokenJson());
                var resStr = SocketReceiveText(client);
                var reSendStr = CheckReceiveJson(resStr);
                SocketSendText(client,reSendStr);
                AcceptSocket(socket);
            },null);
        }

        private string SocketReceiveText(Socket socket)
        {
            string res = "";
            byte[] buffer = new byte[1024];
            try
            {
                socket.BeginReceive(buffer, 0, buffer.Length, SocketFlags.None, socketReceive =>
                {
                    int length = socket.EndReceive(socketReceive);
                    if (length > 0)
                    {
                        res = Encoding.UTF8.GetString(buffer, 0, length);
                    }
                }, null);
            } catch (Exception ex)
            {
                MessageBox.Show(ex.Message);
                SetStatus(2, ex.Message);
            }
            return res;
        }

        private void SocketSendText(Socket socket, string text)
        {
            byte[] data = new byte[1024];
            data = Encoding.UTF8.GetBytes(text);
            try
            {
                socket.BeginSend(data, 0, data.Length, SocketFlags.None, socketResult =>
                {
                    socket.EndSend(socketResult);
                }, null);
            }
            catch (Exception ex)
            {
                MessageBox.Show(ex.Message);
                SetStatus(2, ex.Message);
            }
        }

        private void SocketSendFile(Socket socket, string file)
        {
            try
            {
                socket.BeginSendFile(file, socketResult =>
                {
                    socket.EndSend(socketResult);
                },null);
            }
            catch (Exception ex)
            {
                MessageBox.Show(ex.Message);
                SetStatus(2, ex.Message);
            }
        }

        private void Window_Closing(object sender, CancelEventArgs e)
        {
            //if(globalSocket != null)
            //{
            //    try
            //    {
            //        globalSocket.Shutdown(SocketShutdown.Both);
            //    } catch (Exception) { } 
            //    finally
            //    {
            //        globalSocket.Close();
            //        //globalSocket.Dispose();
            //    }
            //}
        }
    }
}
