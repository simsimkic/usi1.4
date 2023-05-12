using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;
using ZdravoCorp.Models.Entities.Appointments;

namespace ZdravoCorp.Models.Entities.Users;
public class Patient: User, IDataErrorInfo {
  private List < uint > _examinationIds;
  private List < Examination > _examinations;
  private List<Operation> _operations;
  private bool _blocked;

  public List < uint > ExaminationId {
    get => _examinationIds;
    set => _examinationIds = value;
  }

  public List < Examination > Examinations {
    get => _examinations;
    set => _examinations = value;
  }

  public List<Operation> Operations
  {
    get => _operations;
    set => _operations = value;
  }
  
  public bool Blocked {
    get => _blocked;
    set => _blocked = value;
  }

  public Dictionary < DateTime, bool > ExaminationsMap {
    get;
    set;
  } //dictionary of appoitments and date of them

  public Patient(): base() {
    _examinations = new List < Examination > ();
    _operations = new List<Operation>();
    _examinationIds = new List < uint > ();
    ExaminationsMap = new Dictionary < DateTime, bool > ();
  }

  public Patient(uint id) {
     Id = id;
    _examinations = new List < Examination > ();
    _operations = new List<Operation>();
    _examinationIds = new List < uint > ();
    ExaminationsMap = new Dictionary < DateTime, bool > ();
  }

  protected Patient(uint id, string username, string password, string firstName, string lastName): base(id, username, password, firstName, lastName) {
    _examinations = new List < Examination > ();
    _operations = new List<Operation>();
    _examinationIds = new List < uint > ();
    ExaminationsMap = new Dictionary < DateTime, bool > ();
    Blocked = false;
  }

  protected Patient(uint id, string username, string password, string firstName, string lastName, List < Examination > examinations, bool blocked,
    Dictionary < DateTime, bool > examinationsMap): base(id, username, password, firstName, lastName) {
    _examinations = examinations;
    _blocked = blocked;
    ExaminationsMap = examinationsMap;
  }

  public Patient(Patient patient)
  {
      Id = patient.Id;
      Username = patient.Username;
      Password = patient.Password;
      FirstName = patient.FirstName;
      LastName = patient.LastName;
      _examinations = patient.Examinations;
      _operations = patient.Operations;
      _examinationIds = patient.ExaminationId;
      ExaminationsMap = patient.ExaminationsMap;
      _blocked = patient.Blocked;
  }

  public override void FromCSV(string[] values) {
    Id = uint.Parse(values[0]);
    Username = values[1];
    Password = values[2];
    FirstName = values[3];
    LastName = values[4];
    string tempString = values[5];
    string[] examinationList = tempString.Split(";");
    foreach(string tempStr in examinationList) {
      ExaminationId.Add(uint.Parse(tempStr));
    }

    Blocked = bool.Parse(values[6]);
  }

  public override string[] ToCSV() {
        
    string examinationIdsStr = String.Join(";", ExaminationId);
    Console.WriteLine(examinationIdsStr);
    if (String.IsNullOrEmpty(examinationIdsStr)) {
      examinationIdsStr = "0";
    }

    string[] csvValues = {
      Id.ToString(),
      Username,
      Password,
      FirstName,
      LastName,
      examinationIdsStr,
      _blocked.ToString()

    };
    return csvValues;
  }

  public string Error => null;

  public string this[string columnName] {
    get {
      switch (columnName) {
      case "Name":
        if (string.IsNullOrEmpty(FirstName)) {
          return "First name is required";
        }
        if (FirstName.Length < 3) {
          return "First name must be at least 3 characters long";
        }
        break;

      case "Surname":
        if (string.IsNullOrEmpty(LastName)) {
          return "Last name is required";
        }
        if (LastName.Length < 3) {
          return "Last name must be at least 3 characters long";
        }
        break;

      case "Username":
        if (string.IsNullOrEmpty(Username)) {
          return "Username is required";
        }
        if (Username.Length < 3) {
          return "Username must be at least 3 characters long";
        }
        break;

      case "Password":
        if (string.IsNullOrEmpty(Password)) {
          return "Password is required";
        }
        if (Password.Length < 3) {
          return "Password must be at least 3 characters long";
        }
        break;

      case "Id":
        if (Id <= 0) {
          return "Id must be greater than 0";
        }
        break;
      }
      return null;
    }
  }

  private static readonly string[] ValidatedProperties = {
    "Name",
    "Surname",
    "Username",
    "Password",
    "Id"
  };
  public bool IsValid => ValidatedProperties.All(prop => this[prop] != null);

  public event PropertyChangedEventHandler PropertyChanged;
  protected virtual void OnPropertyChanged(string name) {
    PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(name));
  }
}