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
namespace IRC
{   
    // PLAN na serwer:
    // serwer ma tylko tablicę użytkowników, i tak wysyła do wszystkich
    // otrzymuje wiadomości: 
    // a) "<new_user>,nick" to dodaje userów (to będzie się samo wysyłało przy tworzeniu menu głównego w aplikacji klienta)
    // b) "<message>room, actualOccupancy, TREŚC WIADOMOSCI (z nickiem), godzina_na_serwerze,"> to wiadomość, serwer odsyła do WSZYSTKICH userów 
    // c) "<message>,<room>,TREŚC WIADOMOSCI (z nickiem)" to wiadomość, którą klient wysyła do serwera 
    // d) "<delete_user>,nick" usuwa z tablicy usera, zamyka gniazdo
    // e) "<new_room>,czy_liczba_osób_2_czy_3_cyfrowa,liczba_osób" tworzy pokój
    // przecinki dodane dla czytelności, w wysyłanej wiadomości nie ma ich, każda wysłana wiadomość kończy się znacznikiem <end>
    public partial class AddRoomWindow : Window
    {
        public ObservableCollection<Room> Rooms = new ObservableCollection<Room>();
        MenuGlowne menu;
        
        
        public AddRoomWindow(ref ObservableCollection<Room> Rooms,MenuGlowne menu)
        {
            this.Rooms=Rooms;
            this.menu = menu;
            InitializeComponent();
        }

        private void CreateRoom_Click(object sender, RoutedEventArgs e)
        {
          
            if(RoomName.Text.Length>0)
            {
                if (RoomName.Text.Length > 16)
                {
                    MessageBox.Show("Za długa nazwa pokoju! (max 16 znaków)");
                }
                else
                {
                    Boolean IsNameTaken = false;
                    foreach (Room CheckingRoom in Rooms)
                    {
                        if (CheckingRoom.Name == RoomName.Text)
                        {
                            IsNameTaken = true;
                        }
                    }

                    if (IsNameTaken == false)
                    {

                        menu.SendDate(menu.state,"<"+RoomName.Text + "><" + (int)OccupancyInteger.Value + ">", "<new_room>");
                        this.Close();
                    }
                    else
                    {
                        MessageBox.Show("Ta nazwa pokoju jest zajęta!");
                    }
                }
            }else{
                MessageBox.Show("Wpisz nazwę pokoju!");
            }
            
        }

    }
}
