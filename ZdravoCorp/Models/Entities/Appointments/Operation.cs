using System;
using ZdravoCorp.Models.Entities.Users;
using ZdravoCorp.Models.Enumerations;

namespace ZdravoCorp.Models.Entities.Appointments;
public class Operation: Appointment {
    private uint _durationInMinutes;

    public uint Duration {
        get => _durationInMinutes;
        set => _durationInMinutes = value;
    }

    public Operation() : base() { }

    public Operation(uint id, Doctor doctor, Patient patient, DateTime dateBegin, uint duration)
    {
        Id = id;
        Doctor = doctor;
        Patient = patient;
        DateTime = dateBegin;
        Duration = duration;
    }
    public override string[] ToCSV() {
        string[] csvValues = {
            Id.ToString(),
            Doctor.Id.ToString(),
            Patient.Id.ToString(),
            DateTime.ToString("yyyy-MM-dd HH:mm:ss"),
            Duration.ToString(),
            Status.ToString(),
        };
        return csvValues;
    }
    public override void FromCSV(string[] values) {
        Id = uint.Parse(values[0]);
        Doctor = new Doctor(uint.Parse(values[1]));
        Patient = new Patient(uint.Parse(values[2]));
        DateTime = DateTime.Parse(values[3]);
        Duration = uint.Parse(values[4]);
        Status = (AppointmentStatus)Enum.Parse(typeof(AppointmentStatus), values[5]);
    }
}