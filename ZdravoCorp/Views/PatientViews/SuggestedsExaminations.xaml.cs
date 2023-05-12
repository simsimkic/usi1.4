using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
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
using ZdravoCorp.Models.Entities.Appointments;
using ZdravoCorp.Models.Entities.Patients;
using ZdravoCorp.Models.Services.AppointmentServices;
using ZdravoCorp.Models.Services.UserServices;

namespace ZdravoCorp.Views.PatientViews
{
    public partial class SuggestedExaminations : Window
    {
        public ObservableCollection<Examination> PreferredExaminations { get; set; }

        public ExaminationsServices ExaminationService;
        public PatientService PatientService;
        public ObservableCollection<Examination> ExaminationList;
        public SuggestedExaminations(List<Examination> preferredExaminations, ExaminationsServices examinationService, PatientService patientService, ObservableCollection<Examination> examinationList)
        {
            InitializeComponent();
            ExaminationService = examinationService;
            PatientService = patientService;
            ExaminationList = examinationList;
            PreferredExaminations = new ObservableCollection<Examination>(preferredExaminations);
            ExaminationsDataGrid.ItemsSource = PreferredExaminations;
            PatientService = patientService;
        }
        private void SelectedExamination_OnClick(object sender, RoutedEventArgs e)
        {
            if ((Examination)ExaminationsDataGrid.SelectedItem != null)
            {
                Examination examination = (Examination)ExaminationsDataGrid.SelectedItem;
                bool created = ExaminationService.CreateAppointments(examination.Patient, PatientService,
                    examination.DateTime, examination.Doctor, PreferredExaminations);
                if (created)
                    ExaminationList.Add(examination);
                this.Close();
            }
        }
    }
}
