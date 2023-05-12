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
using ZdravoCorp.Models.Entities.Users;
using ZdravoCorp.Models.Services.AppointmentServices;
using ZdravoCorp.ViewModels.PatientViewModels;

namespace ZdravoCorp.Views.PatientViews
{
    public partial class ShowAllAnamnesis : Window
    {
        public AnamnesisViewModel anamnesisViewModel { get; set; }
        public ObservableCollection<Anamnesis> ListOfAnamnesisPatient { get; set; }
        public ExaminationsServices ExaminationsServices { get; set; }
        public ShowAllAnamnesis(ObservableCollection<Anamnesis> listOfAnamnesisPatient, ExaminationsServices examinationsServices)
        {
            InitializeComponent();
            ListOfAnamnesisPatient = listOfAnamnesisPatient;
            ExaminationsServices = examinationsServices;
            List<MedicalProfile> showMedicalProfileList = examinationsServices.FindMedicalProfilesFromExaminationsAndAnamnesis(ListOfAnamnesisPatient);
            anamnesisViewModel = new AnamnesisViewModel(showMedicalProfileList, examinationsServices);
            DataContext = anamnesisViewModel;
        }
        private void ButtonFilter_OnClick(object sender, RoutedEventArgs e)
        {
            ApplyFilter();
        }
        private void ApplyFilter()
        {
            string filteringDoctor = (string)FilteringDoctor.SelectionBoxItem;
            string filteringSpecialization = (string)FilteringSpecialization.SelectionBoxItem;
            anamnesisViewModel.FilterMedicalProfile(filteringDoctor, filteringSpecialization);
        }
        private void ButtonReset_OnClick(object sender, RoutedEventArgs e)
        {
            anamnesisViewModel.LoadMedicalProfile();
        }

        private void FilteringDoctor_OnSelectionChanged(object sender, SelectionChangedEventArgs e)
        {
            int selectedIndex = FilteringDoctor.SelectedIndex;
            anamnesisViewModel.UpdateFilteringSpecialization(selectedIndex);
        }

        private void SearchBar_TextChanged(object sender, TextChangedEventArgs e)
        {
            string searchText = textBoxFilter.Text.ToLower();
            anamnesisViewModel.FilterByText(searchText);
        }
    }
}
