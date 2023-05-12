using ZdravoCorp.Serialization;
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;
using System.Windows;
using ZdravoCorp.Models.Entities.Appointments;
using ZdravoCorp.Models.Entities.Users;
using ZdravoCorp.Models.Enumerations;
using ZdravoCorp.Models.Services.UserServices;
using ZdravoCorp.Models.Entities.Patients;

namespace ZdravoCorp.Models.Services.AppointmentServices;

public class ExaminationsServices
{
    public static List<ArchiveExamination> Archive=new List<ArchiveExamination>();
    public static List<Doctor> Doctors;
    public static List<Examination> Examinations = new List<Examination>();
    public static List<Examination> ExaminationsPatient = new List<Examination>();
    public ExaminationsServices()
    {
        List<Examination> examinations = ExaminationsFromCSV("..\\..\\..\\Data\\Appointments\\examinations.txt").ToList();
        List<ArchiveExamination> archive = ArchiveFromCSV("..\\..\\..\\Data\\Appointments\\examinationArchive.txt");
        Archive = archive;
        Examinations = examinations;
        List<Examination> examinationsPatient = new List<Examination>();
        ExaminationsPatient = examinationsPatient;
        Doctors = DoctorsFromCSV("..\\..\\..\\Data\\Users\\doctors.txt");
    }

    public static ObservableCollection<Examination> ExaminationsFromCSV(string filename)
    {
        Serializer<Examination> examinationSerializer = new Serializer<Examination>();
        List<Examination> examinationLoad = examinationSerializer.fromCSV(filename);
        ObservableCollection<Examination> examinationsObservableCollection = new ObservableCollection<Examination>(examinationLoad);
        return examinationsObservableCollection;
    }

    public static List<ArchiveExamination> ArchiveFromCSV(string filename)
    {
        Serializer<ArchiveExamination> archiveSerializer = new Serializer<ArchiveExamination>();
        List<ArchiveExamination> archiveLoad = archiveSerializer.fromCSV(filename);
        return archiveLoad;
    }

    public static void ArchiveExaminationsToCSV(List<ArchiveExamination> archive, string filename)
    {
        Serializer<ArchiveExamination> archiveSerializer = new Serializer<ArchiveExamination>();
        archiveSerializer.toCSV(filename, archive);
    }

    public bool CreateAppointments(Patient patient, PatientService patientServices, DateTime dateAppointments,
        Doctor doctor, ObservableCollection<Examination> observableCollection)
    {
        if (patient.Blocked)
            return false;
        if (!patientServices.IsPatientFree(patient, dateAppointments))
            return false;
        bool freeCreate = true;
        if (freeCreate)
        {
            Examination newExamination = CreateOneExamination(doctor, patient, dateAppointments);
            AddArchiveExamination(patient, newExamination, "a");
            AddExaminationInList(patient, newExamination, observableCollection);
            Patient blockedPatient = new Patient(patient);
            foreach (Patient patientInList in patientServices.GetPatients())
            {
                if (patientInList.Id == patient.Id)
                {
                    blockedPatient = new Patient(patientInList);
                    blockedPatient.ExaminationId.Add(newExamination.Id);
                    blockedPatient.Examinations.Add(newExamination);
                    break;
                }
            }
            bool neededBlocked = patientServices.NeedBlockPatient(patient, Archive);
            if (neededBlocked)
            {
                patient.Blocked = true;
                blockedPatient.Blocked = true;
                freeCreate = false;
            }
            UpdatePatientList(patientServices, patient, blockedPatient);
        }
        return freeCreate;
    }
    public bool CreateDoctorDatePriorityAppointments(Patient patient, PatientService patientServices, DateTime dateEndExamination, TimeSpan timeBeginExamination, TimeSpan timeEndExamination, Doctor doctor, ObservableCollection<Examination> observableCollection)
    {
        bool created = false;
        List<Examination> doctorSelectedExaminationsList = Examinations.Where(examination => examination.Doctor.Id == doctor.Id && examination.Status != AppointmentStatus.Canceled).ToList();
        DateTime dateTimeWithoutHours = StartExaminationDateTime(timeEndExamination);
        int daysBetweenEndAndToday = GetDaysBetweenDates(dateEndExamination);
        if (dateTimeWithoutHours.Date > dateEndExamination.Date)
            return false;
        for (int dayPlus = 0; dayPlus <= daysBetweenEndAndToday; dayPlus++)
        {
            List<TimeSpan> listTimeForThisDay = doctorSelectedExaminationsList.Where(examinationDoctor => examinationDoctor.DateTime.Date == dateTimeWithoutHours).Select(examinationDoctor => examinationDoctor.DateTime.TimeOfDay).ToList();
            listTimeForThisDay.Sort();
            if (listTimeForThisDay.Count == 0)
                return CheckAndCreateAppointmentEmptyList(patient, patientServices, dateTimeWithoutHours, timeBeginExamination, timeEndExamination, doctor, observableCollection, DateTime.Now);
            if (listTimeForThisDay.Count == 1)
                return CreateAppointmentInAvailableTimeSlot(patient, patientServices, dateTimeWithoutHours, timeBeginExamination, timeEndExamination, doctor, observableCollection, listTimeForThisDay[0]);
            created = CreateAppointmentInTimeInterval(patient, patientServices, dateTimeWithoutHours, timeBeginExamination, timeEndExamination, doctor, observableCollection, listTimeForThisDay);
            if (created)
                return created;
            DateTime newDate = dateTimeWithoutHours.AddDays(1);
            dateTimeWithoutHours = newDate;
        }
        return created;
    }

    public DateTime StartExaminationDateTime(TimeSpan timeBeginExamination)
    {
        DateTime dateTime = DateTime.Now.Date;
        if ((int)(DateTime.Now.TimeOfDay - timeBeginExamination).TotalMinutes > 0)
        {
            DateTime newDate = dateTime.AddDays(1);
            dateTime = newDate;
        }
        return dateTime;
    }
    public int GetDaysBetweenDates(DateTime dateEndExamination)
    {
        TimeSpan timeSpan = dateEndExamination - DateTime.Now;
        return timeSpan.Days;
    }

    public bool CheckAndCreateAppointmentEmptyList(Patient patient, PatientService patientServices, DateTime dateTime, TimeSpan timeBeginExamination, TimeSpan timeEndExamination, Doctor doctor, ObservableCollection<Examination> observableCollection, DateTime dateTimeEnd)
    {
        DateTime dateTimeTogetherDayTime = dateTime.Add(timeBeginExamination);
        bool created = CreateAppointments(patient, patientServices, dateTimeTogetherDayTime, doctor, observableCollection);
        return created;
    }

    public bool CheckAndCreateAppointmentDoctorPriorityEmptyList(Patient patient, PatientService patientServices, DateTime dateTime, Doctor doctor, ObservableCollection<Examination> observableCollection, DateTime dateTimeEnd, TimeSpan timeEndExamination)
    {
        DateTime dateTimeTogetherDayTime = dateTime.Add(timeEndExamination);
        bool created = CreateAppointments(patient, patientServices, dateTimeTogetherDayTime, doctor, observableCollection);
        return created;
    }

    public bool CreateAppointmentInAvailableTimeSlot(Patient patient, PatientService patientServices, DateTime dateTime, TimeSpan timeBeginExamination, TimeSpan timeEndExamination, Doctor doctor, ObservableCollection<Examination> observableCollection, TimeSpan timeInterval)
    {
        if (timeBeginExamination <= timeInterval && timeInterval <= timeEndExamination &&
            (int)(timeEndExamination - timeInterval).TotalMinutes >= 15)
        {
            DateTime dateTimeTogetherDayTime = dateTime.Add(timeInterval.Add(TimeSpan.FromMinutes(15)));
            bool created = CreateAppointments(patient, patientServices, dateTimeTogetherDayTime, doctor, observableCollection);
            return created;
        }
        if (timeBeginExamination <= timeInterval && timeInterval <= timeEndExamination &&
            (int)(timeBeginExamination - timeInterval).TotalMinutes <= -15)
        {
            DateTime dateTimeTogetherDayTime = dateTime.Add(timeBeginExamination);
            bool created = CreateAppointments(patient, patientServices, dateTimeTogetherDayTime, doctor, observableCollection);
            return created;
        }

        if (timeBeginExamination > timeInterval || timeEndExamination < timeInterval)
        {
            DateTime dateTimeTogetherDayTime = dateTime.Add(timeBeginExamination);
            bool created = CreateAppointments(patient, patientServices, dateTimeTogetherDayTime, doctor, observableCollection);
            return created;
        }

        return false;
    }

    public bool CreateAppointmentInTimeInterval(Patient patient, PatientService patientServices, DateTime dateTime,
        TimeSpan timeBeginExamination, TimeSpan timeEndExamination, Doctor doctor,
        ObservableCollection<Examination> observableCollection, List<TimeSpan> listTimeForThisDay)
    {
        bool lastElement = true;
        for (int i = 0; i < listTimeForThisDay.Count - 1; i++)
        {
            lastElement = false;
            if (timeBeginExamination <= listTimeForThisDay[i] && listTimeForThisDay[i] <= timeEndExamination &&
                (listTimeForThisDay[i + 1] - listTimeForThisDay[i]).TotalMinutes > 15 && listTimeForThisDay[i]>DateTime.Now.TimeOfDay)
            {
                listTimeForThisDay[i] = listTimeForThisDay[i].Add(new TimeSpan(0, 15, 0));
                DateTime dateTimeTogetherDayTime = dateTime.Add(listTimeForThisDay[i]);
                bool created = CreateAppointments(patient, patientServices, dateTimeTogetherDayTime, doctor, observableCollection);
                return created;
            }
        }
        if (lastElement == false)
        {
            if ((timeEndExamination - listTimeForThisDay[listTimeForThisDay.Count() - 1]).TotalMinutes >= 15)
            {
                TimeSpan endDate = listTimeForThisDay[listTimeForThisDay.Count() - 1].Add(TimeSpan.FromMinutes(15));
                DateTime dateTimeTogetherDayTime = dateTime.Add(endDate);
                bool created = CreateAppointments(patient, patientServices, dateTimeTogetherDayTime, doctor, observableCollection);
                return created;
            }
        }
        return false;
    }

    public bool CreateDoctorPriorityAppointments(Patient patient, PatientService patientServices, DateTime dateEndExamination, TimeSpan endTime, Doctor doctor, ObservableCollection<Examination> observableCollection)
    {
        bool created = false;
        List<Examination> doctorSelectedExaminationsList = Examinations.Where(examination => examination.Doctor.Id == doctor.Id && examination.Status != AppointmentStatus.Canceled).ToList();
        DateTime dateTimeWithoutHours = DateTime.Now.Date;
        if (DateTime.Now.AddMinutes(15).TimeOfDay > endTime && DateTime.Now.Date == dateTimeWithoutHours)
            return false;
        if (DateTime.Now.AddMinutes(15).TimeOfDay > endTime)
            dateTimeWithoutHours.AddDays(1);
        int daysBetweenEndAndToday = GetDaysBetweenDates(dateEndExamination);
        for (int dayPlus = 0; dayPlus <= daysBetweenEndAndToday; dayPlus++)
        {
            List<TimeSpan> listTimeForThisDay = doctorSelectedExaminationsList.Where(examinationDoctor => examinationDoctor.DateTime.Date == dateTimeWithoutHours).Select(examinationDoctor => examinationDoctor.DateTime.TimeOfDay).ToList();
            listTimeForThisDay.Sort();
            if (listTimeForThisDay.Count == 0)
                return CheckAndCreateAppointmentDoctorPriorityEmptyList(patient, patientServices, dateTimeWithoutHours, doctor, observableCollection, DateTime.Now.AddMinutes(15), endTime);
            if (listTimeForThisDay.Count == 1)
                return CreateAppointmentInAvailableTimeSlot(patient, patientServices, dateTimeWithoutHours, TimeSpan.Zero, new TimeSpan(23, 59, 0), doctor, observableCollection, listTimeForThisDay[0]);
            created = CreateAppointmentInTimeInterval(patient, patientServices, dateTimeWithoutHours, TimeSpan.Zero, new TimeSpan(23, 59, 0), doctor, observableCollection, listTimeForThisDay);
            if (created)
                return created;
            DateTime newDate = dateTimeWithoutHours.AddDays(1);
            dateTimeWithoutHours = newDate;
        }
        return created;
    }

    public List<Examination> FindTreeNextDoctorPriorityAppointments(Patient patient, PatientService patientServices, DateTime dateEndExamination, Doctor doctor, ObservableCollection<Examination> observableCollection)
    {
        int numberOfList = 0;
        List<Examination> treeNextEmainations = new List<Examination>();
        List<Examination> doctorSelectedExaminationsList = Examinations.Where(examination => examination.Doctor.Id == doctor.Id && examination.Status != AppointmentStatus.Canceled).ToList();
        DateTime dateTime = DateTime.Now.Date;
        for (int dayPlus = 0; dayPlus <= GetDaysBetweenDates(dateEndExamination); dayPlus++)
        {
            List<TimeSpan> listTimeForThisDay = doctorSelectedExaminationsList.Where(examinationDoctor => examinationDoctor.DateTime.Date == dateTime).Select(examinationDoctor => examinationDoctor.DateTime.TimeOfDay).ToList();
            listTimeForThisDay.Sort();
            if (listTimeForThisDay.Count == 0 || listTimeForThisDay.Count == 1)
            {
                numberOfList += 3;
                return SuggestedExaminations(doctor, patient, DateTime.Now);
            }
            for (int i = 0; i < listTimeForThisDay.Count - 1; i++)
            {
                if (numberOfList >= 3)
                    return treeNextEmainations;
                if ((listTimeForThisDay[i + 1] - listTimeForThisDay[i]).TotalMinutes > 15 && listTimeForThisDay[i] > DateTime.Now.TimeOfDay)
                {
                    DateTime dateTimeTogetherDayTime = dateTime.Add(listTimeForThisDay[i]);
                    if ((listTimeForThisDay[i + 1] - listTimeForThisDay[i]).TotalMinutes < 30)
                    {
                        numberOfList++;
                        Examination nextExamination = CreateOneExamination(doctor, patient, dateTimeTogetherDayTime);
                        treeNextEmainations.Add(nextExamination);
                    }
                    if ((listTimeForThisDay[i + 1] - listTimeForThisDay[i]).TotalMinutes < 45 && listTimeForThisDay[i] > DateTime.Now.TimeOfDay)
                        treeNextEmainations = AddNextExaminationsSuggestions(treeNextEmainations, doctor, patient, dateTimeTogetherDayTime, numberOfList);
                }
                else
                {
                    numberOfList += 3;
                    return SuggestedExaminations(doctor, patient, DateTime.Now);
                }
            }
            DateTime newDate = dateTime.AddDays(1);
            dateTime = newDate;
        }
        return treeNextEmainations;
    }

    public List<Examination> AddNextExaminationsSuggestions(List<Examination> examinations, Doctor doctor,
        Patient patient, DateTime dateTime, int numberOfList)
    {
        if (numberOfList == 2)
        {
            Examination nextExamination = CreateOneExamination(doctor, patient, dateTime);
            examinations.Add(nextExamination);
        }
        if (numberOfList == 1)
        {
            Examination nextExaminationFirst = CreateOneExamination(doctor, patient, dateTime);
            Examination nextExaminationSecond = CreateOneExamination(doctor, patient, nextExaminationFirst.DateTime.AddMinutes(15));
            examinations.Add(nextExaminationFirst);
            examinations.Add(nextExaminationSecond);
        }
        else
        {
            numberOfList += 3;
            return SuggestedExaminations(doctor, patient, dateTime);
        }
        return examinations;
    }

    public List<Examination> FindTreeNextDatePriorityAppointments(Patient patient, PatientService patientServices, DateTime dateEndExamination, TimeSpan timeBeginExamination, TimeSpan timeEndExamination, ObservableCollection<Examination> observableCollection)
    {
        List<Examination> treeBestExamination = new List<Examination>();
        DateTime dateTimeWithoutHours = StartExaminationDateTime(timeEndExamination);
        for (int dayPlus = 0; dayPlus <= GetDaysBetweenDates(dateEndExamination); dayPlus++)
        {
            List<TimeSpan> listTimeForThisDay = Examinations.Where(examination => examination.DateTime.Date == dateTimeWithoutHours.Date).Select(examination => examination.DateTime.TimeOfDay).ToList();
            List<int> differentMinutes = GetDifferentMinutes(listTimeForThisDay, timeBeginExamination, timeEndExamination);
            differentMinutes.Sort();
            differentMinutes.Reverse();
            if (differentMinutes.Count == 0)
                return SuggestedExaminations(Doctors[0], patient, dateTimeWithoutHours.Add(listTimeForThisDay[listTimeForThisDay.Count() - 1] + TimeSpan.FromMinutes(30)));
            if (differentMinutes.Count == 1)
                return SuggestedExaminations(Doctors[1], patient, dateTimeWithoutHours.Add(listTimeForThisDay[0]));
            if (differentMinutes.Count == 2)
                return SuggestedExaminationsDates(Doctors[2], patient, dateTimeWithoutHours.Add(listTimeForThisDay[listTimeForThisDay.Count() - 1] + TimeSpan.FromMinutes(30)), dateTimeWithoutHours.Add(listTimeForThisDay[listTimeForThisDay.Count() - 1] + TimeSpan.FromMinutes(45)), dateTimeWithoutHours.Add(listTimeForThisDay[listTimeForThisDay.Count() - 1] + TimeSpan.FromMinutes(60)));
            if (differentMinutes.Count >= 3)
            {
                int[] indexes = new int[] { differentMinutes.IndexOf(differentMinutes[0]), differentMinutes.IndexOf(differentMinutes[1]), differentMinutes.IndexOf(differentMinutes[2]) };
                DateTime result = dateTimeWithoutHours.Add(listTimeForThisDay[indexes[0]]);
                return SuggestedExaminationsDates(Doctors[0], patient,
                    dateTimeWithoutHours.Add(listTimeForThisDay[indexes[0]]),
                    dateTimeWithoutHours.Add(listTimeForThisDay[indexes[1]]),
                    dateTimeWithoutHours.Add(listTimeForThisDay[indexes[2]]));
            }
            DateTime newDate = dateTimeWithoutHours.AddDays(1);
            dateTimeWithoutHours = newDate;
        }
        return treeBestExamination;
    }
    public List<int> GetDifferentMinutes(List<TimeSpan> listTimeForThisDay, TimeSpan timeBeginExamination, TimeSpan timeEndExamination)
    {
        return listTimeForThisDay
            .Where((time, i) => i < listTimeForThisDay.Count - 1 && (listTimeForThisDay[i + 1] - time).TotalMinutes > 15).Select(time =>
            {
                if (time < timeBeginExamination)
                    return -(timeBeginExamination - time).Minutes;
                else if (time > timeEndExamination)
                    return -(time - timeEndExamination).Minutes;
                else
                    return 0;
            })
            .ToList();
    }

    public List<Examination> SuggestedExaminations(Doctor doctor, Patient patient, DateTime dateTime)
    {
        List<Examination> examinations = new List<Examination>();
        Random rand = new Random();
        uint randomNumber = (uint)rand.Next(1, 1001);
        Examination nextExaminationFirst = new Examination(randomNumber, doctor, patient, DateTime.Now.AddMinutes(30));
        Examination nextExaminationSecond = new Examination(randomNumber, doctor, patient, DateTime.Now.AddMinutes(45));
        Examination nextExaminationThird = new Examination(randomNumber, doctor, patient, DateTime.Now.AddMinutes(60));
        examinations.Add(nextExaminationFirst);
        examinations.Add(nextExaminationSecond);
        examinations.Add(nextExaminationThird);
        return examinations;
    }

    public List<Examination> SuggestedExaminationsDates(Doctor doctor, Patient patient, DateTime dateTimeFirst, DateTime dateTimeSecond, DateTime dateTimeThird)
    {
        List<Examination> examinations = new List<Examination>();
        Random rand = new Random();
        uint randomNumber = (uint)rand.Next(1, 1001);
        Examination nextExaminationFirst = new Examination(randomNumber, doctor, patient, dateTimeFirst);
        Examination nextExaminationSecond = new Examination(randomNumber, doctor, patient, dateTimeSecond);
        Examination nextExaminationThird = new Examination(randomNumber, doctor, patient, dateTimeThird);
        examinations.Add(nextExaminationFirst);
        examinations.Add(nextExaminationSecond);
        examinations.Add(nextExaminationThird);
        return examinations;
    }
    public Examination CreateOneExamination(Doctor doctor, Patient patient, DateTime dateTime)
    {
        Random rand = new Random();
        uint randomNumber = (uint)rand.Next(1, 1001);
        Examination examination = new Examination(randomNumber, doctor, patient, dateTime);
        return examination;
    }
    public bool CreateDatePriorityAppointments(Patient patient, PatientService patientServices,
        DateTime dateEndExamination, TimeSpan timeBeginExamination, TimeSpan timeEndExamination,
        ObservableCollection<Examination> observableCollection)
    {
        bool created = false;
        foreach (Doctor doctor in GetDoctors())
        {
            created = CreateDoctorDatePriorityAppointments(patient, patientServices, dateEndExamination, timeBeginExamination, timeEndExamination, doctor, observableCollection);
            if (created)
                return created;
        }
        return created;
    }

    public bool UpdateAppointments(Patient patient, PatientService patientServices, Examination selectedExamination, DateTime dateBegin, Doctor doctor, ObservableCollection<Examination> observableCollection)      //1day earlier
    {
        bool updated = true;
        if (patient.Blocked)
            return false;
        if (DateTime.Now < selectedExamination.DateTime.AddDays(-1))
        {
            selectedExamination.Status = AppointmentStatus.Canceled;
            selectedExamination.Doctor.Id = doctor.Id;
            Examination updateExamination = new Examination(selectedExamination.Id, doctor, selectedExamination.Patient, dateBegin);
            UpdateExamination(selectedExamination, updateExamination, observableCollection);
            AddArchiveExamination(patient, updateExamination, "u");
            bool neededBlocked = patientServices.NeedBlockPatient(patient, Archive);
            if (neededBlocked)
            {
                patient.Blocked = true;
                updated = false;
            }

            Patient blockedPatient = new Patient(patient);
            foreach (Patient patientInList in patientServices.GetPatients())
            {
                if (patientInList.Id == patient.Id)
                {
                    blockedPatient = new Patient(patientInList);
                    blockedPatient.Blocked = true;
                    break;
                }
            }
            UpdatePatientList(patientServices, patient, blockedPatient);
        }
        else
        {
            updated = false;
        }
        return updated;
    }

    public bool DeleteAppointments(Patient patient, PatientService patientServices, Examination selectedExamination)          //1day earlier
    {
        if (patient.Blocked)
            return false;
        bool deleted = true;
        if (DateTime.Now > selectedExamination.DateTime.AddDays(-1))
            deleted = false;
        else
        {
            AddArchiveExamination(patient, selectedExamination, "r");
            selectedExamination.Status = AppointmentStatus.Canceled;
            bool neededBlocked = patientServices.NeedBlockPatient(patient, Archive);
            if (neededBlocked)
            {
                patient.Blocked = true;
                deleted = false;
                Patient blockedPatient = new Patient(patient);
                foreach (Patient patientInList in patientServices.GetPatients())
                {
                    if (patientInList.Id == patient.Id)
                    {
                        blockedPatient = new Patient(patientInList);
                        blockedPatient.Blocked = true;
                        break;
                    }
                }
                UpdatePatientList(patientServices, patient, blockedPatient);
                TurnOffApplication();
            }
        }
        return deleted;
    }
    public void AddArchiveExamination(Patient patient, Examination selectedExamination, string function)
    {
        ArchiveExamination archiveExamination = new ArchiveExamination(selectedExamination, function, DateTime.Now, patient.Username);
        Archive.Add(archiveExamination);
        ArchiveExaminationsToCSV(Archive, "..\\..\\..\\Data\\Appointments\\examinationArchive.txt");
    }
    public void UpdatePatientList(PatientService patientService, Patient oldPatient, Patient newPatient)
    {
        patientService.AddPatients(newPatient);
        patientService.RemovePatients(oldPatient);
        patientService.PatientsToCSV(patientService.GetPatients(), "..\\..\\..\\Data\\Users\\Patients\\patients.txt");
    }

    public void UpdateExamination(Examination selectedExamination, Examination updateExamination, ObservableCollection<Examination> observableCollection)
    {
        observableCollection.Remove(selectedExamination);
        ExaminationsPatient.Remove(selectedExamination);
        ExaminationsPatient.Add(updateExamination);
        observableCollection.Add(updateExamination);
        ExaminationsToCSV(observableCollection, "..\\..\\..\\Data\\Appointments\\examinations.txt");
    }

    public void AddExaminationInList(Patient patient, Examination examination, ObservableCollection<Examination> observableCollection)
    {
        patient.ExaminationsMap[DateTime.Now] = true;
        Examinations.Add(examination);
        observableCollection.Add(examination);
        ExaminationsToCSV(observableCollection, "..\\..\\..\\Data\\Appointments\\examinations.txt");
        patient.Examinations.Add(examination);
    }

    public void TurnOffApplication()
    {
        Application.Current.Shutdown();
    }

    public List<Examination> GetExaminations()
    {
        return Examinations;
    }
    public void Add(Examination objNewExamination)
    {
        Examinations.Add(objNewExamination);
    }
    public List<ArchiveExamination> GetArchiveExaminations()
    {
        return Archive;
    }
    public List<Examination> GetExaminationsPatient(Patient patient)
    {
        return patient.Examinations;
    }
    public List<Examination> GetExaminationsById(uint Id)
    {
        List<Examination> examinations = new List<Examination>();
        foreach (Examination examination in Examinations)
        {
            if (examination.Patient.Id == Id)
            {
                examinations.Add(examination);
            }
        }
        return examinations;
    }
    public void loadPatient(PatientService patientService)
    {
        List<Patient> patientsLoad = patientService.GetPatients();
        foreach (Patient patientInList in patientsLoad)
        {
            foreach (Examination examinationPatient in patientInList.Examinations)
            {
                foreach (Examination examination in Examinations)
                {
                    if (examination.Id == examinationPatient.Id)
                    {
                        examination.Patient = patientInList;
                    }
                }
            }
        }
    }

    public static void ExaminationsToCSV(ObservableCollection<Examination> patients, string filename)
    {
        List<Examination> examinations = Examinations;
        if (patients.Count > 0)
        {
            Serializer<Examination> patientsSerializer = new Serializer<Examination>();
            patientsSerializer.toCSV(filename, examinations);
        }
    }
    public void ToCSVList(List<Examination> examinations, string filename)
    {
            Serializer<Examination> patientsSerializer = new Serializer<Examination>();
            patientsSerializer.toCSV(filename,examinations);
    }
    public static List<Doctor> DoctorsFromCSV(string filename)
    {
        Serializer<Doctor> doctorSerializer = new Serializer<Doctor>();
        List<Doctor> doctorsAll = doctorSerializer.fromCSV(filename);
        return doctorsAll;
    }
    public List<Doctor> GetDoctors()
    {
        return Doctors;
    }
    public List<string> GetSpecialization()
    {
        List<string> specialization = new List<string>();
        Array specializationValues = Enum.GetValues(typeof(Specialization));
        foreach (Specialization specializationValue in specializationValues)
            if(specializationValue != Specialization.None)
                specialization.Add(specializationValue.ToString());
        return specialization;
    }

    public List<string> DoctorsToStringFormat()
    {
        return Doctors?.Select(doctor => doctor.FullName).ToList() ?? new List<string>();
    }

    public List<MedicalProfile> FindMedicalProfilesFromExaminationsAndAnamnesis(ObservableCollection<Anamnesis> anamnesisList)
    {
        List<MedicalProfile> medicalProfiles = new List<MedicalProfile>();
        foreach (Examination examination in Examinations)
        {
            Anamnesis anamnesis = anamnesisList.FirstOrDefault(a => a.ExaminationId == examination.Id);
            if (anamnesis != null)
            {
                Doctor doctor = ExaminationsServices.Doctors.FirstOrDefault(d => d.Id == examination.Doctor.Id);

                if (doctor != null)
                {
                    MedicalProfile medicalProfile = new MedicalProfile(anamnesis.ExaminationId, examination.DateTime, doctor.FullName, doctor.Specialization.ToString(), anamnesis.Symptoms, anamnesis.Disease, anamnesis.Allergens);
                    medicalProfiles.Add(medicalProfile);
                }
            }
        }
        return medicalProfiles;
    }
}