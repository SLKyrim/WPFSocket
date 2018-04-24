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
using System.Net;
using System.Net.Sockets;
using System.Threading;

namespace Client
{
    /// <summary>
    /// MainWindow.xaml 的交互逻辑
    /// </summary>
    public partial class MainWindow : Window
    {
        public delegate void showData(string msg); //通信窗口输出相关
        TcpClient client;
        NetworkStream sendStream;
        private const int bufferSize = 8000;

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
            Environment.Exit(0);
        }

        private void Switch_Button_Click(object sender, RoutedEventArgs e) //请求连接
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

            IPAddress ip = IPAddress.Parse(IPAdressTextBox.Text);
            client = new TcpClient();
            ComWinTextBox.AppendText("开始连接服务端....\n");
            client.Connect(ip, int.Parse(PortTextBox.Text));
            ComWinTextBox.AppendText("已经连接服务端\n");
            statusInfoTextBlock.Text = "已与服务器建立连接";

            sendStream = client.GetStream();
            Thread thread = new Thread(ListenerServer);
            thread.Start();
        }

        private void ListenerServer()
        {
            do
            {
                try
                {
                    int readSize;
                    byte[] buffer = new byte[bufferSize];
                    lock (sendStream)
                    {
                        readSize = sendStream.Read(buffer, 0, bufferSize);
                    }
                    if (readSize == 0)
                        return;
                    ComWinTextBox.Dispatcher.Invoke(new showData(ComWinTextBox.AppendText), "从服务端发来信息：" + Encoding.Default.GetString(buffer, 0, readSize) + "\n");

                }
                catch
                {
                    ComWinTextBox.Dispatcher.Invoke(new showData(ComWinTextBox.AppendText), "报错\n");
                }

            } while (true);
        }

        private void Send_Button_Click(object sender, RoutedEventArgs e)
        {
            if (client != null)
            {
                //要发送的信息
                if (MessageTextBox.Text.Trim() == string.Empty)
                    return;
                string msg = MessageTextBox.Text.Trim();

                byte[] buffer = Encoding.Default.GetBytes(msg);
                //lock (sendStream)
                //{
                sendStream.Write(buffer, 0, buffer.Length);
                //}
                ComWinTextBox.AppendText("发送给服务端的数据：" + msg + "\n");
                MessageTextBox.Text = string.Empty;
            }
        }

        private void ComWinTextBox_TextChanged(object sender, TextChangedEventArgs e)
        {
            ComWinTextBox.ScrollToEnd(); //当通信窗口内容有变化时保持滚动条在最下面
        }
    }
}
