using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Runtime.InteropServices;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;
using System.Windows.Documents;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using System.Windows.Shapes;
using ZdravoCorp.Models.Entities;
using ZdravoCorp.Models.Entities.ManagerEntities;
using ZdravoCorp.Models.Services;
using ZdravoCorp.ViewModels.ManagerViewModels;

namespace ZdravoCorp.Views  
{
    /// <summary>
    /// Interaction logic for HospitalEquipment.xaml
    /// </summary>
    public partial class OrderEquipment: Window
    {
        private Equipment equipment;
        private EquipmentManager equipmentManager;
        public OrderEquipment(Equipment equipment, EquipmentManager equipmentManager)
        {
            {
                InitializeComponent();

                this.equipmentManager = equipmentManager;
                this.equipment = equipment;
                DataContext = equipment;
            }
        }

        private void CancelButton_Click(object sender, RoutedEventArgs e)
        {
            Close();
        }

        private void PlaceOrderButton_Click(object sender, RoutedEventArgs e)
        {
            int orderAmount;
            if (int.TryParse(TextBoxOrderAmount.Text, out orderAmount))
            {
                string model = equipment.Model;
                int amount = equipment.Amount + orderAmount;

                Console.WriteLine(model + " " + amount.ToString());

                equipmentManager.AddToStorage(equipment, orderAmount);
            }
            Close();
            equipmentManager.UpdateEquipmentList();
        }
    }
}
