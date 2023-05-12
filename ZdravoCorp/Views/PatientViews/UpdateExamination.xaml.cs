using ZdravoCorp.Models.Services.PatientServices;
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Globalization;
using System.Windows;
using System.Windows.Controls;
using ZdravoCorp.Models.Entities.Appointments;
using ZdravoCorp.Models.Entities.Users;
using ZdravoCorp.Models.Services.AppointmentServices;
using ZdravoCorp.Models.Services.UserServices;
using System.Linq;

namespace ZdravoCorp.Views
{
    public partial class UpdateExamination : Window
    {
        public List<string> MyList { get; set; }
        public PatientService PatientServices;
        public ExaminationsServices ExaminationServices;
        public ObservableCollection<Examination> ExaminationList { get; set; }
        public Patient Patient;
        public Examination SelectedExamination { get; set; }

        public UpdateExamination(PatientService patientService, ExaminationsServices examinationService, Patient patient,
            Examination selectedExamination, ObservableCollection<Examination> examinationList)
        {
            InitializeComponent();
            Patient = patient;
            PatientServices = patientService;
            ExaminationServices = examinationService;
            SelectedExamination = selectedExamination;
            ExaminationList = examinationList;
            string doctorSelected = "";
            foreach (Doctor doctor in ExaminationServices.GetDoctors())
            {
                doctorSelectionComboBox.Items.Add(doctor.FullName);
                if (selectedExamination.Doctor.Id == doctor.Id)
                    doctorSelected = doctor.FullName;
            }
            textBox.Text = SelectedExamination.DateTime.ToString("yyyy-MM-dd HH:mm:ss");
            doctorSelectionComboBox.SelectedItem = doctorSelected;
            DataContext = this;
        }
        private void UpdateExaminationButton_Click(object sender, RoutedEventArgs e)
        {
            if (SelectedExamination == null)
                ShowMessage("You must select a patient");
            else
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
                    ShowMessage("That date was before now!");
                    return;
                }
                Doctor selectedDoctor = ExaminationServices.GetDoctors().FirstOrDefault(doctor => doctor.FullName == doctorSelectionComboBox.SelectedItem.ToString());
                bool updated = ExaminationServices.UpdateAppointments(Patient, PatientServices, SelectedExamination, dateBegin, selectedDoctor, ExaminationList);
                if (updated)
                    ShowMessage("Successfully updated Examination");
                else
                {
                    if (PatientServices.NeedBlockPatient(Patient, ExaminationServices.GetArchiveExaminations()))
                        Application.Current.Shutdown();
                    ShowMessage("Update failed");
                }
                Close();
            }
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
