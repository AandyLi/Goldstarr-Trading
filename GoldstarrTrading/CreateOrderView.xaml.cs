using GoldstarrTrading.Classes;
using Microsoft.Toolkit.Uwp.UI.Controls;
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Diagnostics;
using System.Globalization;
using System.IO;
using System.Linq;
using System.Runtime.InteropServices.WindowsRuntime;
using System.Security.Cryptography;
using System.Threading.Tasks;
using Windows.Foundation;
using Windows.Foundation.Collections;
using Windows.UI;
using Windows.UI.Core;
using Windows.UI.Popups;
using Windows.UI.Xaml;
using Windows.UI.Xaml.Controls;
using Windows.UI.Xaml.Controls.Primitives;
using Windows.UI.Xaml.Data;
using Windows.UI.Xaml.Input;
using Windows.UI.Xaml.Media;
using Windows.UI.Xaml.Media.Animation;
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
        private bool isOrderPending;

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
            foreach (var item in vm.ObsMerch)
            {
                localMerchandise.Add(item);
            }
        }

        private async void MerchCombo_SelectionChanged(object sender, SelectionChangedEventArgs e)
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

            AmountDropDown.Visibility = Visibility.Visible;
            AmountDropDown.IsEnabled = true;
            AmountTextBox.Visibility = Visibility.Collapsed;
            ClearAmountTextBox();

            var message = "Product has been added to order";
            MessageDialog messageDialog = new MessageDialog(message);

            if (tmpMerchModel.Amount <= 0)
            {
                message = "This product is not currently in stock! If you proceed with the order, it will be added to the pending order queue.";
                messageDialog = new MessageDialog(message, "Warning!");
                await messageDialog.ShowAsync();

                AmountDropDown.IsEnabled = false;
                AmountDropDown.Visibility = Visibility.Collapsed;
                AmountTextBox.Visibility = Visibility.Visible;
                return;
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

        private void ResetAddOrderButton()
        {
            ConfirmOrderButton.IsEnabled = false;
            ConfirmOrderButton.Opacity = 0.5;
        }

        private void ClearAmountDropDown()
        {
            AmountDropDown.Items.Clear();
        }

        private void ClearAmountTextBox()
        {
            AmountTextBox.Text = string.Empty;
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

        private void AmountTextBox_TextChanged(object sender, TextChangedEventArgs e)
        {
            if (AmountTextBox.Text != "" && UInt32.TryParse(AmountTextBox.Text, out uint orderedAmount) && orderedAmount > 0)
            {
                ConfirmOrderButton.IsEnabled = true;
                ConfirmOrderButton.Opacity = 1.0;
                isOrderPending = true;
            }
            else
            {
                ConfirmOrderButton.IsEnabled = false;
                ConfirmOrderButton.Opacity = 0.5;
                isOrderPending = false;
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

        private async void ConfirmOrderButton_Click(object sender, RoutedEventArgs e) //lägg till skriva till fil (i båda satser)
        {
            ConfirmOrderButton.IsEnabled = false;
            OrderModel newOrder = new OrderModel();
            OrderModel pendingOrder = new OrderModel();
            CustomerModel tmpCustModel = CustomerCombo.SelectedItem as CustomerModel;

            MerchandiseModel tmpMerchModel = GetMerchModel(MerchCombo);

            int orderedAmount;

            if (isOrderPending)
            {
                orderedAmount = Int32.Parse(AmountTextBox.Text);
                pendingOrder.CreateOrder(tmpCustModel.Name, tmpMerchModel, orderedAmount, true);

                vm.PendingOrder.Insert(0, pendingOrder);

                ResetAddOrderButton();
                ClearAmountTextBox();
            }

            else
            {
                orderedAmount = Int32.Parse(AmountDropDown.SelectedItem.ToString());
                tmpMerchModel.RemoveStock(orderedAmount);

                newOrder.CreateOrder(tmpCustModel.Name, tmpMerchModel, orderedAmount);

                vm.Order.Insert(0, newOrder);
                await Task.Delay(500);

                await FileManager.SaveToFile(vm.ObsMerch);

                ClearAmountDropDown();
                UpdateAmountDropDown(tmpMerchModel);

                ResetOrderHistoryItemsSource();
            }
        }

        private async void ConfirmPendingOrderButton_Click(object sender, RoutedEventArgs e) //this is for each ConfirmPendingOrderButton child
        {
            var parent = (sender as Button).Parent;
            Grid grid = (Grid)parent;
            OrderModel tmpPendingOrder = grid.DataContext as OrderModel;

            tmpPendingOrder.Merch = vm.ObsMerch.First(x => x.ProductName == tmpPendingOrder.Merch.ProductName);

            var message = "Pending order will be forwarded to warehouse and saved in Order History.";
            MessageDialog messageDialog = new MessageDialog(message, "Forwarding pending order");
            await messageDialog.ShowAsync();


            tmpPendingOrder.Merch.RemoveStock(tmpPendingOrder.OrderedAmount);
            vm.PendingOrder.Remove(tmpPendingOrder);
            await Task.Delay(500);
            vm.Order.Insert(0, tmpPendingOrder);

            tmpPendingOrder.IsPendingOrder = false;
            await Task.Delay(500);
            await FileManager.SaveToFile(vm.ObsMerch);

            await Task.Delay(50);
        }

        private void PendingOrdersList_Loaded(object sender, RoutedEventArgs e)
        {
            var parent = (sender as Button).Parent;
            Grid grid = (Grid)parent;
            OrderModel tmpPendingOrder = grid.DataContext as OrderModel;

            if (tmpPendingOrder != null)
            {
                try
                {
                    MerchandiseModel tmpMerch = vm.ObsMerch.First(x => x.ProductName == tmpPendingOrder.Merch.ProductName);
                    if (tmpMerch.Amount >= tmpPendingOrder.OrderedAmount)
                    {
                        (sender as Button).IsEnabled = true;
                    }
                }
                catch (Exception)
                {
                    Debug.WriteLine("Cant find a pending merchandise in our merchandise list");
                }
            }
        }

        private void ResetOrderHistoryItemsSource()
        {
            OrderHistory.ItemsSource = vm.Order;
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
