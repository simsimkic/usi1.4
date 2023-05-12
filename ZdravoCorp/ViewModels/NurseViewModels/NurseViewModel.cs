using System.ComponentModel;
using ZdravoCorp.Models.Services.UserServices;

namespace ZdravoCorp.ViewModels.NurseViewModels;
public class NurseViewModel : INotifyPropertyChanged
    {
        public event PropertyChangedEventHandler? PropertyChanged;
        private void OnPropertyChanged(string propertyName)
        {
            PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(propertyName));
        }
    }
