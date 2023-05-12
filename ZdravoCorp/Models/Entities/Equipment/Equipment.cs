using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Documents;
using System.Xml.Linq;
using ZdravoCorp.Serialization;

namespace ZdravoCorp.Models.Entities.ManagerEntities
{
    public class Equipment : Serializable
    {
        private int id { get; set; }
        private string model { get; set; } //name
        private string type { get; set; } //type of activity it is used for
        private string location { get; set; } //where the equipment part is stored
        private int amount { get; set; } //where the equipment part is stored

        public int Id
        {
            get { return id; }
            set { id = value; }
        }

        public string Model
        {
            get { return model; }
            set { model = value; }
        }

        public string Type
        {
            get { return type; }
            set { type = value; }
        }

        public string Location
        {
            get { return location; }
            set { location = value; }
        }

        public int Amount
        {
            get { return amount; }
            set { amount = value; }
        }

        public Equipment()
        {
            id = 0;
            model = "";
            type = "";
            location = "";
        }

        public Equipment(int id_, string model_, string type_, string location_)
        {
            id = id_;
            model = model_;
            type = type_;
            location = location_;
        }

        public string[] ToCSV()
        {
            string[] csvValues =
            {
                id.ToString(),
                model,
                type,
                location
            };
            return csvValues;
        }

        public void FromCSV(string[] values)
        {
            id = int.Parse(values[0]);
            model = values[1];
            type = values[2];
            location = values[3];
        }

        public override string ToString()
        {
            return "id: " + id.ToString() + "\n" + "model: " + model.ToString() + "\n" + "type: " + type.ToString() +
                   "\n" + "location: " + location.ToString() + "\n" + "Amount: " + amount.ToString() + "\n";
        }
    }
}
