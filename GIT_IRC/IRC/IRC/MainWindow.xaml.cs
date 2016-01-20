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


namespace IRC
{
    /// <summary>
    /// Interaction logic for MainWindow.xaml
    /// </summary>
    public partial class MainWindow : Window
    {
        public MainWindow()
        {
            InitializeComponent();
        }

        private void Button_Click(object sender, RoutedEventArgs e)
        {

            String nick;
            String serverAddr;
            String serverPort;
            nick = TextboxNick.Text.ToString();
            serverAddr = TextBoxAddr.Text.ToString();
            serverPort = TextBoxPort.Text.ToString();

            if (nick.Length > 0)
            {
                if(serverAddr.Length>7)
                {
                    if (serverPort.Length > 0)
                    {
                        User user = new User(nick);
                        MenuGlowne menu = new MenuGlowne(user, serverAddr,serverPort);
                        this.Close();
                        menu.ShowDialog();
                    } else { MessageBox.Show("Wpisz numer portu!"); }
                }else { MessageBox.Show("Wpisz poprawny adres IP!"); }
            }
            else { MessageBox.Show("Nie podano nicku!"); }
        }
    }
}
