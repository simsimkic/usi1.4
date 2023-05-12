using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;
using ZdravoCorp.Models.Entities.Users;
using ZdravoCorp.Serialization;

namespace ZdravoCorp.Models.Services.UserServices;
public class NurseService {
    private static List < Nurse > _allNurses;

    public NurseService() {
        _allNurses = new List < Nurse > ();
        _allNurses = NursesFromCSV("..\\..\\..\\Data\\Users\\nurses.txt").ToList();
    }

    public List < Nurse > GetAll() {
        return _allNurses;
    }

    public void Add(Nurse objNewNurse) {
        _allNurses.Add(objNewNurse);
    }

    private static ObservableCollection < Nurse > NursesFromCSV(string filename) {
        var nurseSerializer = new Serializer < Nurse > ();
        var nurses = nurseSerializer.fromCSV(filename);
        return new ObservableCollection < Nurse > (nurses);
    }

    public void NursesToCSV(ObservableCollection < Nurse > nurses, string filename) {
        var nurseList = nurses.ToList();
        var nurseSerializer = new Serializer < Nurse > ();
        nurseSerializer.toCSV(filename, nurseList);
    }
}