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
using ZdravoCorp.Models.Services;
using ZdravoCorp.ViewModels.ManagerViewModels;

namespace ZdravoCorp.Views
{
    /// <summary>
    /// Interaction logic for HospitalEquipment.xaml
    /// </summary>
    public partial class HospitalEquipment : Window
    {
        private HospitalEquipmentViewModel hospitalEquipmentViewModel;
        public HospitalEquipment()
        {
            {
                InitializeComponent();

                hospitalEquipmentViewModel = new HospitalEquipmentViewModel("Pera", "Peric", "username1", "pass1");
                DataContext = hospitalEquipmentViewModel;
            }
        }
        
        private void ButtonFilter_OnClick(object sender, RoutedEventArgs e)
        {
            string filteringType = (string)FilteringSecondType.SelectionBoxItem;
            string quantity = (string)FilteringQuantityType.SelectionBoxItem;

            hospitalEquipmentViewModel.FilterEquipment(filteringType, FilteringType.SelectedIndex, quantity);
        }

        private void ButtonReset_OnClick(object sender, RoutedEventArgs e)
        {
            hospitalEquipmentViewModel.LoadEquipment();
        }

        private void FilteringType_OnSelectionChanged(object sender, SelectionChangedEventArgs e)
        {
            int selectedIndex = FilteringType.SelectedIndex;
            Console.WriteLine(selectedIndex);
            
            hospitalEquipmentViewModel.UpdateFiltringSecondType(selectedIndex);
            hospitalEquipmentViewModel.UpdateFilteringQuantityType(selectedIndex);
        }

        private void SearchBar_TextChanged(object sender, TextChangedEventArgs e)
        {
            string searchText = textBoxFilter.Text.ToLower();

            hospitalEquipmentViewModel.FilterByText(searchText);
        }

        private void EquipmentTransfer_Click(object sender, RoutedEventArgs e)
        {
            EquipmentTransfer equipmentTransfer = new EquipmentTransfer();
            equipmentTransfer.Show();
            Close();
        }

        private void EquipmentManagement_Click(object sender, RoutedEventArgs e)
        {
            EquipmentManagement equipmentManagement = new EquipmentManagement();
            equipmentManagement.Show();
            Close();
        }
    }
}
