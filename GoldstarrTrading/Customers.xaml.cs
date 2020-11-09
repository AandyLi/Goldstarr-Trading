using GoldstarrTrading.Classes;
using Microsoft.Toolkit.Uwp.UI.Controls;
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.IO;
using System.Linq;
using System.Runtime.InteropServices.WindowsRuntime;
using System.Security.Cryptography.X509Certificates;
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
        private ViewModel vm { get; set; }
        protected override void OnNavigatedTo(NavigationEventArgs e)
        {
            vm = (ViewModel)e.Parameter;

        }
        public Customers()
        {
            this.InitializeComponent();
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

        private void AddCustomerButton_Click(object sender, RoutedEventArgs e)
        {

        }

        private void TextBox_TextChanged(object sender, TextChangedEventArgs e)
        {

        }
    }
}

