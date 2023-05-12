using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using ZdravoCorp.Models.Entities.Appointments;
using ZdravoCorp.Models.Entities.Users;
using ZdravoCorp.Models.Enumerations;
using ZdravoCorp.Serialization;

namespace ZdravoCorp.Models.Entities.Notification;

public class Notification : Serializable
{
    Patient _patient;

    Doctor _doctor;

    string _message;

    bool _seenPatient;

    bool _seenDoctor;
    public Patient Patient
    {
        get => _patient;
        set => _patient = value;
    }
    public Doctor Doctor
    {
         get => _doctor;
         set => _doctor = value;
    }
    public string Message
    {
        get => _message;
        set => _message = value;
    }
    public bool SeenPatient
    {
        get => _seenPatient;
        set => _seenPatient = value;
    }
    public bool SeenDoctor
    {
        get => _seenDoctor;
        set => _seenDoctor = value;
    }
    public Notification()
    {
        _patient = new Patient();
        _doctor = new Doctor();
        _seenPatient = false;
        _seenDoctor = false;
    }
    public Notification(Doctor doctor, Patient patient,string message)
    {
        Doctor = doctor;
        Patient = patient;
        Message = message;
        _seenPatient = false;
        _seenDoctor = false;
    }
    public void FromCSV(string[] values)
    {
        Doctor = new Doctor(uint.Parse(values[0]));
        Patient = new Patient(uint.Parse(values[1]));
        Message = values[2];
        SeenDoctor = bool.Parse(values[3]);
        SeenPatient = bool.Parse(values[4]);
        
    }
    public string[] ToCSV()
    {
        string[] csvValues = {
        Doctor.Id.ToString(),
        Patient.Id.ToString(),
        Message,
        SeenDoctor.ToString(),
        SeenPatient.ToString(),
    };
        return csvValues;
    }
}

