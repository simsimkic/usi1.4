using System;
using System.ComponentModel;
using ZdravoCorp.Models.Entities.Users;
using ZdravoCorp.Models.Enumerations;

namespace ZdravoCorp.Models.Entities.Appointments;
public class Examination: Appointment
{
    public Examination(): base() {}

    public Examination(uint id, Doctor doctor, Patient patient, DateTime startTime): 
        base(id, doctor, patient, startTime) {}

    public override string[] ToCSV() {
        string[] csvValues = {
            Id.ToString(),
            Doctor.Id.ToString(),
            Patient.Id.ToString(),
            DateTime.ToString("yyyy-MM-dd HH:mm:ss"),
            Status.ToString(),
        };
        return csvValues;
    }

    public override void FromCSV(string[] values) {
        Id = uint.Parse(values[0]);
        Doctor = new Doctor(uint.Parse(values[1]));
        Patient = new Patient(uint.Parse(values[2]));
        DateTime = DateTime.Parse(values[3]);
        Status = (AppointmentStatus)Enum.Parse(typeof(AppointmentStatus), values[4]);
    }

    public event PropertyChangedEventHandler PropertyChanged;
    protected virtual void OnPropertyChanged(string name) {
        PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(name));
    }
}