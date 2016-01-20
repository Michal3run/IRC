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
using System.Windows.Shapes;
using System.Collections.ObjectModel;
using System.Net;
using System.Net.Sockets;
using System.Threading;

namespace IRC
{
    /// <summary>
    /// Interaction logic for MenuGlowne.xaml
    /// </summary>
    public partial class MenuGlowne : Window
    {
        private Window thisWindow;
        public User user;
        String serverAddrString;
        IPAddress serverAddr;
        String serverPort;
        public Room ActualRoom; //room, którego chat jest aktualnie wyświetlany na panelu
        public ObservableCollection<Room> Rooms = new ObservableCollection<Room>(); //kolekcja pokoi (kanałów)
        Room Main = new Room("Main Room", 200);
        public Socket socketFd;
        public SocketStateObject state;
        public bool isConnected = false;
        public bool receiving = false;
        
        public List<Room> UserInsideRooms = new List<Room>(); //lista pokoi w których jest użytkownik (i mają dochodzić do niego wiadomości od innych)

        public MenuGlowne(User userfrommain, String serverAddrString, String serverPort)
        {
            InitializeComponent();
            this.thisWindow = this;
            this.user = userfrommain;
            this.serverAddrString = serverAddrString;
            this.serverPort = serverPort;
            serverAddr = IPAddress.Parse(serverAddrString);
                      
            Channels.Dispatcher.BeginInvoke(new Action(delegate()
            {
                Channels.ItemsSource = null;
                Channels.ItemsSource = Rooms;
            }));
            ActualRoom = Main;
            ActualRoom.IsInside = IRC.Room.inside.TAK;
            Rooms.Add(Main);
            setThreadedRoomName(Main.Name);
            UserInsideRooms.Add(Main);
            setThreadedChat(); //ustawiay chatbox na chat danego roomu

            //jeżeli klikniemy X (bez przycisku "wyjdź z serwera"
            AppDomain.CurrentDomain.ProcessExit += new EventHandler(OnProcessExit);

            ConnectwithIP();
       }

       
        private void Channels_PreviewMouseDown(object sender, MouseButtonEventArgs e) //klikanie na listę kanałów zmienia chat
        {
            var item = ItemsControl.ContainerFromElement(Channels, e.OriginalSource as DependencyObject) as ListBoxItem;
            if (item != null)
            {

                ActualRoom = (Room)item.Content;
                UserInsideRooms.Add(ActualRoom);
                ActualRoom.IsInside = IRC.Room.inside.TAK;
                Channels.Dispatcher.BeginInvoke(new Action(delegate()
                {
                    Channels.ItemsSource = null;
                    Channels.ItemsSource = Rooms;
                }));
                setThreadedRoomName(ActualRoom.Name);
                setThreadedChat();
                Chat.ScrollToEnd();
                ActualRoom.NewMessages = IRC.Room.inside.NIE;
                
                
            }
        }

        public void Message_GotFocus(object sender, RoutedEventArgs e) //jeśli najedzie się i kliknie myszką na text box to tekst na nim ma się zamienić na pusty string
        {
            TextBox MessageFocuses = (TextBox)sender;
            setThreadedMessage(String.Empty);
            MessageFocuses.GotFocus -= Message_GotFocus;
        }

        private void Wyslij_Click(object sender, RoutedEventArgs e)
        {
            String MessageText="";
            
            getMessageText(ref MessageText);
            if (MessageText.Length > 0)
            {
                SendDate(state,"<"+MessageText+">"+"<"+ActualRoom.Name+">","<new_message>");
                setThreadedChat();
               
                //VisibleRoom.RoomPanel.ChatText = VisibleRoom.Name;
            }
            else { MessageBox.Show("Nie możesz wysłać pustej wiadomości!"); }
        }

        private void ExitServer_Click(object sender, RoutedEventArgs e)
        {
            string sMessageBoxText = "Czy na pewno chcesz wyjść z serwera?";
            string sCaption = "Zastanów się dwa razy...";

            MessageBoxButton btnMessageBox = MessageBoxButton.YesNo;
            MessageBoxImage icnMessageBox = MessageBoxImage.Warning;

            MessageBoxResult rsltMessageBox = MessageBox.Show(sMessageBoxText, sCaption, btnMessageBox, icnMessageBox);

            switch (rsltMessageBox)
            {
                case MessageBoxResult.Yes:

                    SendDate(state, user.Nick, "<delete_user>");
                    MainWindow Restart = new MainWindow();
                    this.Close();
                    
                    Restart.ShowDialog();
                    break;
                case MessageBoxResult.No:

                    break;

                case MessageBoxResult.Cancel:

                    break;

            }

        }

        private void ExitRoom_Click(object sender, RoutedEventArgs e)
        {
            UserInsideRooms.Remove(ActualRoom);
            ActualRoom.IsInside = IRC.Room.inside.NIE;
            ActualRoom.ChatText = "";

           
            Channels.ItemsSource = null;
            Channels.ItemsSource = Rooms;
            ActualRoom = Main;
            setThreadedRoomName(ActualRoom.Name);
            ActualRoom.NewMessages = IRC.Room.inside.NIE;//nie ma nowcyh wiadomości, bo je odczytaliśmy
            setThreadedChat();
            Chat.ScrollToEnd();

        }
      /*  
        private void ExitServerFunction(Socket socketFd)
        {
      */      /* zamknięcie gniazda */
    /*        isConnected = false;
            socketFd.Shutdown(SocketShutdown.Both);
            socketFd.Close();
        }
    */
        void OnProcessExit (object sender, EventArgs e) //jeżeli wyjdziemy za pomocą X
        {
            SendDate(state, user.Nick, "<delete_user>");
            /* zamknięcie gniazda */
            if (isConnected == true)
            {
                socketFd.Shutdown(SocketShutdown.Both);
                socketFd.Close();
            }
        }

        public void Button_Click(object sender, RoutedEventArgs e)
        {
            AddRoomWindow RoomWindow = new AddRoomWindow(ref Rooms,this); //w parametrze przekazujemy zbiór Pokoi

            RoomWindow.ShowDialog();
        }

        //część sieciowa
        //------------------------------------------------------------------------------------

        delegate void setThreadedTextBoxCallback(String text);

        private void setThreadedTextBox(String text)
        {
            if (!ServerInfo.Dispatcher.CheckAccess())
            {
                setThreadedTextBoxCallback textBoxCallback = new
                                           setThreadedTextBoxCallback(setThreadedTextBox);
                thisWindow.Dispatcher.Invoke(textBoxCallback, text);
            }
            else
            {
                ServerInfo.Text = text;
            }
        }

        delegate void setThreadedChatCallback();//ustawianie na textboxie tekstu z aktualnego roomu

        private void setThreadedChat()
        {
            if (!Chat.Dispatcher.CheckAccess())
            {
                setThreadedChatCallback ChatCallback = new
                                           setThreadedChatCallback(setThreadedChat);
                thisWindow.Dispatcher.Invoke(ChatCallback);
            }
            else
            {
                Chat.Text = ActualRoom.ChatText;
                
            }
        }
        

        delegate void dodajChatCallback(String text); //dopisywanie wiadomości do czatu

        private void dodajChat(String text)
        {
            if (!Chat.Dispatcher.CheckAccess())
            {
                dodajChatCallback ChatCallback = new
                                           dodajChatCallback(dodajChat);
                thisWindow.Dispatcher.Invoke(ChatCallback, text);
            }
            else
            {
                ActualRoom.ChatText += ("[" + DateTime.Now.ToString("h:mm:ss tt") + "]" + " <" + user.Nick + "> " + text);
                ActualRoom.ChatText += Environment.NewLine;
                Chat.ScrollToEnd();
                

            }
        }

        delegate void setThreadedRoomNameCallback(String text);

        private void setThreadedRoomName(String text)
        {
            if (!RoomNameBlock.Dispatcher.CheckAccess())
            {
                setThreadedRoomNameCallback RoomNameCallback = new
                                           setThreadedRoomNameCallback(setThreadedRoomName);
                thisWindow.Dispatcher.Invoke(RoomNameCallback, text);
            }
            else
            {
                RoomNameBlock.Text = text;
            }
        }

        delegate void setThreadedMessageCallback(String text); 

        private void setThreadedMessage(String text)
        {
            if (!Message.Dispatcher.CheckAccess())
            {
                setThreadedMessageCallback MessageCallback = new
                                           setThreadedMessageCallback(setThreadedMessage);
                thisWindow.Dispatcher.Invoke(MessageCallback, text);
            }
            else
            {
               Message.Text = text;
            }
        }

        delegate void getMessageCallback(ref String MessageText);

        private void getMessageText(ref String MessageText)
        {
            if (!Message.Dispatcher.CheckAccess())
            {
                getMessageCallback getMessageCallback = new
                                           getMessageCallback(getMessageText);
                thisWindow.Dispatcher.Invoke(getMessageCallback);
            }
            else
            {
                MessageText = Message.Text;
            }
        }

        private void ReceiveCallback(IAsyncResult ar)
        {
            
            try
            {
                /* retrieve the SocketStateObject */
                state = (SocketStateObject)ar.AsyncState;
                Socket socketFd = state.m_SocketFd;
                
                /* read data */
                int size = socketFd.EndReceive(ar);
                //Console.Out.WriteLine("Read data");
                
                
                if (size > 0)
                {
                    state.m_StringBuilder.Append(Encoding.ASCII.GetString(state.m_DataBuf, 0, size));
                    //reszta danych
                    
                    if (!(state.m_StringBuilder.ToString().IndexOf("<end>") > 0))
                    {
                        socketFd.BeginReceive(state.m_DataBuf, 0, state.m_DataBuf.Length, 0,
                                                 new AsyncCallback(ReceiveCallback), state);
                    }    
              }
                                 
                if(state.m_StringBuilder.ToString().IndexOf("<end>") > 0)
                {  
                    if (state.m_StringBuilder.Length > 1)
                    {
                        Console.Out.WriteLine(state.m_StringBuilder.ToString());
                        /* all the data has arrived */
                        if (state.m_StringBuilder.ToString().IndexOf("<new_message>") == 0)//znalazł wzorzec
                        {
                            
                            String tochat = state.m_StringBuilder.ToString().Remove(0, 14);
                            tochat = tochat.Remove(tochat.Length - 5, 5);
                            String roomname = tochat.Substring(tochat.IndexOf("<")+1,tochat.Length-tochat.IndexOf("<")-2);
                            tochat=tochat.Substring(0,tochat.IndexOf("<")-1);
                            
                            Boolean insideRoom = false;

                            foreach (Room room in UserInsideRooms)
                            {
                                if(room.Name==roomname)
                                {
                                    insideRoom = true;
                                    break;
                                }
                            }

                            if (insideRoom == true)
                            {

                                dodajChat(tochat);
                                setThreadedChat();
                                //Console.Out.WriteLine(ActualRoom.ChatText);
                                if (roomname != ActualRoom.Name)
                                //dodajemy powiadomienie że coś nowego
                                {
                                    
                                    foreach (Room room in Rooms)
                                    {
                                        if (roomname == room.Name)
                                        {
                                            dodajChat(tochat);
                                            room.NewMessages = IRC.Room.inside.TAK;//nowe wiadomośći w tym pokoju 
                                            break;
                                        }
                                    }
                                }
                            }

                        }
                        else if (state.m_StringBuilder.ToString().IndexOf("<room_list>") == 0)
                        {
                            String roomlist = state.m_StringBuilder.ToString().Remove(0, 11);
                            int x = 0;
                            String roomname = String.Empty;
                            int roomOccupancy = 0;
                            bool koniec = false;
                            foreach (string s in roomlist.Split('>'))
                            {
                                //Console.Out.WriteLine(s);
                                if (s == "<end")
                                { koniec = true; }
                                if (koniec == false)
                                {
                                    String s1 = s.Remove(0, 1);

                                    if (x % 2 == 0)
                                    { roomname = s1; }
                                    else { roomOccupancy = Int32.Parse(s1); }

                                    if (x % 2 != 0)
                                    {
                                        Room NewRoom = new Room(roomname, roomOccupancy);
                                        Channels.Dispatcher.BeginInvoke(new Action(delegate()
                                        {
                                            Rooms.Add(NewRoom);
                                            Channels.ItemsSource = null;
                                            Channels.ItemsSource = Rooms;
                                        }));
                                    }
                                    x++;
                                }
                            }
                       }
                        else if (state.m_StringBuilder.ToString().IndexOf("<new_room>") == 0)
                        {
                            Console.Out.WriteLine(state.m_StringBuilder.ToString());
                            String roomlist = state.m_StringBuilder.ToString().Remove(0, 10);
                            int x = 0;
                            String roomname = String.Empty;
                            int roomOccupancy = 0;
                            bool koniec = false;
                            foreach (string s in roomlist.Split('>'))
                            {
                                Console.Out.WriteLine(s);
                                if (s == "<end")
                                { koniec = true; }
                                if (koniec == false)
                                {
                                    String s1 = s.Remove(0, 1);

                                    if (x % 2 == 0)
                                    { roomname = s1; }
                                    else { roomOccupancy = Int32.Parse(s1); }
                                }
                                x++;
                            }
                            bool juzistnieje=false;

                            foreach(Room room in Rooms)
                            {
                                if(room.Name==roomname)
                                { juzistnieje = true; }
                            }

                            if(juzistnieje==false)
                            {
                            Room NewRoom = new Room(roomname, roomOccupancy);
                            
                            Channels.Dispatcher.BeginInvoke(new Action(delegate ()
                            {
                                Rooms.Add(NewRoom);
                                Channels.ItemsSource = null;
                                Channels.ItemsSource = Rooms;
                            }));
                            
                            Console.Out.WriteLine(NewRoom.Name);
                            }
                        }
                    }

                    state.m_StringBuilder.Clear(); //po odczytaniu czyścimy
                    
                    Console.Out.WriteLine("Czytam kolejną wiadomość");
                 
                    ReadData();
                    
                }
          }
            catch (Exception exc)
            {
                MessageBox.Show("Exception Receive Callback  :\t\n" + exc.Message.ToString());
                setThreadedTextBox("Wyjdź z serwera i spróbuj ponownie!");
            }
          }
        private void ReadData()
        {
            
            Array.Clear(state.m_DataBuf, 0, state.m_DataBuf.Length);
            state.m_SocketFd.BeginReceive(state.m_DataBuf, 0, state.m_DataBuf.Length, 0,
                                    new AsyncCallback(ReceiveCallback), state);
            //Array.Clear(state.m_DataBuf, 0, state.m_DataBuf.Length);
            
        }
        
        private void ConnectCallback(IAsyncResult ar)
        {            
            try
            {
                /* retrieve the socket from the state object */
                socketFd = (Socket)ar.AsyncState;
                Console.Out.WriteLine("ConnectCallback");
                /* complete the connection */
                socketFd.EndConnect(ar);
                state=new SocketStateObject();
                
                state.m_SocketFd = socketFd;

                setThreadedTextBox("Czekaj, odbieranie danych z serwera!");
                Console.Out.WriteLine("Odbieranie danych");
                /* begin receiving the data */
                
                if(isConnected==false)
                {//SendDate(state, user.Nick, "<new_user>");
                isConnected = true;
                }
               ReadData();                
            }
            catch (Exception exc)
            {
                MessageBox.Show("Exception: ConnectCallback  \t\n" + exc.Message.ToString());
                setThreadedTextBox("Wyjdź z serwera i spróbuj ponownie!");
            }
        }
                        
        public static void SendCallback(IAsyncResult ar)
        {
            try
            {
                /* retrieve the socket from the ar object */
                Socket socketFd = (Socket)ar.AsyncState;

                /* end pending asynchronous send */
                int bytesSent = socketFd.EndSend(ar);

            }
            catch (Exception exc)
            {
                Console.WriteLine(exc.Message.ToString());
            }
        }
        
        public void SendDate(SocketStateObject state,String text,String co)
        {
            try
            {
                //this.state = state;
                //state.m_DataBuf = Encoding.ASCII.GetBytes(String.Empty);
                /* wstaw stringa do bufora */
               
         //       state.m_DataBuf = Encoding.ASCII.GetBytes(co + text + "<end>");
                
                //Console.Out.WriteLine(Encoding.ASCII.GetString(state.m_DataBuf, 0, state.m_DataBuf.Length));
                setThreadedTextBox("Wszystko OK!");
                /* begin sending the date */
             //   state.m_SocketFd.BeginSend(state.m_DataBuf, 0, state.m_DataBuf.Length, 0, 
             //                      new AsyncCallback(SendCallback), state.m_SocketFd);
                //state.m_DataBuf = Encoding.ASCII.GetBytes(String.Empty);
           //     setThreadedMessage(String.Empty);
            }
            catch (Exception exc)
            {
                Console.WriteLine("Send Data exception, "+exc.Message.ToString());
            }
        }
                
        private void ConnectwithIP()
        {
            Console.Out.WriteLine("Connecting");
            try
            {
                IPEndPoint endPoint = null;

                Console.Out.WriteLine("Create socket");
                Socket socketFd = new Socket(AddressFamily.InterNetwork, SocketType.Stream, ProtocolType.Tcp);

                /* remote endpoint for the socket */
                endPoint = new IPEndPoint(serverAddr, Int32.Parse(serverPort));

                setThreadedTextBox("Czekaj, łączenie!");

                /* connect to the server */
                Console.Out.WriteLine("Create socket2");
                socketFd.BeginConnect(endPoint, new AsyncCallback(ConnectCallback), socketFd);
                Console.Out.WriteLine("Create socket3");
            }
            catch (Exception exc)
            {
                MessageBox.Show("Exception ConnectIP:\t\n" + exc.Message.ToString());
                setThreadedTextBox("Wyjdź i spróbuj ponownie!");

            }
        }
 
        
        public class SocketStateObject
        {
            public const int BUF_SIZE = 1024;
            public byte[] m_DataBuf = new byte[BUF_SIZE];
            public StringBuilder m_StringBuilder = new StringBuilder();
            public Socket m_SocketFd = null;
        }

    }


        
}
