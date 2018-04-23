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

namespace Server
{
    /// <summary>
    /// MainWindow.xaml 的交互逻辑
    /// </summary>
    public partial class MainWindow : Window
    {
        private ServerFunc server = new ServerFunc();

        public MainWindow()
        {
            InitializeComponent();
        }

        private void Window_Loaded(object sender, RoutedEventArgs e)
        {
            ControlWriter writer = new ControlWriter(ComWinTextBox); //将TextBox转化成Console

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

        private void Switch_Button_Click(object sender, RoutedEventArgs e)
        {
            server.Start(IPAdressTextBox, PortTextBox);
            Console.WriteLine("服务器 " + IPAdressTextBox.Text.ToString() + ":" + PortTextBox.Text + " 已开启");
            SwitchOff_Button.IsEnabled = true;
            Switch_Button.IsEnabled = false;         
        }

        private void ComWinTextBox_TextChanged(object sender, TextChangedEventArgs e)
        {
            ComWinTextBox.ScrollToEnd(); //当通信窗口内容变化时滚动条定位在最下面
        }

        private void SwitchOff_Button_Click(object sender, RoutedEventArgs e)
        {
            server.Stop();
            SwitchOff_Button.IsEnabled = false;
            Switch_Button.IsEnabled = true;
        }
    }
}
