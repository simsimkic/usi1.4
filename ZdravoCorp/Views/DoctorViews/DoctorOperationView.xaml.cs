using System;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.InteropServices;
using System.Windows;
using System.Windows.Input;
using System.Collections.ObjectModel;
using ZdravoCorp.Models.Entities.Appointments;
using ZdravoCorp.Models.Entities.Users;
using ZdravoCorp.Models.Services.AppointmentServices;
using ZdravoCorp.Models.Services.PatientServices;
using ZdravoCorp.Models.Services.UserServices;
using ZdravoCorp.ViewModels.PatientViewModels;
using ZdravoCorp.Views.DoctorViews;

namespace ZdravoCorp.Views.DoctorViews
{
    public partial class DoctorOperationView : Window
    {

        private static Doctor Doctor { get; set; }
        private static PatientService PatientService { get; set; }
        private static AppointmentService AppointmentService { get; set; }
        private ObservableCollection<Operation> Operations { get; set; }
        private MedicalRecordService MedicalRecordService { get; set; }
        private AnamnesisService AnamnesisService { get; set; }

        public DoctorOperationView(Doctor doctor)
        {
            InitializeComponent();
            Doctor = doctor;
            PatientService = new PatientService();
            AnamnesisService = new AnamnesisService();
            MedicalRecordService = new MedicalRecordService();
            InitializeOperations();
        }

        private void InitializeOperations()
        {
            Operations = new ObservableCollection<Operation>(Doctor.Operations);
            OperationsDataGrid.ItemsSource = Operations;
        }

        private void AddExamination_OnClick(object sender, RoutedEventArgs e)
        {
            /* AddExamination addExamination = new AddExamination(PatientService, ExaminationService, Patient, ExaminationList);
            addExamination.Show(); */
        }
        
        private void UpdateExamination_OnClick(object sender, RoutedEventArgs e)
        {
            /* if ((Operation)OperationsDataGrid.SelectedItem != null)
            {
                UpdateExamination updateExamination = new UpdateExamination(PatientService, ExaminationService, Patient, (Examination)this.ExaminationsDataGrid.SelectedItem, ExaminationList);
                updateExamination.Show();
            } */
        }

        private void Button_Click_Delete_Examination(object sender, RoutedEventArgs e)
        {
            /* if (OperationsDataGrid.SelectedItem == null)
            {
                MessageBox.Show("You must select a patient");
            }
            else
            {
                var examinationToRemove = Operations.FirstOrDefault(ex => ex.Id == ((Examination)OperationsDataGrid.SelectedItem).Id);
                if (examinationToRemove != null)
                {
                    if (ExaminationService.DeleteAppointments(Patient, PatientService, (Examination)ExaminationsDataGrid.SelectedItem))
                    {
                        ExaminationList.Remove(examinationToRemove);
                    } 
                } 
               ExaminationsServices.ExaminationsToCSV(Operations, "..\\..\\..\\Data\\Appointments\\operations.txt");
            } */
        }
        private void MedicalRecordPatientButton_Click(object sender, RoutedEventArgs e)
        {
            /* MedicalRecord medicalRecord = MedicalRecordService.GetAll().FirstOrDefault(mr => mr.Id == Patient.Id);

            if (medicalRecord != null)
            {
                MedicalRecordPatient medicalRecordPatient = new MedicalRecordPatient(Patient, medicalRecord);
                medicalRecordPatient.Show();
            } */
        }
        private void ViewExaminationAnamnesisButton_Click(object sender, RoutedEventArgs e)
        {
            /* if ((Examination)ExaminationsDataGrid.SelectedItem != null)
            {
                uint examinationId = ((Examination)ExaminationsDataGrid.SelectedItem).Id;
                List<Anamnesis> anamnesisForExamination = AnamnesisService.GetAll().Where(anamnesis => anamnesis.ExaminationId == examinationId).ToList();
                if (anamnesisForExamination.Count > 0)
                {
                    ShowExaminationAnamnesis showExaminationAnamnesis = new ShowExaminationAnamnesis(anamnesisForExamination[0]);
                    showExaminationAnamnesis.Show();
                }
            } */
        }
        private void AllExaminationsButton_Click(object sender, RoutedEventArgs e)
        {
            DoctorExaminationView examinationView = new DoctorExaminationView(Doctor);
            examinationView.Show();
            Close();
        }
        
        private void AllPatientsButton_Click(object sender, RoutedEventArgs e)
        {
            DoctorPatientView patientView = new DoctorPatientView(Doctor);
            patientView.Show();
            Close();
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
