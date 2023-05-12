using System;
using System.Collections.Generic;
using System.ComponentModel;
using ZdravoCorp.Models.Services.ManagerServices;
using ZdravoCorp.Models.Services.UserServices;
using ZdravoCorp.Models.Entities.ManagerEntities;
using System.Linq;
using System.IO;
using System.Reflection.Metadata;

namespace ZdravoCorp.ViewModels.ManagerViewModels
{
    public class EquipmentTransferViewModel : INotifyPropertyChanged
    {
        public event PropertyChangedEventHandler? PropertyChanged;

        private void OnPropertyChanged(string propertyName)
        {
            if (PropertyChanged != null)
                PropertyChanged(this, new PropertyChangedEventArgs(propertyName));
        }

        private List<string> equipmentModels;
        private RoomService roomService;
        private List<Room> allRooms;
        private ManagerService managerService;
        private HospitalEquipmentService hospitalEquipmentService;
        private List<int> allRoomsIds;

        public EquipmentTransferViewModel(string name_, string surname_, string username_, string password_)
        {
            roomService = new RoomService();
            hospitalEquipmentService = new HospitalEquipmentService();
            managerService = new ManagerService();
            managerService.AssignManager(name_, surname_, username_, password_);
            LoadRooms();
            EquipmentModels = hospitalEquipmentService.AllEquipmentModels();
            GetAllRoomIds();
        }

        public List<string> EquipmentModels
        {
            get { return equipmentModels; }
            set { equipmentModels = value; OnPropertyChanged("EquipmentModels"); }
        }

        public List<Room> AllRooms
        {
            get { return allRooms; }
            set { allRooms = value; OnPropertyChanged("AllRooms"); }
        }

        public List<int> AllRoomsIds
        {
            get { return allRoomsIds; }
            set { allRoomsIds = value; OnPropertyChanged("AllRoomsIds"); }
        }

        public void FilterRooms(string filteringType)
        {
            LoadRooms();

            AllRooms.RemoveAll(room =>
            {
                bool shouldRemoveRoom = room.Equipment.Any(equipment =>
                    equipment.Model.Equals(filteringType) && equipment.Amount >= 5);

                return shouldRemoveRoom;
            });
            
            foreach (Room room in AllRooms)
            {
                Console.WriteLine(room.ToString());
            }
            Console.WriteLine("-----------------------");
        }

        public void LoadRooms()
        {
            AllRooms = managerService.Manager.RoomsFromCSV();
            foreach (Room room in AllRooms)
                hospitalEquipmentService.SetAllEquipmentAmounts(room.Equipment);
        }

        public void GetAllRoomIds()
        {
            AllRoomsIds = new List<int>();

            foreach (Room elementRoom in managerService.Manager.RoomsFromCSV())
            {
                AllRoomsIds.Add(elementRoom.Id);
            }
        }

        public void FilterRoomIds(Room room, string filteringType)
        {
            List<Room> allRoomsFromCsv = managerService.Manager.RoomsFromCSV();

            // Filter the rooms based on the conditions
            List<int> filteredRoomIds = allRoomsFromCsv
                .Where(r => r.Id != room.Id && r.Equipment.Any(e => e.Model.Equals(filteringType)))
                .Select(r => r.Id)
                .ToList();

            // Update the AllRoomIds list with the filtered room IDs
            AllRoomsIds = AllRoomsIds.Intersect(filteredRoomIds).ToList();
        }

        public Room FindRoomById(int selectionId)
        {
            List<Room> allRoomsFromCsv = managerService.Manager.RoomsFromCSV(); //acts as allRooms double, it exists just in case not to disturb any previous values in allRooms if they are being used at the moment of this func call
            foreach (Room room in allRoomsFromCsv)
            {
                hospitalEquipmentService.SetAllEquipmentAmounts(room.Equipment);
            }

            //finds room then sets the list of amounts that should be made above to the amounts needed.
            Room roomToFind = new Room();
            foreach (Room room in allRoomsFromCsv)
            {
                if (room.Id == selectionId)
                    roomToFind = room;
            }
            Console.WriteLine(roomToFind.ToString());
            Console.WriteLine("____________________________");
            
            return roomToFind;
        }

        public void MoveEquipment(Room roomTo, Room  roomFrom, string selectedItem, int amount)
        {
            LoadRooms();
            List < Equipment > equipmentToRemove = GetEquipmentToTransfer(roomFrom, selectedItem, amount);

            Console.WriteLine("----------------------------");
            Console.WriteLine("AFTER MOVE: ");

            List<Room> allRoomsClone = managerService.Manager.RoomsFromCSV();
            int indexRemove = 0, indexAdd = 0;
            foreach (Room room in allRoomsClone)
            {
                if (room.Id == roomFrom.Id)
                {
                    //change roomFrom
                    indexRemove = allRoomsClone.IndexOf(room);
                    RemoveEquipmentFromRoom(roomFrom, selectedItem, amount);
                }
                else if (room.Id == roomTo.Id)
                {
                    //change roomTo
                    indexAdd = allRoomsClone.IndexOf(room);
                    AddEquipmentToRoom(roomTo, equipmentToRemove);
                }
            }

            //just writes results to console for user to see if the transfer was correct
            Console.WriteLine("FROM:\n" + AllRooms[indexRemove].ToString());
            Console.WriteLine("TO:\n" + AllRooms[indexAdd].ToString());

            WriteChangesToRooms();
            WriteChangesToEquipment(roomFrom, roomTo);
        }

        public List<Equipment> GetEquipmentToTransfer(Room room, string filteringType, int amount)
        {
            return room.Equipment
                .Where(equipment => equipment.Model.Equals(filteringType))
                .Take(amount)
                .ToList();
        }

        public void RemoveEquipmentFromRoom(Room room, string filteringType, int amount)
        {
            int roomIndex = 0;
            for (int i = 0; i < AllRooms.Count; i++)
            {
                if (AllRooms.ElementAt(i).Id == room.Id)
                {
                    roomIndex = i;
                    break;
                }
            }

            List<Equipment> equipmentToRemove = room.Equipment
                .Where(equipment => equipment.Model.Equals(filteringType))
                .Take(amount)
                .ToList();

            // Remove the equipment items from the room
            foreach (Equipment equipment in equipmentToRemove)
            {
                room.Equipment.Remove(equipment);
            }

            AllRooms[roomIndex] = room;
        }

        public void AddEquipmentToRoom(Room room, List<Equipment> equipmentToAdd)
        {
            int roomIndex = 0;
            for (int i = 0; i < AllRooms.Count; i++)
            {
                if (AllRooms.ElementAt(i).Id == room.Id)
                {
                    roomIndex = i;
                    break;
                }
            }

            foreach (Equipment equipment in equipmentToAdd)
            {
                room.Equipment.Add(equipment);
            }

            AllRooms[roomIndex] = room;
        }

        public void WriteChangesToRooms()
        {
            using (StreamWriter writer = new StreamWriter("..\\..\\..\\Data\\Rooms\\rooms.txt"))
            {
                string content = "";
                foreach (Room room in AllRooms)
                {
                    string equipmentString = "";
                    List<Equipment> equipment = room.Equipment;
                    foreach (var item in equipment)
                    {
                        equipmentString += item + ",";
                    }

                    equipmentString = equipmentString.Substring(0, equipmentString.Length - 1);
                    content += room.Id.ToString() + "|" + room.Type + "|" + equipmentString + "\n";
                }

                writer.Write(content);
            }

        }

        public void WriteChangesToEquipment(Room roomFrom, Room roomTo)
        {

        }
    }
}
