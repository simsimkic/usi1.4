using System.Collections.ObjectModel;
using System.Linq;
using System.Windows;
using ZdravoCorp.Models.Entities.Patients;
using ZdravoCorp.Models.Entities.Users;
using ZdravoCorp.Models.Services.PatientServices;
using ZdravoCorp.Models.Services.UserServices;

namespace ZdravoCorp.Views
{
    /// <summary>
    /// Interaction logic for UpdatePatient.xaml
    /// </summary>
    public partial class UpdatePatient : Window
    {
        public Patient Patient { get; set; }
        public Patient SelectedPatient { get; set; }
        public MedicalRecord MedicalRecord { get; set; }
        public MedicalRecord SelectedMedicalRecord { get; set; }
        public ObservableCollection<Patient> PatientsList { get; }
        public ObservableCollection<MedicalRecord> RecordsList { get; }
        public PatientService PatientService { get; set; }
        public MedicalRecordService MedicalRecordService { get; set; }
        public UpdatePatient(Patient selectedPatient, ObservableCollection<Patient> patientsCollection, ObservableCollection<MedicalRecord> recordsList, PatientService patientService, MedicalRecordService medicalRecordService)
        {
            InitializeComponent();
            PatientService = patientService;
            MedicalRecordService = medicalRecordService;
            PatientsList = patientsCollection;
            RecordsList = recordsList;
            DataContext = this;

            Patient = new Patient
            {
                Id = selectedPatient.Id,
                FirstName = selectedPatient.FirstName,
                LastName = selectedPatient.LastName,
                Username = selectedPatient.Username,
                Password = selectedPatient.Password,
                Blocked = selectedPatient.Blocked,
                ExaminationId = selectedPatient.ExaminationId

            };

            SelectedPatient = selectedPatient;

            TextBoxName.Text = selectedPatient.FirstName;
            TextBoxSurname.Text = selectedPatient.LastName;
            TextBoxId.Text = selectedPatient.Id.ToString();
            TextBoxUsername.Text = selectedPatient.Username;
            TextBoxPassword.Text = selectedPatient.Username;

            foreach (MedicalRecord mr in recordsList)
            {
                if (mr.Id == selectedPatient.Id)
                {
                    SelectedMedicalRecord = mr;
                    TextBoxHeight.Text = mr.Height.ToString();
                    TextBoxWeight.Text = mr.Weight.ToString();
                    TextBoxHistory.Text = mr.MedicalHistory;
                    break;
                }
            }
            MedicalRecord = new MedicalRecord
            {
                Id = selectedPatient.Id,
                Height = SelectedMedicalRecord.Height,
                Weight = SelectedMedicalRecord.Weight,
                MedicalHistory = SelectedMedicalRecord.MedicalHistory
            };
            TextBoxHeight.Text = SelectedMedicalRecord.Height.ToString();
            TextBoxWeight.Text = SelectedMedicalRecord.Weight.ToString();
            TextBoxHistory.Text = SelectedMedicalRecord.MedicalHistory;
        }

        private void UpdatePatient_Click(object sender, RoutedEventArgs e)
        {
                PatientsList.Add(Patient);
                RecordsList.Add(MedicalRecord);
                var patientToRemove = PatientsList.FirstOrDefault(p => p.FirstName == SelectedPatient.FirstName && p.Id == SelectedPatient.Id && p.LastName == SelectedPatient.LastName);
                var recordToRemove = RecordsList.FirstOrDefault(mr => mr.Id == SelectedPatient.Id);

                if (patientToRemove != null && recordToRemove != null)
                {
                    PatientsList.Remove(patientToRemove);
                    RecordsList.Remove(recordToRemove);
                }

                PatientService.PatientsToCSV(PatientsList, "..\\..\\..\\Data\\Users\\Patients\\patients.txt");
                MedicalRecordService.MedicalRecordToCSV(RecordsList, "..\\..\\..\\Data\\Users\\Patients\\medicalrecords.txt");
                MessageBox.Show("Successfully updated medical record and patient");
                Close();
        }

        private void CancelButton_Click(object sender, RoutedEventArgs e)
        {
            Close();
        }

    }
}
