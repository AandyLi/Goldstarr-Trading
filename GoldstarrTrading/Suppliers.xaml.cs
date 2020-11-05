﻿using GoldstarrTrading.Classes;
using Microsoft.Toolkit.Uwp.UI.Controls;
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.ComponentModel;
using System.IO;
using System.Linq;
using System.Runtime.InteropServices.WindowsRuntime;
using System.Text.RegularExpressions;
using Windows.Foundation;
using Windows.Foundation.Collections;
using Windows.UI;
using Windows.UI.Xaml;
using Windows.UI.Xaml.Controls;
using Windows.UI.Xaml.Controls.Primitives;
using Windows.UI.Xaml.Data;
using Windows.UI.Xaml.Documents;
using Windows.UI.Xaml.Input;
using Windows.UI.Xaml.Media;
using Windows.UI.Xaml.Navigation;

// The Blank Page item template is documented at https://go.microsoft.com/fwlink/?LinkId=234238

namespace GoldstarrTrading
{
    /// <summary>
    /// An empty page that can be used on its own or navigated to within a Frame.
    /// </summary>
    public sealed partial class Suppliers : Page
    {
        private ViewModel ViewModel;
        private string newSupplierName;
        private string newSupplierAddress;
        private string newSupplierPhone;


        public Suppliers()
        {
            this.InitializeComponent();
        }

        protected override void OnNavigatedTo(NavigationEventArgs e)
        {
            ViewModel = (ViewModel)e.Parameter;
        }

        private void AddSupplierButton_Click(object sender, RoutedEventArgs e)
        {
            if (CheckTextBoxFormat() == true)
            {
                if (CheckDuplicateSupplierEntry() == false)
                {
                    ViewModel.Supplier.Add(new SupplierModel() { Name = newSupplierName, Address = newSupplierAddress, Phone = newSupplierPhone });

                    SupplierAddedDialog();
                }
                else
                {
                    DuplicateEntryDialog();
                }
            }

           EmptyAllTextboxes();
           stockList.ItemsSource = ViewModel.Supplier;

        }

        private void EmptyAllTextboxes()
        {
            SupplierNameTextBox.Text = string.Empty;
            SupplierAddressTextBox.Text = string.Empty;
            SupplierPhoneTextBox.Text = string.Empty;
        }

        private bool CheckDuplicateSupplierEntry()
        {

            foreach (SupplierModel supplier in ViewModel.Supplier)
            {
                if (supplier.Name == newSupplierName)
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
                newSupplierName = SupplierNameTextBox.Text;
                newSupplierAddress = SupplierAddressTextBox.Text;
                newSupplierPhone = SupplierPhoneTextBox.Text;

                if (!IsPhoneNumber(newSupplierPhone))
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
        
        private void TextBox_TextChanged(object sender, TextChangedEventArgs e)
        {
            if ((SupplierNameTextBox.Text.Length > 0) && (SupplierAddressTextBox.Text.Length > 0) && (SupplierPhoneTextBox.Text.Length > 0))
            {
                AddSupplierButton.IsEnabled = true;
            }

        }

        private async void SupplierAddedDialog()
        {
            ContentDialog inputError = new ContentDialog()
            {
                Title = "Supplier Added",
                Content = $"New supplier {newSupplierName} was successfully added to the Supplier Directory.",
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


        private void SupplierName_Sorting(object sender, DataGridColumnEventArgs e)
        {
            //Use the Tag property to pass the bound column name for the sorting implementation 
            if (e.Column.Tag.ToString() == "Supplier Name")
            {
                //Implement sort on the column "Range" using LINQ
                if (e.Column.SortDirection == null)
                {
                    stockList.ItemsSource = new ObservableCollection<SupplierModel>(from item in ViewModel.Supplier
                                                                        orderby item.Name ascending
                                                                        select item);

                    e.Column.SortDirection = DataGridSortDirection.Ascending;
                }
                else if (e.Column.SortDirection == DataGridSortDirection.Ascending)
                {
                    stockList.ItemsSource = new ObservableCollection<SupplierModel>(from item in ViewModel.Supplier
                                                                        orderby item.Name descending
                                                                        select item);

                    e.Column.SortDirection = DataGridSortDirection.Descending;
                }
                else
                {
                    stockList.ItemsSource = ViewModel.Supplier;
                    e.Column.SortDirection = null;
                }
            }
            // add code to handle sorting by other columns as required

            // Remove sorting indicators from other columns
            foreach (var dgColumn in stockList.Columns)
            {
                if (dgColumn.Tag.ToString() != e.Column.Tag.ToString())
                {
                    dgColumn.SortDirection = null;
                }
            }
        }

    }
}
