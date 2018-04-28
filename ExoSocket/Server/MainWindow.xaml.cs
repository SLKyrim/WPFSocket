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
using System.Windows.Navigation;
using System.Windows.Shapes;
using System.Windows.Threading;
using System.Net.Sockets;
using System.Threading;
using System.Net;
using System.Runtime.InteropServices;

namespace Server
{
    /// <summary>
    /// MainWindow.xaml 的交互逻辑
    /// </summary>
    public partial class MainWindow : Window
    {
        public delegate void showData(string msg); //通信窗口输出相关
        private TcpListener server;
        private TcpClient client;
        private const int bufferSiize = 8000;

        [DllImport("C:\\Users\\Long\\Desktop\\ExoSocket\\Labview\\DLL\\SharedLib.dll", EntryPoint = "Add")]
        public extern static double Add(double x, double y);

        public MainWindow()
        {
            InitializeComponent();
        }

        private void Window_Loaded(object sender, RoutedEventArgs e)
        {
            DispatcherTimer dateTimer = new DispatcherTimer();//显示当前时间线程
            dateTimer.Tick += new EventHandler(dateTimer_Tick);
            dateTimer.Interval = new TimeSpan(0, 0, 0, 1, 0);
            dateTimer.Start();
        }

        private void dateTimer_Tick(object sender, EventArgs e)//取当前时间的委托
        {
            string timeDateString = "";
            DateTime now = DateTime.Now;
            timeDateString = string.Format("{0}年{1}月{2}日 {3}:{4}:{5}",
                now.Year,
                now.Month.ToString("00"),
                now.Day.ToString("00"),
                now.Hour.ToString("00"),
                now.Minute.ToString("00"),
                now.Second.ToString("00"));

            timeDateTextBlock.Text = timeDateString;
        }

        private void Window_Closing(object sender, System.ComponentModel.CancelEventArgs e)
        {
            server.Stop();
        }

        struct IpAndPort
        {
            public string Ip;
            public string Port;
        }

        private void Switch_Button_Click(object sender, RoutedEventArgs e) //启动服务器
        {
            Button bt = sender as Button;

            IpAndPort ipAndport = new IpAndPort();
            ipAndport.Ip = IPAdressTextBox.Text;
            ipAndport.Port = PortTextBox.Text;

            if (bt.Content.ToString() == "启动服务器")
            {
                if (IPAdressTextBox.Text.Trim() == string.Empty)
                {
                    ComWinTextBox.Dispatcher.Invoke(new showData(ComWinTextBox.AppendText), "请填入服务器IP地址\n");
                    return;
                }
                if (PortTextBox.Text.Trim() == string.Empty)
                {
                    ComWinTextBox.Dispatcher.Invoke(new showData(ComWinTextBox.AppendText), "请填入服务器端口号\n");
                    return;
                }

                // 测试labview简单加法
                //double z = Add(21.0, 3.0);
                //ComWinTextBox.Dispatcher.Invoke(new showData(ComWinTextBox.AppendText), "Labview加法器传来结果：" + z.ToString() + "\n");

                Thread thread = new Thread(reciveAndListener);            
                thread.Start((object)ipAndport);

                ComWinTextBox.Dispatcher.Invoke(new showData(ComWinTextBox.AppendText), "服务器 " + ipAndport.Ip + " : " + ipAndport.Port + " 已开启监听\n");
                statusInfoTextBlock.Text = "服务器已启动";
                bt.Content = "关闭服务器";
            }

            else
            {
                server.Stop();
                ComWinTextBox.Dispatcher.Invoke(new showData(ComWinTextBox.AppendText), "服务器 " + ipAndport.Ip + " : " + ipAndport.Port + " 已关闭\n");
                statusInfoTextBlock.Text = "服务器已关闭";
                bt.Content = "启动服务器";
            }

        }

        private void reciveAndListener(object ipAndPort)
        {
            IpAndPort ipAndport = (IpAndPort)ipAndPort;

            IPAddress ip = IPAddress.Parse(ipAndport.Ip);
            server = new TcpListener(ip, int.Parse(ipAndport.Port));
            server.Start();//启动监听

            client = server.AcceptTcpClient();
            ComWinTextBox.Dispatcher.Invoke(new showData(ComWinTextBox.AppendText), "有客户端请求连接，连接已建立\n");
            //AcceptTcpClient 是同步方法，会阻塞进程，得到连接对象后才会执行这一步 

            //获得流
            NetworkStream reciveStream = client.GetStream();
            #region
            do
            {
                byte[] buffer = new byte[bufferSiize];
                int msgSize;
                try
                {
                    lock (reciveStream)
                    {
                        msgSize = reciveStream.Read(buffer, 0, bufferSiize);
                    }
                    if (msgSize == 0)
                        return;
                    string msg = Encoding.Default.GetString(buffer, 0, bufferSiize);
                    ComWinTextBox.Dispatcher.Invoke(new showData(ComWinTextBox.AppendText), "从客户端发来信息：" + Encoding.Default.GetString(buffer, 0, msgSize) + "\n");
                }
                catch
                {
                    ComWinTextBox.Dispatcher.Invoke(new showData(ComWinTextBox.AppendText), "出现异常：连接被迫关闭\n");
                    break;
                }
            } while (true);
            #endregion
        }


        private void Send_Button_Click(object sender, RoutedEventArgs e)
        {
            if (MessageTextBox.Text.Trim() != string.Empty)
            {
                NetworkStream sendStream = client.GetStream();
                byte[] buffer = Encoding.Default.GetBytes(MessageTextBox.Text.Trim());
                sendStream.Write(buffer, 0, buffer.Length);
                MessageTextBox.Text = string.Empty;
            }
        }

        private void ComWinTextBox_TextChanged(object sender, TextChangedEventArgs e)
        {
            ComWinTextBox.ScrollToEnd(); //当通信窗口内容有变化时保持滚动条在最下面
        }
    }
}
