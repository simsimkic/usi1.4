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
    public partial class EquipmentTransfer : Window
    {
        private EquipmentTransferViewModel equipmentTransferViewModel;

        public EquipmentTransfer()
        {
            {
                InitializeComponent();

                equipmentTransferViewModel = new EquipmentTransferViewModel("Pera", "Peric", "username1", "pass1");
                DataContext = equipmentTransferViewModel;
            }
        }

        private void ButtonFilter_OnClick(object sender, RoutedEventArgs e)
        {
            string filteringType = (string)FilteringType.SelectionBoxItem;
            equipmentTransferViewModel.FilterRooms(filteringType);
        }

        private void HospitalEquipment_Click(object sender, RoutedEventArgs e)
        {
            HospitalEquipment hospitalEquipment = new HospitalEquipment();
            hospitalEquipment.Show();
            Close();
        }

        private void EquipmentManagement_Click(object sender, RoutedEventArgs e)
        {
            EquipmentManagement equipmentManagement = new EquipmentManagement();
            equipmentManagement.Show();
            Close();
        }

        private void MoveEquipment_OnClick(object sender, RoutedEventArgs e)
        {
            Room selectedRoom = moveEquipmentGrid.SelectedItem as Room;

            // Check if a room is selected
            if (selectedRoom != null)
            {
                TransferEquipmentDialog transferEquipmentDialog = new TransferEquipmentDialog(selectedRoom, (string)FilteringType.SelectionBoxItem);
                transferEquipmentDialog.ShowDialog();
            }
        }
    }
}
