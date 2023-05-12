using System.Collections.ObjectModel;
using System.Linq;
using System.Windows;
using System.Windows.Input;
using ZdravoCorp.Models.Entities.Patients;
using ZdravoCorp.Models.Entities.Users;
using ZdravoCorp.Models.Enumerations;
using ZdravoCorp.Models.Services.PatientServices;
using ZdravoCorp.Models.Services.UserServices;

namespace ZdravoCorp.Views.DoctorViews
{
    public partial class DoctorPatientView : Window
    {
        public Doctor Doctor { get; set; }
        public Patient SelectedPatient { get; set; }
        public PatientService PatientService { get; set; }
        public ObservableCollection<Patient> PatientsList { get; set; }
        public ObservableCollection<MedicalRecord> RecordsList { get; set; }

        public DoctorPatientView(Doctor doctor)
        {
            InitializeComponent();
            this.DataContext = this;

            Doctor = doctor;

            PatientService = new PatientService();

            //convert List to ObservableCollection
            PatientsList = new ObservableCollection<Patient>(PatientService.GetAll());
            
            patientsDataGrid.ItemsSource = PatientsList;
        }

        private void Logout_ButtonClick(object sender, RoutedEventArgs e)
        {
            MainWindow logout = new MainWindow();
            logout.Show();
            Close();
        }

        private void AllExaminationsButton_Click(object sender, RoutedEventArgs e)
        {
            DoctorExaminationView examinationView = new DoctorExaminationView(Doctor);
            examinationView.Show();
            Close();
        }

        private void AllOperationsButton_Click(object sender, RoutedEventArgs e)
        {
            DoctorOperationView operationView = new DoctorOperationView(Doctor);
            operationView.Show();
            Close();
        }
        
        private void EditMedicalRecordButton_Click(object sender, RoutedEventArgs e)
        {
            if (SelectedPatient == null)
            {
                MessageBox.Show("You must select a patient");
            }
            else if (DoctorExaminedPatient())
            {
                UpdateMedicalRecord updateMedicalRecord = new UpdateMedicalRecord(SelectedPatient);
                updateMedicalRecord.Show();
            }
            else
            {
                MessageBox.Show("You never examined this patient, so you cant look into his/hers medical record");
            }
        }

        private bool DoctorExaminedPatient()
        {
            return Doctor.Examinations.Any(examination => examination.Patient.Id == SelectedPatient.Id && examination.Status==AppointmentStatus.Completed);
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

        private void Border_MouseDown(object sender, MouseButtonEventArgs e)
        {
            if (e.ChangedButton == MouseButton.Left)
            {
                this.DragMove();
            }
        }
    }
}