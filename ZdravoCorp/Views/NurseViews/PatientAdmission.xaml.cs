using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Windows;
using ZdravoCorp.Models.Entities.Appointments;
using ZdravoCorp.Models.Entities.Patients;
using ZdravoCorp.Models.Entities.Users;
using ZdravoCorp.Models.Enumerations;
using ZdravoCorp.Models.Services.AppointmentServices;
using ZdravoCorp.Models.Services.PatientServices;
using ZdravoCorp.Models.Services.UserServices;
using ZdravoCorp.Views.NurseViews;

namespace ZdravoCorp.Views
{
    public partial class PatientAdmission : Window
    {
        public ObservableCollection<Examination> ExaminationList { get; set; }
        public static PatientService PatientService { get; set; }
        public static ExaminationsServices ExaminationService { get; set; }
        public AnamnesisService AnamnesisService { get; set; }

        //most copied from a colleague who worked with the patient and his function
        public PatientAdmission(PatientService patientService, ExaminationsServices examinationService,
            AnamnesisService anamnesisService, Patient patient)
        {
            InitializeComponent();
            PatientService = patientService;
            ExaminationService = examinationService;
            AnamnesisService = anamnesisService;
            ExaminationList = new ObservableCollection<Examination>(examinationService.GetExaminationsById(patient.Id));
            examinationDataGrid.ItemsSource = ExaminationList;
        }
        private void PatientProfileButton_Click(object sender, RoutedEventArgs e)
        {
            PatientProfile patientProfile = new PatientProfile();
            patientProfile.Show();
            Close();
        }

        private void AddAnamnesisButton_Click(object sender, RoutedEventArgs e)
        {
            if (examinationDataGrid.SelectedItem == null)
                MessageBox.Show("You must select a examination");
            else
            {
                Examination examination = (Examination)examinationDataGrid.SelectedItem;
                TimeSpan difference = examination.DateTime - DateTime.Now;

                List<Anamnesis> anamneses = AnamnesisService.GetAll();
                List<uint> examinationsWithAnamnesis = new List<uint>();
                foreach (Anamnesis anamnesis in anamneses)
                    examinationsWithAnamnesis.Add(anamnesis.ExaminationId);

                if (examinationsWithAnamnesis.Contains(examination.Id))
                    MessageBox.Show("Anamnesis has already been created for this examination");
                else if (examination.Status == AppointmentStatus.Canceled)
                    MessageBox.Show("Examination is canceled");
                else if (difference.Days < 0 || difference.Hours < 0 || difference.Minutes < 0 || difference.Seconds < 0)
                    MessageBox.Show("The date has expired");
                else if (difference.Days>0 || (difference.Days ==0 && difference.Hours>0) || (difference.Days == 0 && difference.Hours ==0 && difference.Minutes > 15))
                    MessageBox.Show("The anamnesis can start no earlier than 15 minutes before the examination");
                else
                {
                    CreateAnamnesis createAnamnesis = new CreateAnamnesis(examination, AnamnesisService);
                    createAnamnesis.Show();
                }
            }
        }
    }
}
