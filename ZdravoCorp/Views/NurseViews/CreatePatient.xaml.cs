using System.Collections.ObjectModel;
using System.Windows;
using ZdravoCorp.Models.Entities.Patients;
using ZdravoCorp.Models.Entities.Users;
using ZdravoCorp.Models.Services.PatientServices;
using ZdravoCorp.Models.Services.UserServices;

namespace ZdravoCorp.Views
{
    /// <summary>
    /// Interaction logic for CreatePatient.xaml
    /// </summary>
    public partial class CreatePatient : Window
    {
        public Patient Patient { get; set; }
        public MedicalRecord MedicalRecord { get; set; }
        public ObservableCollection<Patient> PatientsList { get; }
        public ObservableCollection<MedicalRecord> RecordsList { get; }
        public PatientService PatientService { get; set; }
        public MedicalRecordService MedicalRecordService { get; set; }
        
        public CreatePatient(ObservableCollection<Patient> patientsCollection, ObservableCollection<MedicalRecord> recordsCollection, PatientService patientService, MedicalRecordService medicalRecordService)
        {
            InitializeComponent();
            DataContext = this;
            Patient = new Patient();
            MedicalRecord = new MedicalRecord();
            PatientsList = patientsCollection;
            RecordsList = recordsCollection;
            PatientService = patientService;
            MedicalRecordService = medicalRecordService;
        }

        private void CreatePatient_Click(object sender, RoutedEventArgs e)
        {
                PatientsList.Add(Patient);
                PatientService.PatientsToCSV(PatientsList, "..\\..\\..\\Data\\Users\\Patients\\patients.txt");
                MedicalRecord.Id = Patient.Id;
                RecordsList.Add(MedicalRecord);
                MedicalRecordService.MedicalRecordToCSV(RecordsList,"..\\..\\..\\Data\\Users\\Patients\\medicalrecords.txt");
                MessageBox.Show("Successfully created medical record and add a patient");
                Close();
        }

        private void CancelButton_Click(object sender, RoutedEventArgs e)
        {
            Close();
        }
    }
}
