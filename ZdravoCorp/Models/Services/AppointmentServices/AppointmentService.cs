using System.CodeDom.Compiler;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;
using System.IO;
using System;
using System.Security.Cryptography.Xml;
using ZdravoCorp.Models.Entities.Users;

namespace ZdravoCorp.Models.Services.AppointmentServices;
using Entities.Appointments;
using Serialization;
using ZdravoCorp.Models.Services.UserServices;

public class AppointmentService
{
    private static List<Examination> _allExaminations = new List<Examination>();
    private static List<Operation> _allOperations = new List<Operation>();
    private static readonly string ExaminationsFilename = "..\\..\\..\\Data\\Appointments\\examinations.txt";
    private static readonly string OperationsFilename = "..\\..\\..\\Data\\Appointments\\operations.txt";
    public PatientService PatientService { get; set; }
    public AppointmentService()
    {
        if (File.Exists(ExaminationsFilename))
            _allExaminations = ExaminationsFromCsv(ExaminationsFilename);
        if (File.Exists(OperationsFilename))
            _allOperations = OperationsFromCsv(OperationsFilename);
    }

    private static List<Examination> ExaminationsFromCsv(string filename)
    {
        Serializer<Examination> examinationSerializer = new Serializer<Examination>();
        List<Examination> examinationList = examinationSerializer.fromCSV(filename);
        return examinationList;
    }

    private static List<Operation> OperationsFromCsv(string filename)
    {
        Serializer<Operation> operationSerializer = new Serializer<Operation>();
        List<Operation> operationList = operationSerializer.fromCSV(filename);
        return operationList;
    }

    public void ExaminationsToCsv(ObservableCollection<Examination> examinations)
    {
        List<Examination> examinationsList = examinations.ToList();
        Serializer<Examination> examinationsSerializer = new Serializer<Examination>();
        examinationsSerializer.toCSV(ExaminationsFilename, examinationsList);
    }

    public static void OperationsToCsv(ObservableCollection<Operation> operations)
    {
        List<Operation> operationsList = operations.ToList();
        Serializer<Operation> operationSerializer = new Serializer<Operation>();
        operationSerializer.toCSV(OperationsFilename, operationsList);
    }
    public List<Appointment> GetAllAppointments()
    {
        List<Appointment> _allAppointments = new List<Appointment>();
        _allAppointments.AddRange(_allExaminations);
        _allAppointments.AddRange(_allOperations);
        return _allAppointments;
    }
    public List<Examination> GetAllExaminations()
    {
        return _allExaminations;
    }
    public void AddExaminations(Examination objNewExamination)
    {
        _allExaminations.Add(objNewExamination);
    }
    public void RemoveExamination(Examination examination)
    {
        Examination examinationToRemove = _allExaminations.Find(e => e.Id == examination.Id);
        if (examinationToRemove != null)
            _allExaminations.Remove(examinationToRemove);
    }
    public List<Operation> GetAllOperations()
    {
        return _allOperations;
    }
    public void AddOperations(Operation objNewOperation)
    {
        _allOperations.Add(objNewOperation);
    }
    public void RemoveOperation(Operation operation)
    {
        Operation operationToRemove = _allOperations.Find(o => o.Id == operation.Id);
        if (operationToRemove != null)
            _allOperations.Remove(operationToRemove);
    }

    public void GetDoctorsExaminations(Doctor doctor)
    {
        List<Examination> examinations = new List<Examination>();
        foreach (var examination in _allExaminations)
        {
            if (examination.Doctor.Id.Equals(doctor.Id))
                examinations.Add(examination);
        }

        doctor.Examinations = examinations;
    }

    public void GetDoctorsOperations(Doctor doctor)
    {
        List<Operation> operations = new List<Operation>();
        foreach (var operation in _allOperations)
        {
            if (operation.Doctor.Id.Equals(doctor.Id))
                operations.Add(operation);
        }

        doctor.Operations = operations;
    }
    public void GetDoctorsAppointments(Doctor doctor)
    {
        GetDoctorsExaminations(doctor);
        GetDoctorsOperations(doctor);
    }
    
    public void GetPatientsExaminations(Patient patient)
    {
        List<Examination> patientsExaminations = new List<Examination>();
        foreach (var examination in _allExaminations)
        {
            if (examination.Patient.Id.Equals(patient.Id))
                patientsExaminations.Add(examination);
        }
        patient.Examinations = patientsExaminations;
    }

    public List<Appointment> CheckAvailabilityAppointments(string specialization,DoctorService doctorService)
    {
        List<Appointment> appointments = new List<Appointment>();
        List<Appointment> allAppointments = GetAllAppointments();
        allAppointments.Sort((x, y) => DateTime.Compare(x.DateTime, y.DateTime));
        DateTime currentDateTime = DateTime.Now;
        int countAppointment = 0;

        foreach (Appointment appointment in allAppointments)
        {
            if (appointment.DateTime > currentDateTime)
            {
                Doctor doctor = doctorService.GetAll().FirstOrDefault(d => d.Id == appointment.Doctor.Id && d.Specialization.ToString() == specialization);
                if (doctor != null)
                {
                    appointments.Add(appointment);
                    countAppointment++;
                    if (countAppointment == 5)
                        break;
                }       
            }
        }
        return appointments;
    }
    public void PostponeAppointment(Appointment appointment, DoctorService doctorService)
    {
        AvailabilityService availabilityService = new AvailabilityService();
        DateTime avaiableDoctorDateTime = DateTime.Now;
        GetDoctorsAppointments(appointment.Doctor);
        while (true)
        {
            if (availabilityService.IsDoctorAvailable(appointment.Doctor, avaiableDoctorDateTime))
            {
                if (appointment is Examination)
                {
                    var foundExamination = _allExaminations.FirstOrDefault(a => a.Id == appointment.Id);
                    Examination examination = foundExamination;
                    examination.DateTime = avaiableDoctorDateTime;
                    RemoveExamination(foundExamination);
                    AddExaminations(examination);
                    ObservableCollection<Examination> examinationCollection = new ObservableCollection<Examination>(_allExaminations);
                    ExaminationsToCsv(examinationCollection);
                    break;
                }
                else
                {
                    var foundOperation = _allOperations.FirstOrDefault(a => a.Id == appointment.Id);
                    Operation operation = foundOperation;
                    operation.DateTime = avaiableDoctorDateTime;
                    RemoveOperation(foundOperation);
                    AddOperations(operation);
                    ObservableCollection<Operation> operationCollection = new ObservableCollection<Operation>(_allOperations);
                    OperationsToCsv(operationCollection);
                    break;
                }
            }
            else
                avaiableDoctorDateTime = avaiableDoctorDateTime.AddMinutes(15);
        }
    }
}
