using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using ZdravoCorp.Models.Entities.Notification;
using ZdravoCorp.Models.Entities.Users;
using ZdravoCorp.Serialization;
namespace ZdravoCorp.Models.Services.NotificationServices
{
    public class NotificationService
    {
        private static List<Notification> _allNotifications;

        public NotificationService()
        {
            _allNotifications = new List<Notification>();
            _allNotifications = NotificationsFromCSV("..\\..\\..\\Data\\Notifications\\notifications.txt").ToList();
        }
        public List<Notification> GetAll()
        {
            return _allNotifications;
        }

        public void Add(Notification objNewNotification)
        {
            _allNotifications.Add(objNewNotification);
        }

        private static ObservableCollection<Notification>  NotificationsFromCSV(string filename)
        {
            var notificationSerializer = new Serializer<Notification>();
            var notifications = notificationSerializer.fromCSV(filename);
            return new ObservableCollection<Notification>(notifications);
        }

        public void NotificationsToCSV(ObservableCollection<Notification> notifications, string filename)
        {
            var notificationList = notifications.ToList();
            var notificationSerializer = new Serializer<Notification>();
            notificationSerializer.toCSV(filename, notificationList);
        }
    }
}
