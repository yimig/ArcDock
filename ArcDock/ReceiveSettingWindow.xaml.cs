using ArcDock.Data.Json;
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;
using System.Net;
using System.Net.Sockets;
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
        }

        private void StartClient()
        {
            var socket = new Socket(AddressFamily.InterNetwork, SocketType.Stream, ProtocolType.Tcp);
            try
            {
                var endPoint = new IPEndPoint(IPAddress.Parse(IP), PORT);
                socket.BeginConnect(endPoint, resultAsync =>
                {
                    SetStatus(1, "连接成功，等待交换信息");
                    var text = SocketReceiveText(socket);
                    SocketSendText(socket, text);
                    SetStatus(1, "验证完毕，正在接收配置文件");
                    var res = SocketReceiveText(socket);
                }, null);
                SetStatus(1, "配置文件成功接收");
                globalSocket = socket;
            }
            catch (Exception ex)
            {
                MessageBox.Show(ex.Message);
                SetStatus(0,ex.Message);
            }
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
            }
            catch (Exception ex)
            {
                MessageBox.Show(ex.Message);
                SetStatus(0, ex.Message);
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
                SetStatus(0, ex.Message);
            }
        }

        private void BtnConnect_Click(object sender, RoutedEventArgs e)
        {
            StartClient();
        }
    }
}
