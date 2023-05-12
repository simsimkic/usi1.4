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

namespace ZdravoCorp.Views
{
    public partial class AddExamination : Window
    {
        public PatientService PatientService;
        public ExaminationsServices ExaminationService;
        public ObservableCollection<Examination> ExaminationList { get; set; }
        public Patient Patient;
        public AddExamination(PatientService patientService, ExaminationsServices examinationService, Patient patient,
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
        private void AddExamination_Click(object sender, RoutedEventArgs e)
        {
            DateTime dateBegin;
            string dateTimeBegin = textBox.Text;
            try
            {
                dateBegin = ParseDateTime(textBox.Text);
            }
            catch (ArgumentException ex)
            {
                ShowMessage("No Valid Date");
                return;
            }
            if (dateBegin < DateTime.Now)
            {
                ShowMessage("Selected date is in the past!");
                return;
            }
            Doctor doctorSelected = ExaminationService.GetDoctors().FirstOrDefault(doctor => doctor.FullName == comboBox.SelectedItem.ToString());
            bool created = ExaminationService.CreateAppointments(Patient, PatientService, dateBegin, doctorSelected, ExaminationList);
            if (Patient.Blocked)
                Application.Current.Shutdown();
            if (created)
                ShowMessage("Successfully created Examination");
            else
                ShowMessage("Creation failed");
            Close();
        }

        public DateTime ParseDateTime(string dateTimeString)
        {
            DateTime parsedDateTime;
            if (!DateTime.TryParseExact(dateTimeString, "yyyy-MM-dd HH:mm:ss", CultureInfo.InvariantCulture, DateTimeStyles.None, out parsedDateTime))
                throw new ArgumentException("Invalid date format");
            return parsedDateTime;
        }
        private void ShowMessage(string message)
        {
            MessageBox.Show(message);
        }
        private void CancelButton_Click(object sender, RoutedEventArgs e)
        {
            Close();
        }
    }
}
