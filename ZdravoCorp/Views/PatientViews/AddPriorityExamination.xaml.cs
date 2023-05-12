using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Globalization;
using System.Linq;
using System.Windows;
using ZdravoCorp.Models.Entities.Appointments;
using ZdravoCorp.Models.Entities.Users;
using ZdravoCorp.Models.Services.AppointmentServices;
using ZdravoCorp.Models.Services.UserServices;

namespace ZdravoCorp.Views.PatientViews
{
    public partial class AddPriorityExamination : Window
    {
        public PatientService PatientService;
        public ExaminationsServices ExaminationService;
        public ObservableCollection<Examination> ExaminationList { get; set; }
        public Patient Patient;
        public AddPriorityExamination(PatientService patientService, ExaminationsServices examinationService, Patient patient,
            ObservableCollection<Examination> observableCollection)
        {
            InitializeComponent();
            Patient = patient;
            PatientService = patientService;
            ExaminationService = examinationService;
            ExaminationList = observableCollection;
            foreach (Doctor doctor in ExaminationService.GetDoctors())
            {
                comboBox.Items.Add(doctor.FullName);
            }
            DataContext = this;
        }

        private void AddExaminationPriority_Click(object sender, RoutedEventArgs e)
        {
            DateTime dateEndExamination;
            DateTime dateTimeBeginExamination;
            DateTime dateTimeEndExamination;
            TimeSpan timeBeginExamination;
            TimeSpan timeEndExamination;
            string dateTimeEndTextBox = textBoxDateEnd.Text;
            string timeBeginTextBox = textBoxTimeBegin.Text;
            string timeEndTextBox = textBoxTimeEnd.Text;
            if (!DateTime.TryParseExact(dateTimeEndTextBox, "yyyy-MM-dd", CultureInfo.InvariantCulture,
                    DateTimeStyles.None, out dateEndExamination))
            {
                MessageBox.Show("No valid date!");
                return;
            }
            if (!DateTime.TryParseExact(timeBeginTextBox, "HH:mm:ss", CultureInfo.InvariantCulture,
                    DateTimeStyles.None, out dateTimeBeginExamination))
            {
                MessageBox.Show("No valid time in time begin!");
                return;
            }

            timeBeginExamination = dateTimeBeginExamination.TimeOfDay;
            if (!DateTime.TryParseExact(timeEndTextBox, "HH:mm:ss", CultureInfo.InvariantCulture,
                    DateTimeStyles.None, out dateTimeEndExamination))
            {
                MessageBox.Show("No valid time in time end!");
                return;
            }
            timeEndExamination = dateTimeEndExamination.TimeOfDay;
            if (RadioButtonPriorityDoctor.IsChecked == false && RadioButtonPriorityDate.IsChecked == false)
            {
                MessageBox.Show("No Selected Priority!");
                return;
            }
            var doctorSelected = ExaminationService.GetDoctors().FirstOrDefault(doctor => doctor.FullName == comboBox.SelectedItem?.ToString());
            if (doctorSelected == null)
            {
                MessageBox.Show("No doctor selected!");
                return;
            }
            bool createdAppointment = false;
            bool createdAppointmentDoctor = false;
            bool createdAppointmentDate = false;
            createdAppointment = ExaminationService.CreateDoctorDatePriorityAppointments(Patient, PatientService, dateEndExamination, timeBeginExamination, timeEndExamination, doctorSelected, ExaminationList);
            if (!createdAppointment)
            {
                if (RadioButtonPriorityDoctor.IsChecked == true)
                {
                    createdAppointmentDoctor = ExaminationService.CreateDoctorPriorityAppointments(Patient, PatientService, dateEndExamination, timeEndExamination, doctorSelected, ExaminationList);
                    if (!createdAppointmentDoctor)
                    {
                        List<Examination> listCreatedAppointmentDoctor = new List<Examination>();
                        listCreatedAppointmentDoctor = ExaminationService.FindTreeNextDoctorPriorityAppointments(Patient, PatientService, dateEndExamination, doctorSelected, ExaminationList);
                        SuggestedExaminations suggestedExaminations = new SuggestedExaminations(listCreatedAppointmentDoctor, ExaminationService, PatientService, ExaminationList);
                        suggestedExaminations.Show();
                    }
                }
                else
                {
                    createdAppointmentDate = ExaminationService.CreateDatePriorityAppointments(Patient, PatientService, dateEndExamination, timeBeginExamination, timeEndExamination, ExaminationList);
                    if (!createdAppointmentDate)
                    {
                        List<Examination> listCreatedAppointmentDate = new List<Examination>();
                        listCreatedAppointmentDate = ExaminationService.FindTreeNextDatePriorityAppointments(Patient, PatientService, dateEndExamination, timeBeginExamination, timeEndExamination, ExaminationList);
                        SuggestedExaminations suggestedExaminations = new SuggestedExaminations(listCreatedAppointmentDate, ExaminationService, PatientService, ExaminationList);
                        suggestedExaminations.Show();
                    }
                }
                if (Patient.Blocked)
                    Application.Current.Shutdown();
            }
            if (createdAppointment)
                MessageBox.Show("Successfully created Examination");
            Close();
        }
        private void CancelButton_Click(object sender, RoutedEventArgs e)
        {
            Close();
        }
    }
}
