using System;
using System.Collections.ObjectModel;
using System.Windows;
using ZdravoCorp.Models.Entities.Appointments;
using ZdravoCorp.Models.Entities.Patients;
using ZdravoCorp.Models.Services.PatientServices;

namespace ZdravoCorp.Views.NurseViews
{
    public partial class CreateAnamnesis : Window
    {
        private static Random random = new Random();
        public Anamnesis Anamnesis { get; set; }
        public Examination Examination{ get; set; }
        public AnamnesisService AnamnesisService { get; set; }
        public CreateAnamnesis(Examination examination, AnamnesisService anamnesisService)
        {
            InitializeComponent();
            DataContext = this;
            Anamnesis = new Anamnesis();
            Examination = examination;
            AnamnesisService = anamnesisService;
        }

        private void CreateAnamnesisButton_Click(object sender, RoutedEventArgs e)
        {
            Anamnesis.Id = (uint)random.Next(100, 1000);
            Anamnesis.ExaminationId = Examination.Id;
            AnamnesisService.Add(Anamnesis);
            AnamnesisService.AnamnesisToCSV(new ObservableCollection<Anamnesis>(AnamnesisService.GetAll()), "..\\..\\..\\Data\\Users\\Patients\\anamneses.txt");
            Close(); }

        private void CancelButton_Click(object sender, RoutedEventArgs e)
        {
            Close();
        }
    }
}
