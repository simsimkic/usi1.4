using System;
using System.Linq;
using System.Windows;
using System.Windows.Input;
using System.Collections.ObjectModel;
using ZdravoCorp.Models.Entities.Appointments;
using ZdravoCorp.Models.Entities.Users;
using ZdravoCorp.Models.Enumerations;
using ZdravoCorp.Models.Services.AppointmentServices;
using ZdravoCorp.Models.Services.PatientServices;
using ZdravoCorp.Models.Services.UserServices;
using ZdravoCorp.Views.NurseViews;

namespace ZdravoCorp.Views.DoctorViews
{
    public partial class DoctorExaminationView : Window
    {
        private static Doctor Doctor { get; set; }
        private ObservableCollection<Examination> ExaminationList { get; set; }
        private static PatientService PatientService { get; set; }
        private AnamnesisService AnamnesisService { get; set; }
        private MedicalRecordService MedicalRecordService { get; set; }
        
        public DoctorExaminationView(Doctor doctor)
        {
            InitializeComponent();
            PatientService = new PatientService();
            Doctor = doctor;
            AnamnesisService = new AnamnesisService();
            MedicalRecordService = new MedicalRecordService();
            InitializeExaminationList();
        }

        private void InitializeExaminationList()
        {
            ExaminationList = new ObservableCollection<Examination>(Doctor.Examinations);
            ExaminationsDataGrid.ItemsSource = ExaminationList;
        }
        
        private void AddExamination_OnClick(object sender, RoutedEventArgs e)
        {
            /* AddExamination addExamination = new AddExamination(PatientService, ExaminationService, Patient, ExaminationList);
            addExamination.Show(); */
        }

        private void AddPriorityExamination_OnClick(object sender, RoutedEventArgs e)
        {
            /* AddPriorityExamination addPriorityExamination = new AddPriorityExamination(PatientService, ExaminationService, Patient, ExaminationList);
            addPriorityExamination.Show(); */
        }

        private void UpdateExamination_OnClick(object sender, RoutedEventArgs e)
        {
            /* if ((Examination)ExaminationsDataGrid.SelectedItem != null)
            {
                UpdateExamination updateExamination = new UpdateExamination(PatientService, ExaminationService, Patient, (Examination)this.ExaminationsDataGrid.SelectedItem, ExaminationList);
                updateExamination.Show();
            } */
        }

        private void Button_Click_Delete_Examination(object sender, RoutedEventArgs e)
        {
            /* if (ExaminationsDataGrid.SelectedItem == null)
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
        private void AllOperationsButton_Click(object sender, RoutedEventArgs e)
        {
            DoctorOperationView operationView = new DoctorOperationView(Doctor);
            operationView.Show();
            Close();
        }

        private void AllPatientsButton_Click(object sender, RoutedEventArgs e)
        {
            DoctorPatientView patientView = new DoctorPatientView(Doctor);
            patientView.Show();
            Close();
        }

        private void StartExaminationButton_Click(object sender, RoutedEventArgs e)
        {
            var examination = (Examination)ExaminationsDataGrid.SelectedItem;
            if (examination is null)
            {
                MessageBox.Show("You must select an examination");
            }  
            else if (!CanStartExamination(examination.DateTime))
            {
                MessageBox.Show("It is not corresponding time for this examination");
            }
            else if (examination.Status != AppointmentStatus.Scheduled)
            {
                if (examination.Status == AppointmentStatus.Canceled)
                    MessageBox.Show("This examination is canceled");
                else
                    MessageBox.Show("This examination is completed");
            }
            else
            {
                var patient = PatientService.FindPatientById(examination.Patient.Id);
                UpdateMedicalRecord updateMedicalRecord = new UpdateMedicalRecord(patient);
                updateMedicalRecord.Closed += (sender, e) =>
                {
                    CreateAnamnesis createAnamnesis = new CreateAnamnesis(examination, new AnamnesisService());
                    createAnamnesis.Closed += (sender, e) =>
                    {
                        var aux = examination;
                        examination.Status = AppointmentStatus.Completed;

                        var examinationsServices = new ExaminationsServices();
                        examinationsServices.UpdateExamination(aux, examination, ExaminationList);
                        
                        MessageBox.Show("Examination completed sucessfully");
                    };
                    createAnamnesis.Show();
                };
                updateMedicalRecord.Show();
            }
        }

        private bool CanStartExamination(DateTime dt)
        {
            return dt <= DateTime.Now && DateTime.Now <= dt.AddMinutes(15);
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
