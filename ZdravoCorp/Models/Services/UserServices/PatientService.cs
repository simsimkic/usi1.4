using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;
using ZdravoCorp.Models.Entities.Appointments;
using ZdravoCorp.Models.Entities.Users;
using ZdravoCorp.Models.Enumerations;
using ZdravoCorp.Serialization;

namespace ZdravoCorp.Models.Services.UserServices;
public class PatientService
{
    public static List<Patient> _patients = new List<Patient>();
    private static List<Patient> _objPatientsList;

    public PatientService()
    {
        _objPatientsList = new List<Patient>();
        _objPatientsList = PatientsFromCSV("..\\..\\..\\Data\\Users\\Patients\\patients.txt").ToList();
    }
    public List<Patient> GetAll()
    {
        return _objPatientsList;
    }
    public void Add(Patient objNewPatient)
    {
        _objPatientsList.Add(objNewPatient);
    }
    private static ObservableCollection<Patient> PatientsFromCSV(string filename)
    {
        var patientSerializer = new Serializer<Patient>();
        var patients = patientSerializer.fromCSV(filename);
        return new ObservableCollection<Patient>(patients);
    }
    public void PatientsToCSV(ObservableCollection<Patient> patients, string filename)
    {
        Serializer<Patient> patientsSerializer = new Serializer<Patient>();
        patientsSerializer.toCSV(filename, patients.ToList());
    }

    public PatientService(List<Examination> examinations)
    {
        _patients = PatientsFromCSV(examinations);
    }

    private static List<Patient> PatientsFromCSV(List<Examination> examinations)
    {
        Serializer<Patient> patientsSerializer = new Serializer<Patient>();
        var patients = patientsSerializer.fromCSV("..\\..\\..\\Data\\Users\\Patients\\patients.txt");
        return LoadAll(patients, examinations);
    }

    public void PatientsToCSV(List<Patient> patients, string filename)
    {
        Serializer<Patient> patientsSerializer = new Serializer<Patient>();
        patientsSerializer.toCSV(filename, patients);
    }

    public static List<Patient> LoadAll(List<Patient> patients, List<Examination> examinations)
    {
        foreach (Patient patient in patients)
        {
            foreach (int examinationId in patient.ExaminationId)
            {
                foreach (Examination examination in examinations)
                {
                    if (examinationId == examination.Id)
                        patient.Examinations.Add(examination);
                }
            }
        }
        return patients;
    }

    public bool NeedBlockPatient(Patient patient, List<ArchiveExamination> archive)
    {
        bool blockedCurrent = false;
        int counterAppointmentsMonthBefore = 0;
        int counterEditsMonthBefore = 0;
        foreach (ArchiveExamination archiveExamination in archive)
        {
            if (archiveExamination.PatientUsername == patient.Username)
            {
                if (archiveExamination.DateUpdate > DateTime.Now.AddDays(-30))
                {
                    if (archiveExamination.Type == "a")
                        counterAppointmentsMonthBefore++;
                    else
                        counterEditsMonthBefore++;
                }
            }
        }

        if (counterAppointmentsMonthBefore >= 8 || counterEditsMonthBefore >= 5)
            blockedCurrent = true;

        return blockedCurrent;
    }

    public bool IsPatientFree(Patient patient, DateTime dateBeginDoctor)
    {
        bool freeDoctor = true;
        for (int i = 0; i < patient.Examinations.Count; i++)
        {
            if (patient.Examinations[i].DateTime >= dateBeginDoctor && patient.Examinations[i].DateTime <= dateBeginDoctor.AddMinutes(15) &&
                patient.Examinations[i].Status == AppointmentStatus.Scheduled)
            {
                freeDoctor = false;
                break;
            }
        }
        return freeDoctor;
    }
    public List<Patient> GetPatients()
    {
        return _patients;
    }
    public void RemovePatients(Patient patientToDelete)
    {

        Patient patientToRemove = _patients.Find(patient => patient.Id == patientToDelete.Id); // find the patient in the list
        if (patientToRemove != null)
            _patients.Remove(patientToRemove); // remove the patient from the list
    }

    public void AddPatients(Patient patient)
    {
        _patients.Add(patient);
    }
    public bool IsPatient(string username, string password)
    {
        return _patients.Any(patient => patient.Username == username && patient.Password == password && !patient.Blocked);
    }
    public Patient LoginPatient(string username, string password)
    {
        return _patients.FirstOrDefault(patient => patient.Username == username && patient.Password == password);
    }

    public void AddExaminationToPatient(Patient patient,Examination examination)
    {
        Patient NewPatient = new Patient();
        patient.Examinations.Add(examination);
        NewPatient = _objPatientsList.FirstOrDefault(p => p.Id == patient.Id);
        NewPatient?.ExaminationId.Add(examination.Id);
        NewPatient?.Examinations.Add(examination);

        RemovePatients(patient);
        AddPatients(NewPatient);
        PatientsToCSV(_objPatientsList, "..\\..\\..\\Data\\Users\\Patients\\patients.txt");
    }

    public Patient FindPatientById(uint id)
    {
        return _patients.FirstOrDefault(patient => patient.Id == id);
    }
}