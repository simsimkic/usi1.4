using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using System.Numerics;
using System.Text;
using System.Threading.Tasks;
using ZdravoCorp.Serialization;
using System.ComponentModel;
using ZdravoCorp.Models.Entities.Users;

namespace ZdravoCorp.Models.Entities.ManagerEntities
{
    public class Manager : User, INotifyPropertyChanged
    {
        private List<Equipment> _equipment;
        private List<Room> _rooms;
        private List<string> _filteringTypes; //potentially should be moved

        public List<Equipment> Equipment
        {
            get => _equipment;
            set
            {
                _equipment = value;
                OnPropertyChanged("Equipment");
            }
        }

        public List<Room> Rooms
        {
            get => _rooms;
            set
            {
                _rooms = value;
                OnPropertyChanged("Rooms");
            }
        }

        public List<string> FilteringTypes
        {
            get => _filteringTypes;
            set
            {
                _filteringTypes = value;
                OnPropertyChanged("FilteringTypes");
            }
        }

        public Manager() : base()
        {
            _equipment = new List<Equipment>();
            _rooms = new List<Room>();
            _filteringTypes = new List<string>();
        }

        public Manager(uint id, string username, string password, string firstName, string lastName) :
            base(id, username, password, firstName, lastName)
        {
            _equipment = EquipmentFromCsv();
            _rooms = RoomsFromCSV();
            _filteringTypes = new List<string>()
            {
                "tip prostorije",
                "tip opreme"
            };
        }
        
        public List<Equipment> EquipmentFromCsv()
        {
            Serializer<Equipment> equipmentSerializer = new Serializer<Equipment>();
            return equipmentSerializer.fromCSV("..\\..\\..\\Data\\Equipment\\equipment.txt");
            ;
        }

        public void EquipmentToCsv(string filename)
        {
            Serializer<Equipment> equipmentSerializer = new Serializer<Equipment>();
            equipmentSerializer.toCSV(filename, _equipment);
        }

        public List<Room> RoomsFromCSV()
        {
            Serializer<Room> equipmentSerializer = new Serializer<Room>();
            return equipmentSerializer.fromCSV("..\\..\\..\\Data\\Rooms\\rooms.txt");
            ;
        }

        public void RoomsToCSV(string filename)
        {
            Serializer<Room> roomSerializer = new Serializer<Room>();
            roomSerializer.toCSV(filename, _rooms);
        }

        public override void FromCSV(string[] values)
        {
            Id = uint.Parse(values[0]);
            Username = values[1];
            Password = values[2];
            FirstName = values[3];
            LastName = values[4];
        }

        public override string[] ToCSV()
        {
            string[] csvValues =
            {
                Id.ToString(),
                Username,
                Password,
                FirstName,
                LastName
            };
            return csvValues;
        }

        public event PropertyChangedEventHandler? PropertyChanged;

        private void OnPropertyChanged(string propertyName)
        {
            PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(propertyName));
        }
    }
}
