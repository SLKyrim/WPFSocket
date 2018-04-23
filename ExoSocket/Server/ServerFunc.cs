using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Threading;
using System.Windows.Controls;
using System.Windows.Media;
using System.Windows.Controls.Primitives;

using System.Net;
using System.Net.Sockets;
using System.Threading;


namespace Server
{
    class ServerFunc
    {
        private TextBox IPAdressIPAdressTextBox;
        private TextBox PortTextBox;
        static Socket serverSocket;
        private static byte[] result = new byte[1024];

        public void Start(TextBox IPAdress, TextBox Port)
        {
            IPAdressIPAdressTextBox = IPAdress;
            PortTextBox = Port;

            IPAddress ip = IPAddress.Parse(IPAdressIPAdressTextBox.Text.ToString()); //服务器IP地址
            int myPort = int.Parse(PortTextBox.Text); // 端口号
            Socket serverSocket = new Socket(AddressFamily.InterNetwork, SocketType.Stream, ProtocolType.Tcp);
            serverSocket.Bind(new IPEndPoint(ip, myPort)); //绑定IP地址：端口
            serverSocket.Listen(10); //设定最多0个排队连接请求

            Thread myThread = new Thread(ListenClientConnect);
            myThread.Start();
            Console.ReadLine();
        }

        //监听客户端连接  
        private static void ListenClientConnect()
        {
            //while (true)
            //{
            //    Socket clientsocket = serverSocket.Accept();
            //    clientsocket.Send(Encoding.ASCII.GetBytes("server say hello"));
            //    Thread receiveThread = new Thread(ReceiveMessage);
            //    receiveThread.Start(clientsocket);
            //}
        }

        //接收消息
        private static void ReceiveMessage(object clientSocket)
        {
            Socket myClientSocket = (Socket)clientSocket;
            while (true)
            {
                try
                {
                    //通过clientsocket接收数据
                    int num = myClientSocket.Receive(result);
                    Console.WriteLine("接收客户端{0}消息{1}", myClientSocket.RemoteEndPoint.ToString(), Encoding.ASCII.GetString(result, 0, num));

                }
                catch (Exception ex)
                {
                    Console.WriteLine(ex.Message);
                    myClientSocket.Shutdown(SocketShutdown.Both);
                    myClientSocket.Close();
                    break;
                }
            }
        }

        public void Stop()
        {
            serverSocket.Close();
        }

    }
}
