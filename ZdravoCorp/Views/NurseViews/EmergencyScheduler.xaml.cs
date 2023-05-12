using System;
using System.Collections.ObjectModel;
using System.Numerics;
using System.Windows;
using ZdravoCorp.Models.Entities.Appointments;
using ZdravoCorp.Models.Entities.Notification;
using ZdravoCorp.Models.Entities.Users;
using ZdravoCorp.Models.Enumerations;
using ZdravoCorp.Models.Services.AppointmentServices;
using ZdravoCorp.Models.Services.NotificationServices;
using ZdravoCorp.Models.Services.UserServices;

namespace ZdravoCorp.Views.NurseViews
{
    /// <summary>
    /// Interaction logic for EmergencyScheduler.xaml
    /// </summary>
    public partial class EmergencyScheduler : Window
    {
        public PatientService PatientService { get; set; }
        public ExaminationsServices ExaminationService { get; set; }
        public DoctorService DoctorService { get; set; }
        public AppointmentService AppointmentService { get; set; }
        public ObservableCollection<Appointment> AppointmentList { get; set; }
        public Appointment SelectedAppointment { get; set; }
        public uint OperationDuration { get; set; }
        public EmergencyScheduler(PatientService patientService, ExaminationsServices examinationService)
        {
            InitializeComponent();
            this.DataContext = this;
            PatientService = patientService;
            ExaminationService = examinationService;
            AppointmentService = new AppointmentService();
            DoctorService = new DoctorService();

            //adding specialization in combobox, it can't simply code because it's a combobox
            Array values = Enum.GetValues(typeof(Specialization));
            foreach (Specialization value in values)
                comboBoxSpecialization.Items.Add(value);

            foreach (Patient patient in PatientService.GetAll())
                comboBoxPatient.Items.Add(patient.Id + " " + patient.FirstName + " " + patient.LastName);

            comboBoxAppointment.Items.Add("Examination");
            comboBoxAppointment.Items.Add("Operation");}
        private void FindAppointmentsButton_Click(object sender, RoutedEventArgs e)
        {
            if (comboBoxSpecialization.SelectedItem == null || comboBoxAppointment.SelectedItem == null)
            {
                MessageBox.Show("You must select specialization and appointment");
                return;
            }

            string specialization = comboBoxSpecialization.SelectedItem.ToString();
            string appointment = comboBoxAppointment.SelectedItem.ToString();
            string patientNotSplitted = comboBoxPatient.SelectedItem.ToString();
            string[] partsPateint = patientNotSplitted.Split(' ');
            uint patientId = uint.Parse(partsPateint[0]);

            Doctor availableDoctor = DoctorService.GetSpecializedDoctorForDateRange(specialization, AppointmentService, appointment, new Patient(patientId), OperationDuration, ExaminationService,PatientService);
            if (availableDoctor.FirstName != "")
            {
                if (appointment == null)
                    MessageBox.Show("You must select examination or operation");
                else if (appointment == "Examination")
                    MessageBox.Show("Successfully added emergency examination, doctor is " + availableDoctor.FirstName + " " + availableDoctor.LastName);
                else
                    MessageBox.Show("Successfully added emergency operation , doctor is " + availableDoctor.FirstName + " " + availableDoctor.LastName);
            }
            else
            {
                AppointmentList = new ObservableCollection<Appointment>(AppointmentService.CheckAvailabilityAppointments(specialization,DoctorService));
                appointmentDataGrid.ItemsSource = AppointmentList;
            }
        }
        private void PostponeAppointmentButton_Click(object sender, RoutedEventArgs e)
        {
            if (appointmentDataGrid.SelectedItem == null)
                MessageBox.Show("You must select a appointment");
            else
            {
                NotificationService notificationService = new NotificationService();
                Appointment appointment = (Appointment)appointmentDataGrid.SelectedItem;
                AppointmentService.PostponeAppointment(appointment, DoctorService);
                if (appointment is Examination)
                {
                    MessageBox.Show("Examination is canceled");
                    notificationService.Add(new Notification(new Doctor(appointment.Doctor.Id), new Patient(appointment.Patient.Id), "The examination was postponed"));
                }
                else if (appointment is Operation)
                {
                    MessageBox.Show("Operation is canceled");
                    notificationService.Add(new Notification(new Doctor(appointment.Doctor.Id), new Patient(appointment.Patient.Id), "The operation was postponed"));
                }
                notificationService.NotificationsToCSV(new ObservableCollection<Notification>(notificationService.GetAll()), "..\\..\\..\\Data\\Notifications\\notifications.txt");
                appointmentDataGrid.ItemsSource = new ObservableCollection<Appointment>();
            }
        }
        private void LogoutButton_Click(object sender, RoutedEventArgs e)
        {
            MainWindow logout = new MainWindow();
            logout.Show();
            Close();
        }
        private void PatientProfileButton_Click(object sender, RoutedEventArgs e)
        {
            PatientProfile patientProfile = new PatientProfile();
            patientProfile.Show();
            Close();
        }
    }
}
