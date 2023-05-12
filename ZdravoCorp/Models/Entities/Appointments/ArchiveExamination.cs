using System;
using ZdravoCorp.Serialization;

namespace ZdravoCorp.Models.Entities.Appointments;

public class ArchiveExamination : Serializable
{
    public uint ExaminationId { get; set; }
    public string Type { get; set; }

    public DateTime DateUpdate { get; set; }

    public string PatientUsername { get; set; }

    public ArchiveExamination()
    {
    }

    public ArchiveExamination(Examination ex, string type, DateTime date, string patientUsername)
    {
        ExaminationId = ex.Id;
        Type = type;
        DateUpdate = date;
        PatientUsername = patientUsername;
    }

    public string[] ToCSV()
    {
        string[] csvValues =
        {
            ExaminationId.ToString(),
            Type,
            DateUpdate.ToString(),
            PatientUsername
        };
        return csvValues;
    }

    public void FromCSV(string[] values)
    {
        ExaminationId = uint.Parse(values[0]);
        Type = values[1];
        DateUpdate = DateTime.Parse(values[2]);
        PatientUsername = values[3];
    }
}