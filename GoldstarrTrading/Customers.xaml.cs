using GoldstarrTrading.Classes;
using Microsoft.Toolkit.Uwp.UI.Controls;
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
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
            if ((CustomerNameTextBox.Text.Length > 0) && (CustomerAddressTextBox.Text.Length > 0) && (CustomerPhoneTextBox.Text.Length > 0))
            {
                AddCustomerButtonVisible(true);
            }
            else
            {
                AddCustomerButtonVisible(false);
            }
        }

        private void AddCustomerButtonVisible(bool value)
        {
            if (value)
            {
                AddCustomerButton.Opacity = 1;
            }
            else
            {
                AddCustomerButton.Opacity = 0.5;
            }

            AddCustomerButton.IsEnabled = value;
        }
        private bool CheckDuplicateCustomerEntry()
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
                newCustomerName = CustomerNameTextBox.Text;
                newCustomerAddress = CustomerAddressTextBox.Text;
                newCustomerPhone = CustomerPhoneTextBox.Text;

                if (!IsPhoneNumber(newCustomerPhone))
                {
                    throw new FormatException("Phone number can only contain numeric values and be 8-10 characters long.");
                }
            }
            catch (FormatException fex)
            {
                DisplayInputError(fex);
                return false;
            }
            catch (ArgumentOutOfRangeException aex)
            {
                DisplayInputError(aex);
                return false;
            }
            catch (Exception e)
            {
                DisplayInputError(e);
                return false;
            }
            return true;
        }
       

        private async void DisplayInputError(Exception exception)
        {
            ContentDialog inputError = new ContentDialog()
            {
                Title = "Input Error",
                Content = exception.Message,
                CloseButtonText = "OK"
            };
            await inputError.ShowAsync();
        }

        private async void CustomerAddedDialog()
        {
            ContentDialog inputError = new ContentDialog()
            {
                Title = "Customer Added",
                Content = $"New customer {newCustomerName} was successfully added to the Customer Directory.",
                CloseButtonText = "OK"
            };

            await inputError.ShowAsync();
        }

        private async void DuplicateEntryDialog()
        {
            ContentDialog inputError = new ContentDialog()
            {
                Title = "Duplicate Customer",
                Content = $"Customer is already in the Customer Directory.",
                CloseButtonText = "OK"
            };

            await inputError.ShowAsync();
        }
       
        private void EmptyAllTextboxes()
        {
            CustomerNameTextBox.Text = string.Empty;
            CustomerAddressTextBox.Text = string.Empty;
            CustomerPhoneTextBox.Text = string.Empty;
        }
        private void AddCustomerButton_Click(object sender, RoutedEventArgs e)
        {
            if (CheckTextBoxFormat() == true)
            {
                if (CheckDuplicateCustomerEntry() == false)
                {
                    vm.CustomerList.Add(new CustomerModel() { Name = newCustomerName, Address = newCustomerAddress, Phone = newCustomerPhone });

                    CustomerAddedDialog();
                }
                else
                {
                    DuplicateEntryDialog();
                }
            }

            EmptyAllTextboxes();
            ResetCustomerListItemsSource();
        }

        private void ResetCustomerListItemsSource()
        {
            customerList.ItemsSource = vm.CustomerList;
        }

        private void Customer_Sorting(object sender, DataGridColumnEventArgs e)
        {
            //Use the Tag property to pass the bound column name for the sorting implementation

            if (e.Column.Tag.ToString() == "Name")
            {
                //Implement sort on the column using LINQ
                if (e.Column.SortDirection == null)
                {
                    customerList.ItemsSource = new ObservableCollection<CustomerModel>(from item in vm.CustomerList
                                                                                       orderby item.Name ascending
                                                                                       select item);

                    e.Column.SortDirection = DataGridSortDirection.Ascending;
                }
                else if (e.Column.SortDirection == DataGridSortDirection.Ascending)
                {
                    customerList.ItemsSource = new ObservableCollection<CustomerModel>(from item in vm.CustomerList
                                                                                       orderby item.Name descending
                                                                                       select item);

                    e.Column.SortDirection = DataGridSortDirection.Descending;
                }
                else
                {
                    customerList.ItemsSource = vm.CustomerList;
                    e.Column.SortDirection = null;
                }
            }

            // add code to handle sorting by other columns as required

            // Remove sorting indicators from other columns
            foreach (var dgColumn in customerList.Columns)
            {
                if (dgColumn.Tag.ToString() != e.Column.Tag.ToString())
                {
                    dgColumn.SortDirection = null;
                }
            }
        

       
        

        }
    }
}

