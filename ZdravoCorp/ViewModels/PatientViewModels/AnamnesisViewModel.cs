using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using ZdravoCorp.Models.Entities.Patients;
using ZdravoCorp.Models.Entities.Users;
using ZdravoCorp.Models.Services.AppointmentServices;

namespace ZdravoCorp.ViewModels.PatientViewModels
{
    public class AnamnesisViewModel : INotifyPropertyChanged
    {
        public event PropertyChangedEventHandler? PropertyChanged;

        public List<MedicalProfile> medicalProfile { get; set; }

        public List<MedicalProfile> OriginalMedicalProfile { get; set; }
        public ExaminationsServices ExaminationsServices { get; set; }
        private void OnPropertyChanged(string propertyName)
        {
            if (PropertyChanged != null)
                PropertyChanged(this, new PropertyChangedEventArgs(propertyName));
        }
        public AnamnesisViewModel(List<MedicalProfile> externalMedicalProfile, ExaminationsServices examinationsServices)
        {
            medicalProfile = externalMedicalProfile;
            OriginalMedicalProfile = externalMedicalProfile;
            ExaminationsServices = examinationsServices;
            FilteringDoctors = examinationsServices.DoctorsToStringFormat();
            FilteringSpecialization = examinationsServices.GetSpecialization();
        }
        public List<MedicalProfile> MedicalProfile
        {
            get { return medicalProfile; }
            set { medicalProfile = value; OnPropertyChanged("MedicalProfile"); }
        }
        public void LoadMedicalProfile()
        {
            MedicalProfile = OriginalMedicalProfile;
        }
        private List<string> filteringDoctors;
        public List<string> FilteringDoctors
        {
            get { return filteringDoctors; }
            set { filteringDoctors = value; OnPropertyChanged("FilteringDoctors"); }
        }
        private List<string> filteringSpecialization;
        public List<string> FilteringSpecialization
        {
            get { return filteringSpecialization; }
            set { filteringSpecialization = value; OnPropertyChanged("FilteringSpecialization"); }
        }

        public void UpdateFilteringSpecialization(int selectedSpecializationIndex)
        {
            if (selectedSpecializationIndex == 0)
                FilteringSpecialization = ExaminationsServices.GetSpecialization();
        }

        public void FilterMedicalProfile(string filterDoctor, string filterSpecialization)
        {
            MedicalProfile = MedicalProfile.Where(m => m.NameSurnameDoctor.Equals(filterDoctor) && m.SpecializationDoctor.Equals(filterSpecialization)).ToList();
        }
        public void FilterByText(string searchText)
        {
            LoadMedicalProfile();
            Console.WriteLine(searchText);

            var filteredMedicalProfile = from MedicalProfile medicalProfile in MedicalProfile
                                         where (medicalProfile.IdAnamnesis.ToString()).ToLower().Contains(searchText) ||
                                               medicalProfile.NameSurnameDoctor.ToLower().Contains(searchText) ||
                                               medicalProfile.SpecializationDoctor.ToLower().Contains(searchText) ||
                                               (medicalProfile.DateTimeAnamnesis.ToString()).ToLower().Contains(searchText) ||
                                               medicalProfile.Disease.ToLower().Contains(searchText) ||
                                               medicalProfile.Symptoms.ToLower().Contains(searchText) ||
                                               medicalProfile.Allergens.ToLower().Contains(searchText)
                                         select medicalProfile;
            MedicalProfile = filteredMedicalProfile.ToList();
        }
    }
}
