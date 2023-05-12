using System.Collections.Generic;
using System.ComponentModel;
using ZdravoCorp.Models.Entities.Appointments;
using ZdravoCorp.Models.Entities.Users;
using ZdravoCorp.Models.Services.UserServices;

namespace ZdravoCorp.ViewModels.PatientViewModels;

public class PatientViewModel : INotifyPropertyChanged
{
    public event PropertyChangedEventHandler? PropertyChanged;

    private void OnPropertyChanged(string propertyName)
    {
        PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(propertyName));
    }

    public PatientService PatientService;

    public PatientViewModel(List<Examination> examinations)
    {
        PatientService = new PatientService(examinations);
        LoadData();
    }

    private List<Patient> patientsList;

    public List<Patient> PatientsList
    {
        get { return patientsList; }
        set { patientsList = value; OnPropertyChanged("PatientsList"); }
    }
    public void LoadData()
    {
        PatientsList = PatientService.GetPatients();
    }
}