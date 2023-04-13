using System;
using System.Collections.Generic;
using System.Linq;
using System.Net.Sockets;
using System.Net;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;
using System.Windows.Documents;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using System.Windows.Navigation;
using System.Windows.Shapes;

namespace SocketServerTest
{
    /// <summary>
    /// Interaction logic for MainWindow.xaml
    /// </summary>
    public partial class MainWindow : Window
    {
        public MainWindow()
        {
            InitializeComponent();
            StartServer();
        }

        private async void StartServer()
        {
            IPEndPoint ipEndPoint = new IPEndPoint(IPAddress.Parse("127.0.0.1"), 3210);
            Socket listener = new Socket(
                ipEndPoint.AddressFamily,
                SocketType.Stream,
                ProtocolType.Tcp);

            listener.Bind(ipEndPoint);
            listener.Listen(100);

            var handler = await listener.AcceptAsync();
            //while (true)
            //{
            //    // Receive message.
            //    var buffer = new ArraySegment<byte>[1024];
            //    var received = await handler.ReceiveAsync(buffer, SocketFlags.None);
            //    byte[] a = buffer.ToArray();
            //    var response = Encoding.UTF8.GetString(buffer.ToArray(), 0, received);

            //    var eom = "<|EOM|>";
            //    if (response.IndexOf(eom) > -1 /* is end of message */)
            //    {
            //        TbConsole.Text+=(
            //            $"Socket server received message: \"{response.Replace(eom, "")}\"");

            //        var ackMessage = "<|ACK|>";
            //        var echoBytes = Encoding.UTF8.GetBytes(ackMessage);
            //        await handler.SendAsync(echoBytes, 0);
            //        TbConsole.Text += (
            //            $"Socket server sent acknowledgment: \"{ackMessage}\"");

            //        break;
            //    }
            //    // Sample output:
            //    //    Socket server received message: "Hi friends 👋!"
            //    //    Socket server sent acknowledgment: "<|ACK|>"
            //}
        }
    }
}
