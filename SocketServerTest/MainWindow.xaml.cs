using System;
using System.Net.Sockets;
using System.Runtime.InteropServices;
using System.Windows;

namespace SocketServerTest
{
    /// <summary>
    /// Interaction logic for MainWindow.xaml
    /// </summary>
    public partial class MainWindow : Window
    {
        private Socket socket;
        private byte[] byte_arr;
        private IntPtr ptr;
        private const int SIZE = 1048576000;
        public MainWindow()
        {
            InitializeComponent();
        }

        //private async void InitServer()
        //{
        //    IPEndPoint ipEndPoint = new IPEndPoint(IPAddress.Parse("127.0.0.1"), 3210);
        //    Socket listener = new Socket(
        //        ipEndPoint.AddressFamily,
        //        SocketType.Stream,
        //        ProtocolType.Tcp);
        //    socket = listener;

        //    listener.Bind(ipEndPoint);
        //}

        //private async void StartListen()
        //{
        //    socket.Listen(100);
        //    TbConsole.AppendText("Socket Server Launched!" + "\t\n");
        //    try
        //    {
        //        var handler = await socket.AcceptAsync();
        //        //while (true)
        //        //{

        //        //}

        //        // Receive message.
        //        for(int i=0;i<2;i++)
        //        {
        //            var num = 1024;
        //            var buffer = new ArraySegment<byte>(new byte[num]);
        //            var received = await handler.ReceiveAsync(buffer, SocketFlags.None);
        //            byte[] a = buffer.Select(x => x).ToArray();
        //            var response = Encoding.UTF8.GetString(buffer.ToArray(), 0, received);

        //            var eom = "<|EOM|>";
        //            if (response.IndexOf(eom) > -1 /* is end of message */)
        //            {
        //                TbConsole.Text += (
        //                    $"Socket server received message: \"{response.Replace(eom, "")}\"\t\n");

        //                var ackMessage = "<|ACK|>";
        //                var echoBytes = Encoding.UTF8.GetBytes(ackMessage);
        //                var echoBuffer = new ArraySegment<byte>(echoBytes);
        //                await handler.SendAsync(echoBuffer, 0);
        //                TbConsole.Text += (
        //                    $"Socket server sent acknowledgment: \"{ackMessage}\"\t\n");

        //                //break;
        //            }
        //        }
        //        TbConsole.AppendText("======End Communication=====");

        //        // Sample output:
        //        //    Socket server received message: "Hi friends 👋!"
        //        //    Socket server sent acknowledgment: "<|ACK|>"
        //    } catch (SocketException ex)
        //    {
        //        TbConsole.AppendText(ex.Message+ "\t\n");
        //    }
        //}

        private void BtnStop_Click(object sender, RoutedEventArgs e)
        {
            Marshal.FreeCoTaskMem(ptr);
            //if (socket != null)
            //{
            //    try
            //    {
            //        socket.Shutdown(SocketShutdown.Both);
            //    }
            //    catch (Exception ex)
            //    {
            //        TbConsole.AppendText(ex.Message);
            //    }
            //    finally
            //    {
            //        socket.Close();
            //        socket = null;
            //        TbConsole.AppendText("Socket Server Stoped!" + "\t\n");
            //    }
            //}
        }

        private void BtnStart_Click(object sender, RoutedEventArgs e)
        {

            ptr = Marshal.AllocCoTaskMem(SIZE);
            byte_arr = new byte[SIZE];
            for (var i = 0; i < byte_arr.Length; i++)
            {
                byte_arr[i] = (byte)8;
            }
            Marshal.Copy(byte_arr, 0, ptr, byte_arr.Length);
            //if (socket == null)
            //{
            //    InitServer();
            //    StartListen();
            //}
        }
    }
}
