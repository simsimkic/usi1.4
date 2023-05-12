using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using ZdravoCorp.Models.Entities.ManagerEntities;

namespace ZdravoCorp.Models.Services.UserServices
{
    public class RoomService
    {
        public event PropertyChangedEventHandler? PropertyChanged;

        private void OnPropertyChanged(string propertyName)
        {
            if (PropertyChanged != null)
                PropertyChanged(this, new PropertyChangedEventArgs(propertyName));
        }

        private List<Room> allRooms;

        public RoomService()
        {
            allRooms = new List<Room>();
        }

        public List<Room> AllRooms
        {
            get { return allRooms; }
            set { allRooms = value; OnPropertyChanged("AllRooms"); }
        }
    }
}
