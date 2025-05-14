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

using System.Net;
using System.Threading;
using UserControl = System.Windows.Controls.UserControl;



namespace OpenHIoT.Client.Pages.User
{
    /// <summary>
    /// Interaction logic for ConnectionCntl.xaml
    /// </summary>
    public partial class ConnectionCntl : UserControl
    {
        //public static readonly RoutedCommand ConnectCmd = new RoutedCommand();
  //      public int Port { get; set; }
  //      public string  IP { get; set; }



        public ConnectionCntl( )
        {          
            Connections conns = Client.client.StartupInfor.Connections;
            if (conns.Count == 0)
            {
                Connection c = new Connection();
                c.Port = 0x6543;
                c.IP = "127.0.0.1";
                c.Name = "Local";
                conns.Add(c);
                c = new Connection();
                c.Port = 0x6543;
                c.IP = "192.169.2.199";
                c.Name = "Cable";

                conns.Add(c);
                c = new Connection();
                c.Port = 0x6543;
                c.IP = "10.178.1.138";
                c.Name = "Wifi";
                conns.Add(c);
                conns.Selected = 0;
            }


       //     Client.client.StartupInfor.Servers.Clear();
            InitializeComponent();
            if (conns.Selected < 0)
                conns.Selected = 0;
            connList.ItemsSource = conns;
            connList.SelectedItem = conns[conns.Selected];

        }

        public void LoopConnectios()
        {
            
        }


        public static void StartConnect()
        {
            TcpPortC dp = Client.client.DataPort;
            dp.KeyRxed = false;
            int k = 0;
            Connections conns = Client.client.StartupInfor.Connections;
            Connection c = conns[conns.Selected];
            IPAddress ip = IPAddress.Parse(c.IP);
            dp.Connect(ip, c.Port);
            Thread.Sleep(500);

            if (dp.Connected)
                dp.StartRx();
        }

        private void Connect_Click(object sender, RoutedEventArgs e)
        {
            Connection c = (Connection)DataContext;
            TcpPortC dp = Client.client.DataPort;
            dp.KeyRxed = false;
            IPAddress ip;
            if(IPAddress.TryParse(c.IP, out ip))
            {
                if (dp.Connect(ip, c.Port))
                    dp.StartRx();                
            }
        }

        private void ConnList_SelectionChanged(object sender, SelectionChangedEventArgs e)
        {
            DataContext = connList.SelectedItem;
            Client.client.StartupInfor.Connections.Selected = connList.SelectedIndex;
        }
    }
}
