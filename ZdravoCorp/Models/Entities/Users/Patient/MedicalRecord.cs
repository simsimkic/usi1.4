using System.ComponentModel;
using System.Linq;
using ZdravoCorp.Serialization;

namespace ZdravoCorp.Models.Entities.Patients {
    public class MedicalRecord: Serializable, IDataErrorInfo {
        public uint Id {
            get;
            set;
        }
        public uint Height {
            get;
            set;
        }

        public uint Weight {
            get;
            set;
        }
        public string MedicalHistory {
            get;
            set;
        }

        public MedicalRecord() {}
        public MedicalRecord(uint id, uint height, uint weight, string medicalHistory) {
            Id = id;
            Height = height;
            Weight = weight;
            MedicalHistory = medicalHistory;
        }

        public void FromCSV(string[] values) {
            Id = uint.Parse(values[0]);
            Height = uint.Parse(values[1]);
            Weight = uint.Parse(values[2]);
            MedicalHistory = values[3];
        }

        public string[] ToCSV() {
            string[] csvValues = {
                Id.ToString(),
                Height.ToString(),
                Weight.ToString(),
                MedicalHistory
            };
            return csvValues;
        }

        public string Error => null;
        public string this[string columnName] {
            get {
                switch (columnName) {
                    case "MedicalHistory":
                        return string.IsNullOrWhiteSpace(MedicalHistory) ? "Medical history is required" : null;
                    case "Height":
                        return Height <= 0 ? "Height must be greater than 0" : null;
                    case "Weight":
                        return Weight <= 0 ? "Weight must be greater than 0" : null;
                    default:
                        return null;
                }
            }
        }

        private readonly string[] _validatedProperties = {
            "Height",
            "Weight",
            "MedicalHistory"
        };
        public bool IsValid => _validatedProperties.All(property => this[property] != null);
    }
}