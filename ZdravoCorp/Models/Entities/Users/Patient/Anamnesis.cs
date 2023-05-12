using System.ComponentModel;
using ZdravoCorp.Serialization;

namespace ZdravoCorp.Models.Entities.Patients;

public class Anamnesis: Serializable, INotifyPropertyChanged {
    private uint _id;
    private uint _examinationId;

    public string Disease {
        get;
        set;
    }

    public string Symptoms {
        get;
        set;
    }

    public string Allergens {
        get;
        set;
    }

    public Anamnesis() {
        _id = 0;
        _examinationId = 0;
    }

    public uint Id {
        get => _id;
        set {
            _id = value;
            OnPropertyChanged("Id");
        }
    }

    public uint ExaminationId {
        get => _examinationId;
        set {
            _examinationId = value;
            OnPropertyChanged("IdPatient");
        }
    }

    public Anamnesis(uint id, string disease, string symptoms, string allergens) {
        _id = id;
        Disease = disease;
        Symptoms = symptoms;
        Allergens = allergens;
    }

    public Anamnesis(uint id, string disease, string symptoms, string allergens, uint examinationId) {
        _id = id;
        Disease = disease;
        Symptoms = symptoms;
        Allergens = allergens;
        ExaminationId = examinationId;
    }

    public void FromCSV(string[] values) {
        Id = uint.Parse(values[0]);
        ExaminationId = uint.Parse(values[1]);
        Disease = values[2];
        Symptoms = values[3];
        Allergens = values[4];
    }

    public string[] ToCSV() {
        string[] csvValues = {
            Id.ToString(),
            _examinationId.ToString(),
            Disease,
            Symptoms,
            Allergens
        };
        return csvValues;
    }

    public event PropertyChangedEventHandler PropertyChanged;
    protected virtual void OnPropertyChanged(string name) {
        PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(name));
    }
}