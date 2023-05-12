using System.Collections.ObjectModel;
using System.Linq;
using System.Windows;
using System.Windows.Input;
using ZdravoCorp.Models.Entities.Patients;
using ZdravoCorp.Models.Entities.Users;
using ZdravoCorp.Models.Services.AppointmentServices;
using ZdravoCorp.Models.Services.PatientServices;
using ZdravoCorp.Models.Services.UserServices;
using ZdravoCorp.Views.NurseViews;

namespace ZdravoCorp.Views
{
    /// <summary>
    /// Interaction logic for PatientProfile.xaml
    /// </summary>
    public partial class PatientProfile : Window
    {
        public ObservableCollection<Patient> PatientsList { get; set; }
        public ObservableCollection<MedicalRecord> RecordsList { get; set; }
        public Patient SelectedPatient { get; set; }
        public PatientService PatientService { get; set; }
        public ExaminationsServices ExaminationService { get; set; }
        public MedicalRecordService MedicalRecordService { get; set; }
        public AnamnesisService AnamnesisService { get; set; }
        public PatientProfile()
        {
            InitializeComponent();
            this.DataContext = this;

            PatientService patientService = new PatientService();
            PatientService = patientService;

            MedicalRecordService recordService = new MedicalRecordService();
            MedicalRecordService = recordService;

            ExaminationsServices examinationService = new ExaminationsServices();
            ExaminationService = examinationService;

            //convert List to ObservableCollection
            PatientsList = new ObservableCollection<Patient>(PatientService.GetAll());
            RecordsList = new ObservableCollection<MedicalRecord>(MedicalRecordService.GetAll());

            AnamnesisService anamnesisService = new AnamnesisService();
            AnamnesisService = anamnesisService;

            patientsDataGrid.ItemsSource = PatientsList;
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
            }}
        private void Border_MouseDown(object sender, MouseButtonEventArgs e)
        {
            if (e.ChangedButton == MouseButton.Left)
            {
                this.DragMove();
            }}
        private void Button_Click_Add_Patient(object sender, RoutedEventArgs e)
        {
            CreatePatient createPatient = new CreatePatient(PatientsList,RecordsList, PatientService, MedicalRecordService);
            createPatient.Show();
        }

        private void Button_Click_Delete_Patient(object sender, RoutedEventArgs e)
        {
            if (SelectedPatient == null)
                MessageBox.Show("You must select a patient");
            else
            {
                var patientToRemove = PatientsList.FirstOrDefault(p => p.FirstName == SelectedPatient.FirstName && p.Id == SelectedPatient.Id && p.LastName == SelectedPatient.LastName);
                var recordToRemove = RecordsList.FirstOrDefault(mr => mr.Id == SelectedPatient.Id);
               
                if (patientToRemove != null && recordToRemove!=null)
                {
                    PatientsList.Remove(patientToRemove);
                    RecordsList.Remove(recordToRemove);
                }

                PatientService.PatientsToCSV(PatientsList, "..\\..\\..\\Data\\Users\\Patients\\patients.txt");
                MedicalRecordService.MedicalRecordToCSV(RecordsList, "..\\..\\..\\Data\\Users\\Patients\\medicalrecords.txt");
            }
        }
        private void Button_Click_Update_Patient(object sender, RoutedEventArgs e)
        {
            if (SelectedPatient == null)
                MessageBox.Show("You must select a patient");
            else
            {
                UpdatePatient updatePatient = new UpdatePatient(SelectedPatient,PatientsList, RecordsList, PatientService, MedicalRecordService);
                updatePatient.Show();
            }
        }

        private void Admission_Patients(object sender, RoutedEventArgs e)
        {
            if (SelectedPatient == null)
                MessageBox.Show("You must select a patient");
            else
            {
                PatientAdmission patientAdmission = new PatientAdmission(PatientService, ExaminationService,AnamnesisService, SelectedPatient);
                patientAdmission.Show();
                Close();
            }
        }

        private void EmergencyBookingButtonClick(object sender, RoutedEventArgs e)
        {
            EmergencyScheduler emergencyScheduler = new EmergencyScheduler(PatientService, ExaminationService);
            emergencyScheduler.Show();
            Close();
        }

        private void Logout_ButtonClick(object sender, RoutedEventArgs e)
        {
            MainWindow logout = new MainWindow();
            logout.Show();
            Close();
        }
    }
}