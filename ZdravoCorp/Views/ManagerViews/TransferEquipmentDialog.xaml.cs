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
using ZdravoCorp.Models.Services.ManagerServices;
using ZdravoCorp.ViewModels.ManagerViewModels;

namespace ZdravoCorp.Views  
{
    /// <summary>
    /// Interaction logic for HospitalEquipment.xaml
    /// </summary>
    public partial class TransferEquipmentDialog : Window
    {
        //private List<int> allRoomsIds;
        private ManagerService managerService;
        private EquipmentTransferViewModel equipmentTransferViewModel;
        private Room room;
        private string selectedItem;

        public TransferEquipmentDialog(Room room, string selectedItem)
        {
            {
                InitializeComponent();
                equipmentTransferViewModel = new EquipmentTransferViewModel("Pera", "Peric", "username1", "pass1");
                managerService = new ManagerService();
                //allRoomsIds = new List<int>();
                this.room = room;
                this.selectedItem = selectedItem;

                TextBoxEquipmentModel.Text = selectedItem;
                TextBoxTo.Text = room.Id.ToString();


                equipmentTransferViewModel.FilterRoomIds(room, selectedItem);
                DataContext = equipmentTransferViewModel;
            }
        }

        private void CancelButton_Click(object sender, RoutedEventArgs e)
        {
            Close();
        }

        private void MoveEquipmentButton_Click(object sender, RoutedEventArgs e)
        {
            Room roomTo = room;
            Room roomFrom = equipmentTransferViewModel.FindRoomById((int)FilteringType.SelectedValue);
            selectedItem = TextBoxEquipmentModel.Text;
            int amount = Convert.ToInt32(TextBoxAmount.Text);
            
            Console.WriteLine(roomTo.ToString());
            Console.WriteLine(roomFrom.ToString());
            Console.WriteLine(selectedItem);
            Console.WriteLine(amount);

            equipmentTransferViewModel.MoveEquipment(roomTo, roomFrom, selectedItem, amount);
        }

        private void FilteringType_OnSelectionChanged(object sender, SelectionChangedEventArgs e)
        {
            if (FilteringType.SelectedValue != null)
            {
                int selectedId = (int)FilteringType.SelectedValue;
                equipmentTransferViewModel.FindRoomById(selectedId);
            }
        }
    }
}
