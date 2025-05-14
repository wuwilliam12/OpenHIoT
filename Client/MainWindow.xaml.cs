using OpenHIoT.Client;
using OpenHIoT.Client.Pages.Device;
using OpenHIoT.Client.Pages.Channel;
using OpenHIoT.Client.Pages.Channel.DataList;
using OpenHIoT.Client.Requests;
using System.Net.Http;
using System.Text;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;
using System.Windows.Documents;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using System.Windows.Navigation;
using System.Windows.Shapes;

namespace OpenHIoT.Client
{

    /// <summary>
    /// Interaction logic for MainWindow.xaml
    /// </summary>
    public partial class MainWindow : Window
    {
        public static MainWindow mainWindow; 
  //      static IReamTimeCntl? rtCntl;
        static private CancellationTokenSource cts = new CancellationTokenSource();
        //       System.Timers.Timer refreshTimer;
        public static CancellationTokenSource GetCancellationTokenSource()
        {
            return cts;
        }

        /*
        public static void SetRealtimeCntl(IReamTimeCntl? _rtCntl)
        {
            if (rtCntl != _rtCntl)
            {
                cts.Cancel();
                cts = new CancellationTokenSource();
                rtCntl = _rtCntl;
            }
        }
        */

        DeviceMainPage devMainPage = new DeviceMainPage();
        DisplayMainPage displayMainPage = new DisplayMainPage();
        public  DeviceMainPage  DeviceMainPage { get { return devMainPage; } }
        public DisplayMainPage DisplayMainPage { get { return displayMainPage; } }


        public MainWindow()
        {
            mainWindow = this;
            ServerRequest.CreateHttpClient();
         //   ClientEdge.Start();
            InitializeComponent();
            SysLogCntl.SysLog = sysLog;
            //           refreshTimer = new System.Timers.Timer();
            //           refreshTimer.Elapsed += RefreshTimer_Elapsed;
            //           refreshTimer.Start();
        }
        /*
                private async void RefreshTimer_Elapsed(object? sender, System.Timers.ElapsedEventArgs e)
                {
                    if(rtCntl != null)
                        rtCntl.UpdateItems();
                }
        */
        private void devicesBtn_Click(object sender, RoutedEventArgs e)
        {
           // SetRealtimeCntl(null);
            mainFrame.Navigate(devMainPage);
        }

        private void displayBtn_Click(object sender, RoutedEventArgs e)
        {
            mainFrame.Navigate(displayMainPage);
        }
    }
}