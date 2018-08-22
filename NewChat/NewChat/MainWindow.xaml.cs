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
using System.IO;
using System.Runtime.Serialization;
using System.Runtime.Serialization.Formatters.Binary;

namespace NewChat
{
    [Serializable]
    class MessageInfo
    {
        public readonly string Message;
        public readonly string UserName;
        public string Color;
        

        public MessageInfo(string userName, string message, string colorValue)
        {
            UserName = userName;
            Message = message;
            Color = colorValue;
            
        }
    }

    /// <summary>
    /// Interaction logic for MainWindow.xaml
    /// </summary>
    public partial class MainWindow : Window
    {
        UdpClient _sender, _reciever;
        IPEndPoint _localEP, _remoteEP;
        Thread _recieverThread;
        

        
        private void Send()
        {
            byte[] buffer;

            var messageInfo = new MessageInfo(UserNameTextBox.Text, MessageTextBox.Text, ColorComboBox.SelectedValue.ToString());
            using (var stream = new MemoryStream())
            {
                var formatter = new BinaryFormatter();
                formatter.Serialize(stream, messageInfo);
                buffer = stream.ToArray();
            }

          
            _sender.Send(buffer, buffer.Length, _remoteEP);
            MessageTextBox.Clear();
        }

        private void Button_Click(object sender, RoutedEventArgs e)
        {
            Send();
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

        private void ColorComboBox_SelectionChanged(object sender, SelectionChangedEventArgs e)
        {

        }

        private void Window_PreviewKeyDown(object sender, KeyEventArgs e)
        {
            switch (e.Key)
            {
                case Key.Enter:
                    Send();
                    break;
            }
        }

        private void RecieverTask()
        {
            while (true)
            {
                IPEndPoint recivers = new IPEndPoint(IPAddress.Any, 0);
                var bytes = _reciever.Receive(ref recivers);
                MessageInfo messageInfo = null;
                //var message = Encoding.Default.GetString(bytes);
                using (var stream = new MemoryStream(bytes))
                {
                    var formatter = new BinaryFormatter();
                    messageInfo = (MessageInfo)formatter.Deserialize(stream);
                }

                Dispatcher.Invoke(() =>
                {
                    var paragraph = new Paragraph();
                    paragraph.Inlines.Add(new Run("[" + DateTime.Now.ToString("HH:mm:ss") + "]\n"));                    
                    paragraph.Inlines.Add(new Run(messageInfo.UserName));
                    paragraph.Inlines.Add(new Run(" >> "));
                    paragraph.Inlines.Add(new Run(messageInfo.Message)
                    {
                        Foreground = GetBrush(messageInfo.Color)
                    });
                    ChatDocument.Blocks.Add(paragraph);
                    
                });

            }
        }

        private Brush GetBrush(string color)
        {
            switch (color)
            {
                case "Black":
                    return Brushes.Black;
                case "Red":
                    return Brushes.Red;
                case "Blue":
                    return Brushes.Blue;
                case "Green":
                    return Brushes.Green;
                default:
                    return Brushes.Black;
            }
        }
    }
}
