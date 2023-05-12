using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;
using ZdravoCorp.Serialization;
using ZdravoCorp.Models.Entities.Appointments;
using ZdravoCorp.Models.Enumerations;
using ZdravoCorp.Models.Entities.Users;
using System;
using ZdravoCorp.Models.Services.AppointmentServices;
using System.Windows;

namespace ZdravoCorp.Models.Services.UserServices;

public class DoctorService {
    private static List < Doctor > _allDoctors;

    public DoctorService() {
        _allDoctors = new List <Doctor> ();
        _allDoctors = DoctorsFromCsv("..\\..\\..\\Data\\Users\\doctors.txt").ToList();
    }
    public List < Doctor > GetAll() {
        return _allDoctors;
    }
    public void Add(Doctor newDoctor) {
        _allDoctors.Add(newDoctor);
    }
    private static ObservableCollection < Doctor > DoctorsFromCsv(string filename) {
        var doctorSerializer = new Serializer < Doctor > ();
        var doctors = doctorSerializer.fromCSV(filename);
        return new ObservableCollection < Doctor > (doctors);
    }
    public void DoctorsToCsv(ObservableCollection < Doctor > doctors, string filename) {
        var doctorsToSave = doctors.ToList();
        var doctorSerializer = new Serializer < Doctor > ();
        doctorSerializer.toCSV(filename, doctorsToSave);
    }
    public List < Appointment > CombineExaminationsAndOperations(List < Examination > examinations, List < Operation > operations) {
        List < Appointment > combinedAppointments = new List < Appointment > (examinations);
        combinedAppointments.AddRange(operations);
        return combinedAppointments;
    }
    public Doctor GetSpecializedDoctorForDateRange(string specialization, AppointmentService appointmentService, string appointment, Patient patient, uint operationDuration, ExaminationsServices examinationService,PatientService patientService)
    {
        Doctor doctor = new Doctor();
        AvailabilityService availabilityService = new AvailabilityService();

        foreach (Doctor d in _allDoctors)
        {
            DateTime currentDateTime = DateTime.Now;
            DateTime dateTimeAfter2Hours = DateTime.Now.AddHours(2);
            appointmentService.GetDoctorsAppointments(d);
            if (d.Specialization.ToString() == specialization)
            {
                while (currentDateTime < dateTimeAfter2Hours)
                {
                    if (availabilityService.IsDoctorAvailable(d, currentDateTime))
                    {
                        doctor = d;
                        Random random = new Random();
                        uint randomNumber = (uint)random.Next(100, 1000);
                        if (appointment == "Examination")
                        {
                            Examination examination = new Examination(randomNumber, doctor, new Patient(patient.Id), currentDateTime.AddMinutes(5));
                            appointmentService.AddExaminations(examination);
                            doctor.Examinations.Add(examination);
                            examinationService.Add(examination);
                            examinationService.ToCSVList(examinationService.GetExaminations(), "..\\..\\..\\Data\\Appointments\\examinations.txt");

                            patientService.AddExaminationToPatient(patient,examination);
                        }
                        else
                        {
                            Operation operation = new Operation(randomNumber, doctor, new Patient(patient.Id), currentDateTime, operationDuration);
                            appointmentService.AddOperations(operation);
                            doctor.Operations.Add(operation);
                            AppointmentService.OperationsToCsv(new ObservableCollection<Operation>(appointmentService.GetAllOperations()));
                        }
                        return doctor;
                    }
                    else
                        currentDateTime = currentDateTime.AddMinutes(5);
                }}}
        return doctor;
    }
}