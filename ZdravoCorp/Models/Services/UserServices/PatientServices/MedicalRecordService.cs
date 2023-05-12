using ZdravoCorp.Models.Entities.Patients;
using ZdravoCorp.Serialization;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;

namespace ZdravoCorp.Models.Services.PatientServices;
public class MedicalRecordService {
    private static List < MedicalRecord > _medicalRecords;

    public List<MedicalRecord> MedicalRecords
    {
        get => _medicalRecords;
        set => _medicalRecords = value;
    }

    public MedicalRecordService() {
        _medicalRecords = new List < MedicalRecord > ();
        _medicalRecords = MedicalRecordFromCSV("..\\..\\..\\Data\\Users\\Patients\\medicalrecords.txt").ToList();
    }

    public List < MedicalRecord > GetAll() {
        return _medicalRecords;
    }

    public void Add(MedicalRecord record) {
        _medicalRecords.Add(record);
    }

    public static ObservableCollection < MedicalRecord > MedicalRecordFromCSV(string filename) {
        var recordSerializer = new Serializer < MedicalRecord > ();
        var records = recordSerializer.fromCSV(filename);
        return new ObservableCollection < MedicalRecord > (records);
    }

    public static void MedicalRecordToCSV(ObservableCollection < MedicalRecord > records, string filename) {
        Serializer < MedicalRecord > recordSerializer = new Serializer < MedicalRecord > ();
        recordSerializer.toCSV(filename, records.ToList());
    }
}