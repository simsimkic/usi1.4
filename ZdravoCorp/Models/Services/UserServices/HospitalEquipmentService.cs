using System;
using System.Collections;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

using ZdravoCorp.Models.Entities.ManagerEntities;
using ZdravoCorp.Serialization;

namespace ZdravoCorp.Models.Services.ManagerServices
{
    class HospitalEquipmentService
    {
        //{ eqptName, available/unavailable }
        private Dictionary<string, string> equipmentStatus;

        private ManagerService managerService;
        private List<Equipment> equipmentList;

        public HospitalEquipmentService()
        {
            managerService = new ManagerService();
            EquipmentStatus = new Dictionary<string, string>();
            equipmentStatusFromCSV();
            equipmentList = managerService.Manager.EquipmentFromCsv();
            SetAllEquipmentAmounts(equipmentList);
        }

        public Dictionary<string, string> EquipmentStatus
        {
            get { return equipmentStatus; }
            set { equipmentStatus = value; }
        }

        private void equipmentStatusFromCSV()
        {
            EquipmentStatus.Clear();
            using (StreamReader reader = new StreamReader("..\\..\\..\\Data\\Equipment\\equipmentStatus.txt"))
            {
                string line;
                string[] lineSplit;
                while ((line = reader.ReadLine()) != null)
                {
                    lineSplit = line.Split('|');
                    EquipmentStatus.Add(lineSplit[0], lineSplit[1]);
                }
            }
        }

        public List<Equipment> UnavailableEquipment()
        {
            List<Equipment> unavailablEquipment = new List<Equipment>();

            foreach (string key in EquipmentStatus.Keys)
            {
                if (EquipmentStatus[key].Equals("unavailable"))
                    unavailablEquipment.Add(new Equipment(0, key, "", ""));
            }

            return unavailablEquipment;
        }

        public void SetAllEquipmentAmounts(List<Equipment> equipmentList)
        {
            List<string> allModels = AllEquipmentModels();
            int occuranceCounter;

            foreach (Equipment equipment in equipmentList) // resets the amounts if for some reason they are not already 0
                equipment.Amount = 0;

            foreach (string model in allModels)
            {
                occuranceCounter = 0;
                foreach (Equipment equipment in equipmentList)
                {
                    if (equipment.Model.Equals(model))
                    {
                        occuranceCounter++;
                    }
                }

                foreach (Equipment equipment in equipmentList)
                {
                    if (equipment.Model.Equals(model))
                        equipment.Amount = occuranceCounter;
                }
            }
        }

        public List<string> AllEquipmentModels()
        {   
            equipmentList.Clear();
            equipmentList = managerService.Manager.EquipmentFromCsv();
            List<string> allEquipmentModels = new List<string>();
            foreach (Equipment equipment in equipmentList)
            {
                if (!allEquipmentModels.Contains(equipment.Model))
                    allEquipmentModels.Add(equipment.Model);
            }
            return allEquipmentModels;
        }

        public void UpdateUnavailableEquipment(Equipment equipment)
        {
            List<string[]> allLinesSplit = new List<string[]>();
            using (StreamReader reader = new StreamReader("..\\..\\..\\Data\\Equipment\\equipmentStatus.txt"))
            {
                string line;
                string[] lineSplit;
                while ((line = reader.ReadLine()) != null)
                {
                    lineSplit = line.Split('|');
                    if (lineSplit[0].Equals(equipment.Model))
                        lineSplit[1] = "available";
                    allLinesSplit.Add(lineSplit);
                }
            }

            string output = "";
            foreach (string[] line in allLinesSplit)
            {
                foreach (string word in line)
                {
                    output += word + "|";
                }
                output = output.Substring(0, output.Length - 1) + "\n";
            }

            using (StreamWriter writer = new StreamWriter("..\\..\\..\\Data\\Equipment\\equipmentStatus.txt"))
            {
                writer.Write(output);
                writer.Close();
            }

            equipmentStatusFromCSV();
        }
    }
}
