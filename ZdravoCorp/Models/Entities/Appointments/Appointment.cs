using System;
using ZdravoCorp.Models.Entities.Users;
using ZdravoCorp.Models.Enumerations;
using ZdravoCorp.Serialization;

namespace ZdravoCorp.Models.Entities.Appointments;
public abstract class Appointment: Serializable {
    private uint _id;
    private Doctor _doctor;
    private Patient _patient;
    private DateTime _startTime;
    private AppointmentStatus _status;

    public uint Id {
        get => _id;
        protected set => _id = value;
    }

    public Doctor Doctor {
        get => _doctor;
        set => _doctor = value;
    }

    public Patient Patient {
        get => _patient;
        set => _patient = value;
    }

    public DateTime DateTime {
        get => _startTime;
        set => _startTime = value;
    }

    public AppointmentStatus Status
    {
        get => _status;
        set => _status = value;
    }
    protected Appointment() {}

    protected Appointment(uint id, Doctor doctor, Patient patient, DateTime startTime) {
        _id = id;
        _doctor = doctor;
        _patient = patient;
        _startTime = startTime;
        _status = AppointmentStatus.Scheduled;
    }

    public abstract string[] ToCSV();
    public abstract void FromCSV(string[] values);
}