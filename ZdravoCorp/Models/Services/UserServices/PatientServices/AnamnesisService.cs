using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;
using ZdravoCorp.Models.Entities.Appointments;
using ZdravoCorp.Models.Entities.Patients;
using ZdravoCorp.Models.Entities.Users;
using ZdravoCorp.Models.Services.AppointmentServices;
using ZdravoCorp.Serialization;

namespace ZdravoCorp.Models.Services.PatientServices;
public class AnamnesisService {
    private static List<Anamnesis> _anamneses;

    public AnamnesisService()
    {
        _anamneses = new List<Anamnesis>();
        _anamneses = AnamnesisFromCSV("..\\..\\..\\Data\\Users\\Patients\\anamneses.txt").ToList();
    }

    public static List<Anamnesis> GetAll()
    {
        return _anamneses;
    }

    public void Add(Anamnesis anamnesis)
    {
        _anamneses.Add(anamnesis);
    }

    public static ObservableCollection<Anamnesis> AnamnesisFromCSV(string filename)
    {
        var anamnesisSerializer = new Serializer<Anamnesis>();
        var anamneses = anamnesisSerializer.fromCSV(filename);
        return new ObservableCollection<Anamnesis>(anamneses);
    }

    public void AnamnesisToCSV(ObservableCollection<Anamnesis> anamneses, string filename)
    {
        Serializer<Anamnesis> recordSerializer = new Serializer<Anamnesis>();
        recordSerializer.toCSV(filename, anamneses.ToList());
    }
    public List<Anamnesis> GetAnamnesisForPatient(Patient patient, ExaminationsServices examinationsServices)
    {
        List<Anamnesis> anamnesisList = new List<Anamnesis>();
        foreach (Anamnesis anamnesis in _anamneses)
        {
            foreach (Examination examination in examinationsServices.GetExaminations())
            {
                if (anamnesis.ExaminationId == examination.Id && examination.Patient.Id == patient.Id)
                {
                    anamnesisList.Add(anamnesis);
                }
            }
        }
        return anamnesisList;
    }
}