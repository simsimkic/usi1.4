using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Automation;
using ZdravoCorp.Models.Entities.ManagerEntities;
using ZdravoCorp.Models.Services.ManagerServices;

namespace ZdravoCorp.ViewModels.ManagerViewModels
{
    public class EquipmentManager : INotifyPropertyChanged
    {
        public event PropertyChangedEventHandler? PropertyChanged;

        private void OnPropertyChanged(string propertyName)
        {
            if (PropertyChanged != null)
                PropertyChanged(this, new PropertyChangedEventArgs(propertyName));
        }

        private ManagerService ManagerService;
        private HospitalEquipmentService HospitalEquipmentService;
        public EquipmentManager()
        {
            ManagerService = new ManagerService();
            HospitalEquipmentService = new HospitalEquipmentService();
            EquipmentList = new List<Equipment>();
            UnavailableEquipmentList = HospitalEquipmentService.UnavailableEquipment();
        }

        public EquipmentManager(string name_, string surname_, string username_, string password_)
        {
            ManagerService = new ManagerService();
            HospitalEquipmentService = new HospitalEquipmentService();
            ManagerService.AssignManager(name_, surname_, username_, password_);
            EquipmentList = new List<Equipment>();
            UnavailableEquipmentList = HospitalEquipmentService.UnavailableEquipment();
        }

        private List<Equipment> equipmentList;
        public List<Equipment> EquipmentList
        {
            get { return equipmentList; }
            set { equipmentList = value; OnPropertyChanged("EquipmentList"); }
        }

        private List<Equipment> unavailableEquipmentList;
        public List<Equipment> UnavailableEquipmentList
        {
            get { return unavailableEquipmentList; }
            set { unavailableEquipmentList = value; OnPropertyChanged("UnavailableEquipmentList"); }
        }

        public void UpdateEquipmentList()
        {
            List<Equipment> filteredEquipment = new List<Equipment>();
            EquipmentList = ManagerService.Manager.EquipmentFromCsv();

            int occuranceCounter;
            int occuranceCounter2;
            foreach (string model in HospitalEquipmentService.AllEquipmentModels())
            {
                occuranceCounter = 0;
                foreach (Equipment equipment in EquipmentList)
                {
                    if (equipment.Model.Equals(model))
                        occuranceCounter++;
                }

                if (occuranceCounter < 5)
                {
                    occuranceCounter2 = 0;
                    foreach (Equipment equipment in EquipmentList)
                    {
                        if (equipment.Model.Equals(model))
                        {
                            if (occuranceCounter2 < 1)
                                if (equipment.Type.Equals("oprema za preglede") || equipment.Type.Equals("oprema za operacije"))
                                    filteredEquipment.Add(equipment);
                            equipment.Amount = occuranceCounter;
                            occuranceCounter2++;
                        }
                    }
                }
            }

            foreach (Equipment equipment in UnavailableEquipmentList)
            {
                filteredEquipment.Add(equipment);
            }
            EquipmentList = filteredEquipment;
        }

        public void AddToStorage(Equipment equipment, int amount)
        {
            if (equipment.Type.Equals("") && equipment.Amount == 0)
            {
                equipment.Type = "oprema za operacije";
                equipment.Amount = amount;
                HospitalEquipmentService.UpdateUnavailableEquipment(equipment);
                UnavailableEquipmentList = HospitalEquipmentService.UnavailableEquipment();
                UpdateEquipmentList();
            }

            Random r = new Random();
            for (int i = 0; i < amount; i++)
            {
                ManagerService.Manager.Equipment.Add(new Equipment(r.Next(0, 100), equipment.Model, equipment.Type, "magacin"));
            }
            ManagerService.Manager.EquipmentToCsv("..\\..\\..\\Data\\Equipment\\equipment.txt");
        }
    }
}
