using System.Collections.Generic;
using System.Linq;
using System.Windows;
using ZdravoCorp.Models.Entities.ManagerEntities;
using ZdravoCorp.Views;
using ZdravoCorp.Models.Entities.Users;
using ZdravoCorp.Models.Services.AppointmentServices;
using ZdravoCorp.Models.Services.UserServices;
using ZdravoCorp.Models.Services.NotificationServices;
using ZdravoCorp.Models.Entities.Notification;
using ZdravoCorp.Models.Services.ManagerServices;
using System.Collections.ObjectModel;
using ZdravoCorp.Views.DoctorViews;

namespace ZdravoCorp
{
    public partial class MainWindow : Window
    {   
        public MainWindow()
        {
            InitializeComponent();    
        }
        private void LoginButton_Click(object sender, RoutedEventArgs e)
        {
            string username = UsernameTextBox.Text;
            string password = PasswordBox.Password;

            ManagerService managerService = new ManagerService();
            List<Manager> managersList = managerService.GetAll();

            NurseService nurseService = new NurseService();
            List<Nurse> nursesList = nurseService.GetAll();

            DoctorService doctorService = new DoctorService();
            List<Doctor> doctorsList = doctorService.GetAll();

            NotificationService notificationService = new NotificationService();
            List<Notification> notificationsList = notificationService.GetAll();

            AppointmentService appointmentService = new AppointmentService();
            ExaminationsServices ExaminationServices = new ExaminationsServices();
            PatientService PatientServices = new PatientService(ExaminationServices.GetExaminations());
            ExaminationServices.loadPatient(PatientServices);


            var managerLogin = managersList.FirstOrDefault(m => m.Username == username && m.Password == password);
            var nurseLogin = nursesList.FirstOrDefault(n => n.Username == username && n.Password == password);
            var doctorLogin = doctorsList.FirstOrDefault(d => d.Username == username && d.Password == password);
            if (nurseLogin != null)
            {
                PatientProfile patientProfile = new PatientProfile();
                patientProfile.Show();
                Close();
            }
            else if (managerLogin != null)
            {
                HospitalEquipment hospitalEquipment = new HospitalEquipment();
                hospitalEquipment.Show();
                Close();
            }
            else if (doctorLogin != null)
            {
                var foundNotification = notificationsList.FirstOrDefault(n => n.Doctor.Id == doctorLogin.Id);
                if (foundNotification != null)
                {
                    MessageBox.Show(foundNotification.Message);
                    notificationsList.Remove(foundNotification);
                    if (!foundNotification.SeenPatient)
                    {
                        foundNotification.SeenDoctor = true;
                        notificationsList.Add(foundNotification);
                    }
                    notificationService.NotificationsToCSV(new ObservableCollection<Notification>(notificationService.GetAll()), "..\\..\\..\\Data\\Notifications\\notifications.txt");
                }
                appointmentService.GetDoctorsAppointments(doctorLogin);
                DoctorExaminationView docView = new DoctorExaminationView(doctorLogin);
                docView.Show();
                Close();

            }
            else if (PatientServices.IsPatient(username, password))
            {
                Patient patientLogin= PatientServices.LoginPatient(username, password);
                var foundNotification = notificationsList.FirstOrDefault(n => n.Patient.Id == patientLogin.Id);
                
                if (foundNotification != null)
                {
                    MessageBox.Show(foundNotification.Message);
                    notificationsList.Remove(foundNotification);
                    if (!foundNotification.SeenDoctor)
                    {
                        foundNotification.SeenPatient = true;
                        notificationsList.Add(foundNotification);
                    }
                    notificationService.NotificationsToCSV(new ObservableCollection<Notification>(notificationService.GetAll()), "..\\..\\..\\Data\\Notifications\\notifications.txt");
                }

                MyExams myExams = new MyExams(PatientServices, ExaminationServices, patientLogin);
                myExams.Show();
                Close();
            }
            else
                MessageBox.Show("Failed login");
        }
    }
}
