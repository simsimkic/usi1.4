using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;
using System.Windows.Documents;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using System.Windows.Shapes;
using ZdravoCorp.Models.Entities.Patients;
using ZdravoCorp.Models.Entities.Users;

namespace ZdravoCorp.Views.PatientViews
{
    /// <summary>
    /// Interaction logic for MedicalRecordPatient.xaml
    /// </summary>
    public partial class MedicalRecordPatient : Window
    {
        public Patient Patient;
        public MedicalRecord MedicalRecord;
        public MedicalRecordPatient(Patient patient, MedicalRecord medicalRecord)
        {
            InitializeComponent();
            this.DataContext = this;
            Patient = patient;
            MedicalRecord = medicalRecord;
            TextBoxUsername.Text = patient.Username;
            TextBoxPassword.Text = patient.Password;
            TextBoxId.Text = patient.Id.ToString();
            TextBoxName.Text = patient.FirstName;
            TextBoxSurname.Text = patient.LastName;
            TextBoxHeight.Text = medicalRecord.Height.ToString();
            TextBoxWeight.Text = medicalRecord.Weight.ToString();
            TextBoxHistory.Text = medicalRecord.MedicalHistory;
            this.DataContext = this;
        }
        private void CancelButtonClick(object sender, RoutedEventArgs e)
        {
            Close();
        }
    }
}
