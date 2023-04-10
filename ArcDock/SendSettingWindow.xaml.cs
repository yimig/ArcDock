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

        public SendSettingWindow()
        {
            InitializeComponent();
            this.DataContext = this;
            SetStatus(2, "正在初始化网络传输组件");
            StartServer();
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
                var endPoint = new IPEndPoint(IPAddress.Parse(IP), 3052);
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
            socket.BeginAccept(asyncSocket =>
            {
                var client = socket.EndAccept(asyncSocket);
                SetStatus(0, client.RemoteEndPoint.ToString() + "已连接");
                SocketSendText(client, "Welcome");
                var resStr = SocketReceiveText(client);
                SetStatus(0, "Client say:" + resStr);
                SocketSendFile(client, @"D:\Projects\VS_Projects\ArcDock\ArcDock\template\template.html");
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
