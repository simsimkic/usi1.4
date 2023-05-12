using System.Collections.ObjectModel;
using System.Linq;
using System.Windows;
using ZdravoCorp.Models.Entities.Patients;
using ZdravoCorp.Models.Entities.Users;
using ZdravoCorp.Models.Services.PatientServices;
using ZdravoCorp.Models.Services.UserServices;

namespace ZdravoCorp.Views.DoctorViews
{
    public partial class UpdateMedicalRecord : Window
    {
        public Patient Patient { get; set; }
        public Patient SelectedPatient { get; set; }
        public MedicalRecord MedicalRecord { get; set; }
        public MedicalRecord SelectedMedicalRecord { get; set; }
        public ObservableCollection<MedicalRecord> RecordsList { get; }
        public MedicalRecordService MedicalRecordService { get; set; }
        public UpdateMedicalRecord(Patient selectedPatient)
        {
            InitializeComponent();
            MedicalRecordService = new MedicalRecordService();
            RecordsList = new ObservableCollection<MedicalRecord>(MedicalRecordService.GetAll());

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

            SelectedMedicalRecord = RecordsList.FirstOrDefault(mr => mr.Id == selectedPatient.Id);
            
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

        private void UpdateButton_Click(object sender, RoutedEventArgs e)
        {
                RecordsList.Remove(RecordsList.FirstOrDefault(mr => mr.Id == SelectedPatient.Id));
                RecordsList.Add(MedicalRecord);
                MedicalRecordService.MedicalRecordToCSV(RecordsList, "..\\..\\..\\Data\\Users\\Patients\\medicalrecords.txt");
                MessageBox.Show("Successfully updated medical record for patient");
                Close();
        }

        private void CancelButton_Click(object sender, RoutedEventArgs e)
        {
            Close();
        }
    }
}
