using System.Collections.Generic;
using System.ComponentModel;
using ZdravoCorp.Models.Entities.Appointments;
using ZdravoCorp.Models.Services.AppointmentServices;

namespace ZdravoCorp.ViewModels.PatientViewModels;

public class ExaminationViewModel : INotifyPropertyChanged
{
    public event PropertyChangedEventHandler? PropertyChanged;
    private void OnPropertyChanged(string propertyName)
    {
        if (PropertyChanged != null)
        {
            PropertyChanged(this, new PropertyChangedEventArgs(propertyName));
        }
    }
    public ExaminationsServices ExaminationService;
    public ExaminationViewModel()
    {
        ExaminationService = new ExaminationsServices();
        LoadData();
    }

    private List<Examination> examinationsList;

    public List<Examination> ExaminationsList
    {
        get { return examinationsList; }
        set { examinationsList = value; OnPropertyChanged("PatientsList"); }
    }

    public void LoadData()
    {
        ExaminationsList = ExaminationService.GetExaminations();
    }
}