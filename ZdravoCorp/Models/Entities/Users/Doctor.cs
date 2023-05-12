using System;
using ZdravoCorp.Models.Enumerations;
using ZdravoCorp.Models.Entities.Appointments;
using System.Collections.Generic;
using System.Windows.Documents.DocumentStructures;
using ZdravoCorp.Serialization;

namespace ZdravoCorp.Models.Entities.Users;
public class Doctor: User {
    private Specialization _specialization;
    private List < Examination > _examinations;
    private List < Operation > _operations;

    public Specialization Specialization {
        get => _specialization;
        set => _specialization = value;
    }

    public List < Examination > Examinations {
        get => _examinations;
        set => _examinations = value;
    }

    public List < Operation > Operations {
        get => _operations;
        set => _operations = value;
    }

    public Doctor(): base() {
        _specialization = Specialization.None;
        _examinations = new List < Examination > ();
        _operations = new List < Operation > ();
    }

    public Doctor(uint id, string username, string password, string firstName, string lastName): base(id, username, password, firstName, lastName) {
        _specialization = Specialization.None;
        _examinations = new List < Examination > ();
        _operations = new List < Operation > ();
    }

    public Doctor(uint id) {
        Id = id;
        _specialization = Specialization.None;
        _examinations = new List < Examination > ();
        _operations = new List < Operation > ();
    }

    public override void FromCSV(string[] values) {
        Id = uint.Parse(values[0]);
        Username = values[1];
        Password = values[2];
        FirstName = values[3];
        LastName = values[4];
        Enum.TryParse(values[5], out Specialization specialization);
        Specialization = specialization;
    }

    public override string[] ToCSV() {
        string[] csvValues = {
            Id.ToString(),
            Username,
            Password,
            FirstName,
            LastName
        };
        return csvValues;
    }
}