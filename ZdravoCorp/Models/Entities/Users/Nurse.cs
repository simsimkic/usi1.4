using ZdravoCorp.Serialization;

namespace ZdravoCorp.Models.Entities.Users;

public class Nurse: User {
    public override void FromCSV(string[] values) {
        Id = uint.Parse(values[0]);
        Username = values[1];
        Password = values[2];
        FirstName = values[3];
        LastName = values[4];
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