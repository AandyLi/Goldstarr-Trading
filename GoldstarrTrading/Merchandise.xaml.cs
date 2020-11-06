﻿using System;
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
            merchandiseList.ItemsSource = ((ViewModel)e.Parameter).ObsMerch;
        }

        private async void AddProductsButton_Click(object sender, RoutedEventArgs e)
        {
            
            var message = string.Empty;
            MessageDialog messageDialog = new MessageDialog(message, "New product has been added");

            var supplier = string.Empty;
            var product = string.Empty;

            supplier = SupplierNameTextBox.Text;
            product = ProductNameTextBox.Text;

            if (string.IsNullOrEmpty(SupplierNameTextBox.Text) && string.IsNullOrEmpty(ProductNameTextBox.Text))
            {
                message = "Data is missing";
                messageDialog = new MessageDialog(message, "Information");
                await messageDialog.ShowAsync();
                return;
            }
            if (string.IsNullOrEmpty(SupplierNameTextBox.Text))
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
            char x = supplier.ToCharArray().ElementAt(0);
            if (char.IsNumber(x))
            {
                message = "Invalid Supplier data";
                messageDialog = new MessageDialog(message, "Information");
                await messageDialog.ShowAsync();
                return;
            }
            if(vm.ObsMerch.Any(v => v.ProductName.ToLower().Trim() == product.ToLower().Trim()) 
                && vm.ObsMerch.Any(v => v.Supplier.ToLower().Trim() == supplier.ToLower().Trim()))
            {
                message = "The product and the supplier already exists";
                messageDialog = new MessageDialog(message, "Information");
                await messageDialog.ShowAsync();
                SupplierNameTextBox.Text = string.Empty;
                ProductNameTextBox.Text = string.Empty;
                return;
            }
            if (vm.ObsMerch.Any(v => v.ProductName.ToLower().Trim() == product.ToLower().Trim()))
            {
                message = "The product already exists";
                messageDialog = new MessageDialog(message, "Information");
                await messageDialog.ShowAsync(); 
                ProductNameTextBox.Text = string.Empty;
                return;
            }
            if (vm.ObsMerch.Any(v => v.Supplier.ToLower().Trim() == supplier.ToLower().Trim()))
            {
                message = "The supplier already exists";
                messageDialog = new MessageDialog(message, "Information");
                await messageDialog.ShowAsync();
                SupplierNameTextBox.Text = string.Empty;
                return;
            }
            vm.ObsMerch.Add(new MerchandiseModel() { Supplier = supplier, ProductName = product });
            await messageDialog.ShowAsync();
            SupplierNameTextBox.Text = string.Empty;
            ProductNameTextBox.Text = string.Empty;
        }
    }
}
