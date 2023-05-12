using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ZdravoCorp.Models.Entities.Patients
{
    public class MedicalProfile
    {
        public uint IdAnamnesis { get; set; }
        public DateTime DateTimeAnamnesis { get; set; }

        public string NameSurnameDoctor { get; set; }
        public string SpecializationDoctor { get; set; }

        public string Symptoms { get; set; }

        public string Disease { get; set; }

        public string Allergens { get; set; }
        public MedicalProfile(uint idAnamnesis, DateTime dateTimeAnamnesis, string nameSurnameDoctor, string specializationDoctor, string symptoms, string disease, string allergens)
        {
            IdAnamnesis = idAnamnesis;
            TimeSpan timeSpan = new TimeSpan(0, 15, 0);
            DateTimeAnamnesis = dateTimeAnamnesis.Subtract(timeSpan);
            NameSurnameDoctor = nameSurnameDoctor;
            SpecializationDoctor = specializationDoctor;
            Symptoms = symptoms;
            Disease = disease;
            Allergens = allergens;
        }
    }
}
