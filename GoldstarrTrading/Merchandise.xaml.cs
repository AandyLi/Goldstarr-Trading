using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Runtime.InteropServices.WindowsRuntime;
using Windows.Foundation;
using Windows.Foundation.Collections;
using Windows.UI.Xaml;
using Windows.UI.Xaml.Controls;
using Windows.UI.Xaml.Controls.Primitives;
using Windows.UI.Xaml.Data;
using Windows.UI.Xaml.Input;
using Windows.UI.Xaml.Media;
using Windows.UI.Xaml.Navigation;
using GoldstarrTrading.Classes;
using System.Collections.ObjectModel;
using Windows.UI.Popups;
using System.Threading.Tasks;

// The Blank Page item template is documented at https://go.microsoft.com/fwlink/?LinkId=234238

namespace GoldstarrTrading
{
    
    /// <summary>
    /// An empty page that can be used on its own or navigated to within a Frame.
    /// </summary>
    public sealed partial class Merchandise : Page
    {
        
        private ViewModel vm { get; set; }
        string message = string.Empty;
        MessageDialog messageDialog;

        /// <summary>
        /// Binding Merchandise list to merchandiseList listview
        /// </summary>
        public Merchandise() 
        {
            this.InitializeComponent();
        }

        protected override void OnNavigatedTo(NavigationEventArgs e)
        {
            vm = ((ViewModel)e.Parameter);
            
            merchandiseList.ItemsSource = vm.ObsMerch;
            
            SupplierNameListComboBox.ItemsSource = ((ViewModel)e.Parameter).Supplier;
           
        }

        private async void AddProductsButton_Click(object sender, RoutedEventArgs e)
        {
            message = "New product has been added";
            messageDialog = new MessageDialog("Information",message);

            var supplier = string.Empty;
            var product = string.Empty;

            if ((SupplierNameListComboBox.SelectedValue == null) && string.IsNullOrEmpty(ProductNameTextBox.Text))
            {
                message = "Data is missing";
                messageDialog = new MessageDialog(message, "Information");
                await messageDialog.ShowAsync();
                return;
            }
            if (SupplierNameListComboBox.SelectedValue == null)
            {
                message = "Supplier information is missing";
                messageDialog = new MessageDialog(message, "Information");
                await messageDialog.ShowAsync();
                return;
            }
            if (string.IsNullOrEmpty(ProductNameTextBox.Text))
            {
                message = "Product information is missing";
                messageDialog = new MessageDialog(message, "Information");
                await messageDialog.ShowAsync();
                return;
            }
            
            supplier = SupplierNameListComboBox.SelectedValue.ToString();
            product = ProductNameTextBox.Text;
            if (vm.ObsMerch.Any(v => v.ProductName.ToLower().Trim() == product.ToLower().Trim()))
            {
                message = "The product already exists";
                messageDialog = new MessageDialog(message, "Information");
                await messageDialog.ShowAsync();
                ProductNameTextBox.Text = string.Empty;
                return;
            }

            vm.ObsMerch.Add(new MerchandiseModel() { Supplier = supplier, ProductName = product });
            await messageDialog.ShowAsync();
            SupplierNameListComboBox.SelectedIndex = -1;
            ProductNameTextBox.Text = string.Empty;
        }

        private async void SortListButton_Click(object sender, RoutedEventArgs e)
        {
            
             
            if (OrderBy.SelectionBoxItem == null && SortBy.SelectionBoxItem == null)
            {
                message = "Selection is missing";
                messageDialog = new MessageDialog(message, "Information");
                await messageDialog.ShowAsync();
                return;
            }
            if(OrderBy.SelectionBoxItem == null)
            {
                message = "Selection Order by is missing";
                messageDialog = new MessageDialog(message, "Information");
                await messageDialog.ShowAsync();
                return;
            }
            if(SortBy.SelectionBoxItem == null)
            {
                message = "Selection Sort by is missing";
                messageDialog = new MessageDialog(message, "Information");
                await messageDialog.ShowAsync();
                return;
            }
            var orderBy = OrderBy.SelectionBoxItem.ToString();
            var sortBy = SortBy.SelectionBoxItem.ToString();
            switch (sortBy.ToLower())
            {
                case "product":
                    {
                        if (orderBy.ToLower() == "ascending")
                        {
                            vm.ObsMerch = new ObservableCollection<MerchandiseModel>(vm.ObsMerch.OrderBy(v => v.ProductName));
                        }
                        else 
                        {
                            vm.ObsMerch = new ObservableCollection<MerchandiseModel>(vm.ObsMerch.OrderByDescending(v => v.ProductName));
                        }
                        merchandiseList.ItemsSource = vm.ObsMerch;
                        break;
                    }
                case "supplier":
                    {
                        if (orderBy.ToLower() == "ascending")
                        {
                            vm.ObsMerch = new ObservableCollection<MerchandiseModel>(vm.ObsMerch.OrderBy(v => v.Supplier));
                        }
                        else
                        {
                            vm.ObsMerch = new ObservableCollection<MerchandiseModel>(vm.ObsMerch.OrderByDescending(v => v.Supplier));
                        }
                        merchandiseList.ItemsSource = vm.ObsMerch;
                        break;
                    }
            }
        }
    }
}
