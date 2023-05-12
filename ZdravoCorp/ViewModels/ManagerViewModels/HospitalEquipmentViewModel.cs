using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.ComponentModel;

using ZdravoCorp.Models;
using System.Windows.Input;
using ZdravoCorp.Models.Entities.ManagerEntities;
using ZdravoCorp.Models.Services.ManagerServices;

namespace ZdravoCorp.ViewModels.ManagerViewModels
{
    public class HospitalEquipmentViewModel : INotifyPropertyChanged
    {
        public event PropertyChangedEventHandler? PropertyChanged;

        private void OnPropertyChanged(string propertyName)
        {
            if (PropertyChanged != null)
                PropertyChanged(this, new PropertyChangedEventArgs(propertyName));
        }

        private ManagerService ManagerService;
        private HospitalEquipmentService HospitalEquipmentService;
        public HospitalEquipmentViewModel()
        {
            ManagerService = new ManagerService();
            HospitalEquipmentService = new HospitalEquipmentService();
            FilteringTypes = ManagerService.Manager.FilteringTypes;
            FilteringSecondTypes = new List<string>() { };
            FilteringQuantityTypes = new List<string>() { };
            LoadEquipment();
            EquipmentStatus = HospitalEquipmentService.EquipmentStatus;
        }

        public HospitalEquipmentViewModel(string name_, string surname_, string username_, string password_)
        {
            ManagerService = new ManagerService();
            HospitalEquipmentService = new HospitalEquipmentService();
            ManagerService.AssignManager(name_, surname_, username_, password_);
            FilteringTypes = ManagerService.Manager.FilteringTypes;
            FilteringSecondTypes = new List<string>() { };
            FilteringQuantityTypes = new List<string>() { };
            LoadEquipment();
            EquipmentStatus = HospitalEquipmentService.EquipmentStatus;
        }

        private List<Equipment> equipmentList;
        public List<Equipment> EquipmentList
        {
            get { return equipmentList; }
            set { equipmentList = value; OnPropertyChanged("EquipmentList"); }
        }

        private Dictionary<string, string> equipmentStatus;
        public Dictionary<string, string> EquipmentStatus
        {
            get { return equipmentStatus; }
            set { equipmentStatus = value; OnPropertyChanged("EquipmentStatus"); }
        }

        private List<string> filteringTypes;
        public List<string> FilteringTypes
        {
            get { return filteringTypes; }
            set { filteringTypes = value; OnPropertyChanged("FilteringTypes"); }
        }

        private List<string> filteringSecondTypes;
        public List<string> FilteringSecondTypes
        {
            get { return filteringSecondTypes; }
            set { filteringSecondTypes = value; OnPropertyChanged("FilteringSecondTypes"); }
        }

        private List<string> filteringQuantityTypes;
        public List<string> FilteringQuantityTypes
        {
            get { return filteringQuantityTypes; }
            set { filteringQuantityTypes = value; OnPropertyChanged("FilteringQuantityTypes"); }
        }

        public void LoadEquipment()
        {
            EquipmentList = ManagerService.Manager.Equipment;
        }

        public void UpdateFiltringSecondType(int itemSelected)
        {
            if (itemSelected == 0)
                FilteringSecondTypes = new List<string>() { "operaciona sala", "magacin", "sala za preglede", "sala za smestaj bolesnika", "cekaonica" };
            else
                FilteringSecondTypes = new List<string>() { "oprema za operacije", "oprema za hodnike", "sobni namestaj", "oprema za preglede" };
        }

        public void UpdateFilteringQuantityType(int itemSelected)
        {
            if (itemSelected == 0)
                FilteringQuantityTypes = new List<string>() { };
            else
                FilteringQuantityTypes = new List<string>() { "no filter", "nema na stanju", "0-10", "10+" };
        }

        public void FilterEquipment(string filterType, int dropBoxOneIndex, string quantity)
        {
            if (string.IsNullOrEmpty(filterType))
            {
                LoadEquipment(); // reset to original list if no filter is specified
                FilteringSecondTypes = new List<string>();
                return;
            }

            Console.WriteLine(filterType);
            if (dropBoxOneIndex == 0)
                EquipmentList = ManagerService.Manager.Equipment
                    .Where(e => e.Location.Equals(filterType))
                    .ToList();
            else
            {
                EquipmentList = ManagerService.Manager.Equipment
                    .Where(e => e.Type.Equals(filterType))
                    .ToList();

                switch (quantity)
                {
                    case "no filter":
                        break;
                    case "nema na stanju":
                        EquipmentList = new List<Equipment>();

                        foreach (string key in EquipmentStatus.Keys)
                        {
                            EquipmentList.Add(new Equipment(0, key, "", ""));
                        }
                    
                        break;
                    case "0-10":
                        if (EquipmentList.Count < 1 || EquipmentList.Count > 9)
                            EquipmentList = new List<Equipment>();
                        break;
                    case "10+":
                        if (EquipmentList.Count < 10)
                            EquipmentList = new List<Equipment>();
                        break;
                    default:
                        break;
                }
                if (EquipmentList.Count == 0)
                    Console.WriteLine("ima 0");
                else if (EquipmentList.Count < 10)
                    Console.WriteLine("ima 0-10: " + EquipmentList.Count);
                else
                    Console.WriteLine("ima 10+: " + EquipmentList.Count);
            }
        }

        public void FilterByText(string searchText)
        {
            LoadEquipment();

            var filteredEquipment = from eq in EquipmentList
                                    where eq.Model.ToLower().Contains(searchText) ||
                                          eq.Type.ToLower().Contains(searchText) ||
                                          eq.Location.ToLower().Contains(searchText)
                                    select eq;

            Console.WriteLine(filteredEquipment.Count());

            EquipmentList = filteredEquipment.ToList();
        }
    }
}
