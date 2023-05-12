using System;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.InteropServices;
using System.Windows;
using System.Windows.Input;
using System.Collections.ObjectModel;
using ZdravoCorp.Models.Entities.Appointments;
using ZdravoCorp.Models.Entities.Patients;
using ZdravoCorp.ViewModels.PatientViewModels;
using ZdravoCorp.Models.Entities.Users;
using ZdravoCorp.Models.Services.AppointmentServices;
using ZdravoCorp.Models.Services.PatientServices;
using ZdravoCorp.Models.Services.UserServices;
using ZdravoCorp.Views.PatientViews;

namespace ZdravoCorp.Views
{
    public partial class MyExams : Window
    {
        public ObservableCollection<Examination> ExaminationList { get; set; }
        public static PatientService PatientService { get; set; }
        public static ExaminationsServices ExaminationService { get; set; }

        public Examination SelectedExamination { get; set; }

        public static Patient Patient { get; set; }

        public AnamnesisService AnamnesisService { get; set; }

        public MedicalRecordService MedicalRecordService { get; set; }

        private PatientViewModel viewModelPatient;
        private ExaminationViewModel viewModelExamination;
        public MyExams(PatientService patientService, ExaminationsServices examinationService, Patient patient)
        {
            InitializeComponent();
            PatientService = patientService;
            ExaminationService = examinationService;
            Patient = patient;
            AnamnesisService = new AnamnesisService();
            MedicalRecordService = new MedicalRecordService();

            InitializeExaminationList();
            InitializeViewModels();
        }
        private bool IsMaximize = false;
        private void Border_MouseLeftButtonDown(object sender, MouseButtonEventArgs e)
        {
            if (e.ClickCount == 2)
            {
                if (IsMaximize)
                {
                    this.WindowState = WindowState.Normal;
                    this.Width = 1080;
                    this.Height = 720;

                    IsMaximize = false;
                }
                else
                {
                    this.WindowState = WindowState.Maximized;

                    IsMaximize = true;
                }
            }
        }

        private void InitializeExaminationList()
        {
            ExaminationList = new ObservableCollection<Examination>(ExaminationService.GetExaminationsPatient(Patient));
            ExaminationsDataGrid.ItemsSource = ExaminationList;
        }
        private void InitializeViewModels()
        {
            viewModelPatient = new PatientViewModel(ExaminationService.GetExaminationsPatient(Patient));
            viewModelExamination = new ExaminationViewModel();
        }
        private void Border_MouseDown(object sender, MouseButtonEventArgs e)
        {
            if (e.ChangedButton == MouseButton.Left)
            {
                this.DragMove();
            }
        }

        private void AddExamination_OnClick(object sender, RoutedEventArgs e)
        {
            AddExamination addExamination = new AddExamination(PatientService, ExaminationService, Patient, ExaminationList);
            addExamination.Show();
        }

        private void AddPriorityExamination_OnClick(object sender, RoutedEventArgs e)
        {
            AddPriorityExamination addPriorityExamination = new AddPriorityExamination(PatientService, ExaminationService, Patient, ExaminationList);
            addPriorityExamination.Show();
        }

        private void UpdateExamination_OnClick(object sender, RoutedEventArgs e)
        {
            if ((Examination)ExaminationsDataGrid.SelectedItem != null)
            {
                UpdateExamination updateExamination = new UpdateExamination(PatientService, ExaminationService, Patient, (Examination)this.ExaminationsDataGrid.SelectedItem, ExaminationList);
                updateExamination.Show();
            }
        }

        private void Button_Click_Delete_Examination(object sender, RoutedEventArgs e)
        {
            if (ExaminationsDataGrid.SelectedItem == null)
                MessageBox.Show("You must select a patient");
            else
            {
                var examinationToRemove = ExaminationList.FirstOrDefault(ex => ex.Id == ((Examination)ExaminationsDataGrid.SelectedItem).Id);
                if (examinationToRemove != null)
                {
                    if (ExaminationService.DeleteAppointments(Patient, PatientService, (Examination)ExaminationsDataGrid.SelectedItem))
                    {
                        ExaminationList.Remove(examinationToRemove);
                    }
                }
                ExaminationsServices.ExaminationsToCSV(ExaminationList, "..\\..\\..\\Data\\Appointments\\examinations.txt");
            }
        }
        private void MedicalRecordPatientButton_Click(object sender, RoutedEventArgs e)
        {
            MedicalRecord medicalRecord = MedicalRecordService.GetAll().FirstOrDefault(mr => mr.Id == Patient.Id);

            if (medicalRecord != null)
            {
                MedicalRecordPatient medicalRecordPatient = new MedicalRecordPatient(Patient, medicalRecord);
                medicalRecordPatient.Show();
            }
        }
        private void ViewExaminationAnamnesisButton_Click(object sender, RoutedEventArgs e)
        {
            if ((Examination)ExaminationsDataGrid.SelectedItem != null)
            {
                uint examinationId = ((Examination)ExaminationsDataGrid.SelectedItem).Id;
                List<Anamnesis> anamnesisForExamination = AnamnesisService.GetAll().Where(anamnesis => anamnesis.ExaminationId == examinationId).ToList();
                if (anamnesisForExamination.Count > 0)
                {
                    ShowExaminationAnamnesis showExaminationAnamnesis = new ShowExaminationAnamnesis(anamnesisForExamination[0]);
                    showExaminationAnamnesis.Show();
                }
            }
        }
        private void AllAnamnesisButton_Click(object sender, RoutedEventArgs e)
        {
            List<Anamnesis> listOfAnamnesisPatient = AnamnesisService.GetAnamnesisForPatient(Patient, ExaminationService);
            ObservableCollection<Anamnesis> observableListOfAnamnesisPatient = new ObservableCollection<Anamnesis>(listOfAnamnesisPatient);
            Console.WriteLine(listOfAnamnesisPatient.Count);
            ShowAllAnamnesis showAnamnesis = new ShowAllAnamnesis(observableListOfAnamnesisPatient, ExaminationService);
            showAnamnesis.Show();
        }
    }


    public partial class App : Application
    {
        [DllImport("kernel32.dll", SetLastError = true)]
        [return: MarshalAs(UnmanagedType.Bool)]
        static extern bool AllocConsole();

        protected override void OnStartup(StartupEventArgs e)
        {
            base.OnStartup(e);
            AllocConsole();
            Console.WriteLine("Console is visible");
        }
    }
}
