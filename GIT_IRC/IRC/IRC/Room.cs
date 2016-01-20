using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.ComponentModel;
using System.Collections.ObjectModel;
using System.Runtime.CompilerServices;


namespace IRC
{
    public class Room : INotifyPropertyChanged
    {
        private String name; //nazwa pokoju

        public enum inside { TAK, NIE }; //czy user w środku
        

        private string _ChatText;

        public string ChatText
        {
            get { return _ChatText; }
            set
            {
                if (value != _ChatText)
                {
                    _ChatText = value;
                    NotifyPropertyChanged(name);
                }
            }
        }

        public String Name
        {
            get { return name; }
            set 
            { 
                    if(this.name!=value)
                    {
                        this.name = value;
                        this.NotifyPropertyChanged("Name");
                    }
           }
        }


        private inside _isInside = inside.NIE;

        public inside IsInside
        {
            get { return _isInside; }
            set
            {
                if (value != _isInside)
                {
                    _isInside = value;
                    NotifyPropertyChanged(name);
                }
            }
        }

        private inside _newMessages = inside.NIE;

        public inside NewMessages
        {
            get { return _newMessages; }
            set
            {
                if (value != _newMessages)
                {
                    _newMessages = value;
                    NotifyPropertyChanged(name);
                }
            }
        }
        


        private int actualOccupancy; //ile osób aktualnie w pokoju

        public int ActualOccupancy
        {
            get { return actualOccupancy; }
            set { actualOccupancy = value; }
        }

        private int occupancy; //ile max osób w pokoju

        public int Occupancy
        {
            get { return occupancy; }
            set { occupancy = value; }
        }
        List <User> users = new List<User>();


        public event PropertyChangedEventHandler PropertyChanged;

        public void NotifyPropertyChanged (string handleName)
        {
            if(this.PropertyChanged!=null)
            {
                this.PropertyChanged(this, new PropertyChangedEventArgs(handleName));
            }
        }

        public void addUser(User addingUser)
        {
            users.Add(addingUser);
        }

        public Room (String name,int occupancy) //konstruktor
        {
            this.name=name;
            this.occupancy = occupancy;
        }

    }
}
