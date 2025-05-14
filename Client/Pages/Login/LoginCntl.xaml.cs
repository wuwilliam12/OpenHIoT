using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading;
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



namespace OpenHIoT.Client.Pages.User
{
    /// <summary>
    /// Interaction logic for UserControl1.xaml
    /// </summary>
    public partial class LoginCntl : System.Windows.Controls.UserControl
    {

        StartupInfor sInfor;

        public LoginCntl()
        {

            InitializeComponent();
            sInfor = Client.client.StartupInfor;
            DataContext = sInfor;
        //    pwTb.Text = sInfor.Password;
            pwBox.Password = sInfor.GetPassword();
            wenoPwBox.Password = sInfor.GetWenoPassword();
            //  sInfor = new StartupInfor();
        }

        public void Login()
        {
            string s = (bool)showPwCb.IsChecked? pwTb.Text : pwBox.Password;
            string s1 = (bool)wenoShowPwCb.IsChecked? wenoPwTb.Text : wenoPwBox.Password;

            sInfor.SetPassword(s, s1);
            SetupProt.SendLoginRequest(sInfor.UserName, sInfor.GetPassword());

            int k = 0;
            while (Client.client.State < ClientState.Welcome && k < 10)
            {
                Thread.Sleep(100);
                k++;
            }
        }

        void Summit_Click(object sender, RoutedEventArgs e)
        {
            Login();
            if (sInfor.RememberUser)
                sInfor.Save();
        }

        void Disconnect_Click(object sender, RoutedEventArgs e)
        {
            Client.client.DataPort.DisConnect();
            Client.client.State = ClientState.Connect;
        }

        private void ShowPassword_Clicked(object sender, RoutedEventArgs e)
        {
            if((bool)showPwCb.IsChecked)
            {
                pwTb.Text = pwBox.Password;
                pwBox.Visibility = Visibility.Hidden;
                pwTb.Visibility = Visibility.Visible;
            }
            else
            {
                pwBox.Password = pwTb.Text;
                pwBox.Visibility = Visibility.Visible;
                pwTb.Visibility = Visibility.Hidden;
            }
        }
        private void WenoShowPassword_Clicked(object sender, RoutedEventArgs e)
        {
            if((bool)wenoShowPwCb.IsChecked)
            {
                wenoPwTb.Text = wenoPwBox.Password;
                wenoPwBox.Visibility = Visibility.Hidden;
                wenoPwTb.Visibility = Visibility.Visible;
            }
            else
            {
                wenoPwBox.Password = wenoPwTb.Text;
                wenoPwBox.Visibility = Visibility.Visible;
                wenoPwTb.Visibility = Visibility.Hidden;
            }
        }
    }
}
