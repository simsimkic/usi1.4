using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using ZdravoCorp.Serialization;

namespace ZdravoCorp.Models.Entities.ManagerEntities
{
    public class Room : Serializable
    {
        private int id { get; set; }
        private string type { get; set; }
        private List<Equipment> equipment;

        public int Id
        {
            get { return id; }
            set { id = value; }
        }

        public string Type
        {
            get { return type; }
            set { type = value; }
        }

        public List<Equipment> Equipment
        {
            get { return equipment; }
            set { equipment = value; }
        }

        public Room()
        {
            id = 0;
            type = "";
            equipment = new List<Equipment>();
        }

        public Room(int id_, string type_, List<Equipment> equipment_)
        {
            id = id_;
            type = type_;
            equipment = equipment_;
        }

        public void FromCSV(string[] values)
        {
            id = int.Parse(values[0]);
            type = values[1];

            List<Equipment> allEquipment = EquipmentFromCSV();
            string[] equipmentIds = values[2].Split(","); //loads ids of equipment that belongs to that room
            foreach (Equipment eqpmt in allEquipment)
            {
                if (equipmentIds.Contains(eqpmt.Id.ToString()))
                    equipment.Add(eqpmt);
            }
        }

        public string[] ToCSV()
        {
            string[] csvValues =
            {
                id.ToString(),
                type,
                equipment.ToString()
            };
            return csvValues;
        }
        public List<Equipment> EquipmentFromCSV()
        {
            Serializer<Equipment> equipmentSerializer = new Serializer<Equipment>();
            List<Equipment> allEquipment = equipmentSerializer.fromCSV("..\\..\\..\\Data\\Equipment\\equipment.txt");

            return allEquipment;
        }

        public override string ToString()
        {
            return "id: " + id.ToString() + "\ntype: " + type.ToString() + "\nequipment: " + equipmentToString();
        }

        private string equipmentToString()
        {
            string eqpmtString = "{\n";
            foreach (Equipment eqpmt in equipment)
            {
                eqpmtString += eqpmt.ToString() + "\n";
            }
            eqpmtString += "}\n";

            return eqpmtString;
        }
    }
}
