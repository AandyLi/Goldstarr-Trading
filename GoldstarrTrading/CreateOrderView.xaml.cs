using GoldstarrTrading.Classes;
using Microsoft.Toolkit.Uwp.UI.Controls;
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Diagnostics;
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

// The Blank Page item template is documented at https://go.microsoft.com/fwlink/?LinkId=234238

namespace GoldstarrTrading
{
    /// <summary>
    /// An empty page that can be used on its own or navigated to within a Frame.
    /// </summary>
    public sealed partial class CreateOrderView : Page
    {
        private ViewModel vm { get; set; }
        private ObservableCollection<MerchandiseModel> localMerchandise;
        public CreateOrderView()
        {
            this.InitializeComponent();

        }

        protected override void OnNavigatedTo(NavigationEventArgs e)
        {
            vm = (ViewModel)e.Parameter;
            PopulateAvailableMerchandise();
        }

        private void PopulateAvailableMerchandise()
        {
            localMerchandise = new ObservableCollection<MerchandiseModel>();
            foreach (var item in vm.ObsMerch.Where(x => x.Amount > 0))
            {
                localMerchandise.Add(item);
            }
        }

       
        public static ObservableCollection<MerchandiseModel> Filter(ObservableCollection<MerchandiseModel> merch)

            => (ObservableCollection<MerchandiseModel>)merch.Where(x => x.Amount > 0);
            
        

        private void MerchCombo_SelectionChanged(object sender, SelectionChangedEventArgs e)
        {
            MerchandiseModel tmpMerchModel = GetMerchModel(sender);

            ClearAmountDropDown();

            if (tmpMerchModel != null)
            {
                for (int i = 0; i < tmpMerchModel.Amount; i++)
                {
                    AmountDropDown.Items.Add(i + 1);
                }
            }

            // Always reset button when we change merchandise
            ResetAddOrderButton();
        }

        private void UpdateAmountDropDown(MerchandiseModel merch)
        {
            ClearAmountDropDown();

            for (int i = 0; i < merch.Amount; i++)
            {
                AmountDropDown.Items.Add(i + 1);
            }
            AmountDropDown.SelectedIndex = 0;
        }

        private void UpdateMerchCombo(MerchandiseModel merch)
        {
            if (merch.Amount == 0)
            {
                localMerchandise.Remove(merch);
            }
        }

        private void ResetAddOrderButton()
        {
            ConfirmOrderButton.IsEnabled = false;
            ConfirmOrderButton.Opacity = 0.5;
        }

        private void ClearAmountDropDown()
        {
            AmountDropDown.Items.Clear();
        }

        private MerchandiseModel GetMerchModel(object obj)
        {
            var container = obj as ComboBox;
            var selectedItem = container.SelectedItem as MerchandiseModel;

            return selectedItem;
        }

        private void AmountDropDown_SelectionChanged(object sender, SelectionChangedEventArgs e)
        {
            if (CustomerCombo.SelectedValue?.ToString() != "" 
                && CustomerCombo.SelectedValue != null)
            {
                ConfirmOrderButton.IsEnabled = true;
                ConfirmOrderButton.Opacity = 1.0;
            }
        }

        private void CustomerCombo_SelectionChanged(object sender, SelectionChangedEventArgs e)
        {
            if (AmountDropDown.Items.Count > 0)
            {
                ConfirmOrderButton.IsEnabled = true;
                ConfirmOrderButton.Opacity = 1.0;
            }
        }

        private void ConfirmOrderButton_Click(object sender, RoutedEventArgs e)
        {
            OrderModel newOrder = new OrderModel();
            CustomerModel tmpCustModel = CustomerCombo.SelectedItem as CustomerModel;

            MerchandiseModel tmpMerchModel = GetMerchModel(MerchCombo);

            int orderedAmount = orderedAmount = Int32.Parse(AmountDropDown.SelectedItem.ToString());
            tmpMerchModel.RemoveStock(orderedAmount);
            ClearAmountDropDown();

            newOrder.CreateOrder(tmpCustModel.Name, tmpMerchModel, orderedAmount);

            vm.Order.Insert(0, newOrder);

            UpdateAmountDropDown(tmpMerchModel);
            UpdateMerchCombo(tmpMerchModel);
        }

        private void Order_Sorting(object sender, DataGridColumnEventArgs e)
        {
            //Use the Tag property to pass the bound column name for the sorting implementation

            if (e.Column.Tag.ToString() == "Customer")
            {
                //Implement sort on the column using LINQ
                if (e.Column.SortDirection == null)
                {
                    OrderHistory.ItemsSource = new ObservableCollection<OrderModel>(from item in vm.Order
                                                                                       orderby item.CustomerName ascending
                                                                                       select item);

                    e.Column.SortDirection = DataGridSortDirection.Ascending;
                }
                else if (e.Column.SortDirection == DataGridSortDirection.Ascending)
                {
                    OrderHistory.ItemsSource = new ObservableCollection<OrderModel>(from item in vm.Order
                                                                                       orderby item.CustomerName descending
                                                                                       select item);

                    e.Column.SortDirection = DataGridSortDirection.Descending;
                }
                else
                {
                    OrderHistory.ItemsSource = vm.Order;
                    e.Column.SortDirection = null;
                }
            }

            if (e.Column.Tag.ToString() == "Merchandise")
            {
                if (e.Column.SortDirection == null)
                {
                    OrderHistory.ItemsSource = new ObservableCollection<OrderModel>(from item in vm.Order
                                                                                       orderby item.Merch.ProductName ascending
                                                                                       select item);

                    e.Column.SortDirection = DataGridSortDirection.Ascending;
                }
                else if (e.Column.SortDirection == DataGridSortDirection.Ascending)
                {
                    OrderHistory.ItemsSource = new ObservableCollection<OrderModel>(from item in vm.Order
                                                                                       orderby item.Merch.ProductName descending
                                                                                       select item);

                    e.Column.SortDirection = DataGridSortDirection.Descending;
                }
                else
                {
                    OrderHistory.ItemsSource = vm.Order;
                    e.Column.SortDirection = null;
                }
            }

            if (e.Column.Tag.ToString() == "Amount")
            {
                if (e.Column.SortDirection == null)
                {
                    OrderHistory.ItemsSource = new ObservableCollection<OrderModel>(from item in vm.Order
                                                                                       orderby item.OrderedAmount ascending
                                                                                       select item);

                    e.Column.SortDirection = DataGridSortDirection.Ascending;
                }
                else if (e.Column.SortDirection == DataGridSortDirection.Ascending)
                {
                    OrderHistory.ItemsSource = new ObservableCollection<OrderModel>(from item in vm.Order
                                                                                       orderby item.OrderedAmount descending
                                                                                       select item);

                    e.Column.SortDirection = DataGridSortDirection.Descending;
                }
                else
                {
                    OrderHistory.ItemsSource = vm.Order;
                    e.Column.SortDirection = null;
                }
            }

            if (e.Column.Tag.ToString() == "Date")
            {
                if (e.Column.SortDirection == null)
                {
                    OrderHistory.ItemsSource = new ObservableCollection<OrderModel>(from item in vm.Order
                                                                                       orderby item.OrderDate ascending
                                                                                       select item);

                    e.Column.SortDirection = DataGridSortDirection.Ascending;
                }
                else if (e.Column.SortDirection == DataGridSortDirection.Ascending)
                {
                    OrderHistory.ItemsSource = new ObservableCollection<OrderModel>(from item in vm.Order
                                                                                       orderby item.OrderDate descending
                                                                                       select item);

                    e.Column.SortDirection = DataGridSortDirection.Descending;
                }
                else
                {
                    OrderHistory.ItemsSource = vm.Order;
                    e.Column.SortDirection = null;
                }
            }



            // add code to handle sorting by other columns as required

            // Remove sorting indicators from other columns
            foreach (var dgColumn in OrderHistory.Columns)
            {
                if (dgColumn.Tag.ToString() != e.Column.Tag.ToString())
                {
                    dgColumn.SortDirection = null;
                }
            }
        }
    }
}
