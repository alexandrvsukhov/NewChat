using System;
using System.Collections.Generic;
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
using System.Windows.Navigation;
using System.Windows.Shapes;
using System.Threading;

namespace NewChat
{
    /// <summary>
    /// Interaction logic for MainWindow.xaml
    /// </summary>
    public partial class MainWindow : Window
    {
        UdpClient _sender, _reciever;
        IPEndPoint _localEP, _remoteEP;
        Thread _recieverThread;

        private delegate void Del(string message);

        private void Button_Click(object sender, RoutedEventArgs e)
        {
            MessageTextBox.WriteLine("Name");
            var bytes = Encoding.Default.GetBytes(MessageTextBox.Text);
            _sender.Send(bytes, bytes.Length, _remoteEP);
            MessageTextBox.WriteLine(DateTime.Now);
            Del Send = new Del(Button_Click);
        }

        const int port = 1231;
        public MainWindow()
        {
            InitializeComponent();
            _remoteEP = new IPEndPoint(IPAddress.Parse("192.168.26.255"), port);
            _localEP = new IPEndPoint(IPAddress.Any, port);
            _sender = new UdpClient();
            _reciever = new UdpClient(_localEP);
            _recieverThread = new Thread(RecieverTask) { IsBackground = true };
            _recieverThread.Start();
        }

        private void RecieverTask()
        {
            while (true)
            {
                IPEndPoint recivers = new IPEndPoint(IPAddress.Any, 0);
                var bytes = _reciever.Receive(ref recivers);
                var message = Encoding.Default.GetString(bytes);
                Dispatcher.Invoke(() => ChatTextBox.Text += message + Environment.NewLine);
            }
        }
        
        private void Send(object sender, KeyEventArgs e)
        {
            switch (e.Key)
            {
                case Key.Enter:
                    Send();
                    break;
            }        
        }
    
        
 
    
}
