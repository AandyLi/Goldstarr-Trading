using GoldstarrTrading.Classes;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Runtime.InteropServices.WindowsRuntime;
using System.Security.Cryptography.X509Certificates;
using System.Text.RegularExpressions;
using Windows.Foundation;
using Windows.Foundation.Collections;
using Windows.UI.Xaml;
using Windows.UI.Xaml.Controls;
using Windows.UI.Xaml.Controls.Primitives;
using Windows.UI.Xaml.Data;
using Windows.UI.Xaml.Input;
using Windows.UI.Xaml.Media;
using Windows.UI.Xaml.Navigation;

// The Blank Page item template is documented at https://go.microsoft.com/fwlink/?LinkId=234238

namespace GoldstarrTrading
{
    /// <summary>
    /// An empty page that can be used on its own or navigated to within a Frame.
    /// </summary>
    
    public sealed partial class Customers : Page

    {
        private string newCustomerName;
        private string newCustomerAddress;
        private string newCustomerPhone;
        private ViewModel vm { get; set; }
        protected override void OnNavigatedTo(NavigationEventArgs e)
        {
            vm = (ViewModel)e.Parameter;

        }
        public Customers()
        {
            this.InitializeComponent();
        }
        private void TextBox_TextChanged(object sender, TextChangedEventArgs e)
        {
            if ((SupplierNameTextBox.Text.Length > 0) && (SupplierAddressTextBox.Text.Length > 0) && (SupplierPhoneTextBox.Text.Length > 0))
            {
                AddSupplierButtonVisible(true);
            }
            else
            {
                AddSupplierButtonVisible(false);
            }
        }

        private void AddSupplierButtonVisible(bool value)
        {
            if (value)
            {
                AddSupplierButton.Opacity = 1;
            }
            else
            {
                AddSupplierButton.Opacity = 0.5;
            }

            AddSupplierButton.IsEnabled = value;
        }
        private bool CheckDuplicateSupplierEntry()
        {

            foreach (var supplier in vm.CustomerList)
            {
                if (supplier.Name == newCustomerName)
                {
                    return true;
                }
            }
            return false;
        }

        public static bool IsPhoneNumber(string number)
        {
            return Regex.Match(number, @"^[0-9]{8,10}$").Success;

        }

        private bool CheckTextBoxFormat()
        {

            try
            {
                newCustomerName = SupplierNameTextBox.Text;
                newCustomerAddress = SupplierAddressTextBox.Text;
                newCustomerPhone = SupplierPhoneTextBox.Text;

                if (!IsPhoneNumber(newCustomerPhone))
                {
                    throw new FormatException("Phone number can only contain numeric values and be 8-10 characters long.");
                }
            }
            catch (FormatException fex)
            {
               // DisplayInputError(fex);
                return false;
            }
            //catch (ArgumentOutOfRangeException aex)
            //{
            //    DisplayInputError(aex);
            //    return false;
            //}
            //catch (Exception e)
            //{
            //    DisplayInputError(e);
            //    return false;
            //}
            return true;
        }
        private async void SupplierAddedDialog()
        {
            ContentDialog inputError = new ContentDialog()
            {
                Title = "Supplier Added",
                Content = $"New supplier {newCustomerName} was successfully added to the Supplier Directory.",
                CloseButtonText = "OK"
            };

            await inputError.ShowAsync();
        }

        private async void DuplicateEntryDialog()
        {
            ContentDialog inputError = new ContentDialog()
            {
                Title = "Duplicate Supplier",
                Content = $"Supplier is already in the Supplier Directory.",
                CloseButtonText = "OK"
            };

            await inputError.ShowAsync();
        }
       
        private void EmptyAllTextboxes()
        {
            SupplierNameTextBox.Text = string.Empty;
            SupplierAddressTextBox.Text = string.Empty;
            SupplierPhoneTextBox.Text = string.Empty;
        }
        private void AddSupplierButton_Click(object sender, RoutedEventArgs e)
        {
            if (CheckTextBoxFormat() == true)
            {
                if (CheckDuplicateSupplierEntry() == false)
                {
                    vm.CustomerList.Add(new CustomerModel() { Name = newCustomerName, Address = newCustomerAddress, Phone = newCustomerPhone });

                    SupplierAddedDialog();
                }
                else
                {
                    DuplicateEntryDialog();
                }
            }

            EmptyAllTextboxes();
        }
    }
}

