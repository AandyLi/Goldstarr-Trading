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
using Microsoft.Toolkit.Uwp.UI.Controls;
using System.Collections.ObjectModel;
using GoldstarrTrading.Classes;

// The Blank Page item template is documented at https://go.microsoft.com/fwlink/?LinkId=234238

namespace GoldstarrTrading
{
    /// <summary>
    /// An empty page that can be used on its own or navigated to within a Frame.
    /// </summary>
    public sealed partial class Stock : Page
    {
        private ViewModel vm { get; set; }
        public Stock()
        {
            this.InitializeComponent();
        }

        private void TextBox_TextChanged(object sender, TextChangedEventArgs e)
        {

        }
        protected override void OnNavigatedTo(NavigationEventArgs e)
        {
            vm = (ViewModel)e.Parameter;

        }

        private void Stock_Sorting(object sender, DataGridColumnEventArgs e)
        {
            //Use the Tag property to pass the bound column name for the sorting implementation

            if (e.Column.Tag.ToString() == "Name")
            {
                //Implement sort on the column using LINQ
                if (e.Column.SortDirection == null)
                {
                    stockList.ItemsSource = new ObservableCollection<MerchandiseModel>(from item in vm.ObsMerch
                                                                                       orderby item.ProductName ascending
                                                                                       select item);

                    e.Column.SortDirection = DataGridSortDirection.Ascending;
                }
                else if (e.Column.SortDirection == DataGridSortDirection.Ascending)
                {
                    stockList.ItemsSource = new ObservableCollection<MerchandiseModel>(from item in vm.ObsMerch
                                                                                       orderby item.ProductName descending
                                                                                       select item);

                    e.Column.SortDirection = DataGridSortDirection.Descending;
                }
                else
                {
                    stockList.ItemsSource = vm.ObsMerch;
                    e.Column.SortDirection = null;
                }
            }

            if (e.Column.Tag.ToString() == "Amount")
            {
                if (e.Column.SortDirection == null)
                {
                    stockList.ItemsSource = new ObservableCollection<MerchandiseModel>(from item in vm.ObsMerch
                                                                                       orderby item.Amount ascending
                                                                                       select item);

                    e.Column.SortDirection = DataGridSortDirection.Ascending;
                }
                else if (e.Column.SortDirection == DataGridSortDirection.Ascending)
                {
                    stockList.ItemsSource = new ObservableCollection<MerchandiseModel>(from item in vm.ObsMerch
                                                                                       orderby item.Amount descending
                                                                                       select item);

                    e.Column.SortDirection = DataGridSortDirection.Descending;
                }
                else
                {
                    stockList.ItemsSource = vm.ObsMerch;
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
