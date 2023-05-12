using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using ZdravoCorp.Models.Entities.ManagerEntities;
using ZdravoCorp.Serialization;

namespace ZdravoCorp.Models.Services.ManagerServices
{
    internal class ManagerService
    {
        //private static List<Manager> managers;
        private Manager manager;
        public static List<Manager> ObjManagerList;

        public Manager Manager
        {
            get { return manager; }
            set { manager = value; }
        }

        public ManagerService()
        {
            //managers = new List<Manager>();
            manager = new Manager();
            ObjManagerList = new List<Manager>();
            ObjManagerList = ManagersFromCSV("..\\..\\..\\Data\\Users\\managers.txt").ToList();
        }

        public void AssignManager(string name_, string surname_, string username_, string password_)
        {
            Random r = new Random();
            manager = new Manager((uint)r.Next(0,100),name_, surname_, username_, password_);
        }

        public static ObservableCollection<Manager> ManagersFromCSV(string filename)
        {
            List<Manager> managers = new List<Manager>();
            Serializer<Manager> managerSerializer = new Serializer<Manager>();
            managers = managerSerializer.fromCSV(filename);
            ObservableCollection<Manager> managerObservableCollection = new ObservableCollection<Manager>(managers);
            return managerObservableCollection;
        }

        public void ManagersToCSV(ObservableCollection<Manager> nurses, string filename)
        {
            List<Manager> managerList = nurses.ToList();
            Serializer<Manager> managerSerializer = new Serializer<Manager>();
            managerSerializer.toCSV(filename, managerList);
        }

        public List<Manager> GetAll()
        {
            return ObjManagerList;
        }

    }
}
