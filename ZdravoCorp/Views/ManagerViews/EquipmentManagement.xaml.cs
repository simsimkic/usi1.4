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
    public partial class EquipmentManagement: Window
    {
        private EquipmentManager equipmentManager;
        public EquipmentManagement()
        {
            {
                InitializeComponent();

                equipmentManager = new EquipmentManager("Pera", "Peric", "username1", "pass1");
                DataContext = equipmentManager;
            }
        }

        private void HospitalEquipment_Click(object sender, RoutedEventArgs e)
        {
            HospitalEquipment hospitalEquipment = new HospitalEquipment();
            hospitalEquipment.Show();
            Close();
        }

        private void EquipmentTransfer_Click(object sender, RoutedEventArgs e)
        {
            EquipmentTransfer equipmentTransfer = new EquipmentTransfer();
            equipmentTransfer.Show();
            Close();
        }

        private void ButtonFilter_OnClick(object sender, RoutedEventArgs e)
        {
            equipmentManager.UpdateEquipmentList();
        }

        private void ButtonOrderMore_OnClick(object sender, RoutedEventArgs e)
        {

            Equipment selectedEquipment = dataGrid.SelectedItem as Equipment;
            if (selectedEquipment != null)
            {
                string selectedModel = selectedEquipment.Model;
                Console.WriteLine(selectedEquipment.ToString());
 
                OrderEquipment orderEquipment = new OrderEquipment(selectedEquipment, equipmentManager);
                orderEquipment.Show();
            }
        }
    }
}
