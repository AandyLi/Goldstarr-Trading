using GoldstarrTrading.Classes;
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Diagnostics;
using System.Globalization;
using System.IO;
using System.Linq;
using System.Runtime.InteropServices.WindowsRuntime;
using System.Security.Cryptography;
using Windows.Foundation;
using Windows.Foundation.Collections;
using Windows.UI;
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
            //ButtonBlink.Stop();
        }

       
        private void PopulateAvailableMerchandise()
        {
            localMerchandise = new ObservableCollection<MerchandiseModel>();
            foreach (var item in vm.ObsMerch)
            {
                localMerchandise.Add(item);
            }

        }


        //public static ObservableCollection<MerchandiseModel> Filter(ObservableCollection<MerchandiseModel> merch)

        //    => (ObservableCollection<MerchandiseModel>)merch.Where(x => x.Amount > 0);



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

            var message = string.Empty;
            MessageDialog messageDialog = new MessageDialog(message, "Product has been added to order");

            if (tmpMerchModel.Amount <= 0)
            {
                message = "This product is not currently in stock! If you proceed with the order, it will be added to the pending order queue.";
                messageDialog = new MessageDialog(message, "Warning!");
                await messageDialog.ShowAsync();

                AmountDropDown.IsEnabled = false;
                AmountDropDown.Visibility = Visibility.Collapsed;
                AmountTextBox.Visibility = Visibility.Visible;
                isOrderPending = true;
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

        private void UpdateMerchCombo(MerchandiseModel merch)
        {
            if (merch.Amount > 0)
            {
                localMerchandise.Add(merch);
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

        private void AmountTextBox_TextChanged(object sender, TextChangedEventArgs e)
        {
            if (AmountTextBox.Text != "" && Int32.TryParse(AmountTextBox.Text, out int orderedAmount))
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
            OrderModel pendingOrder = new OrderModel();
            CustomerModel tmpCustModel = CustomerCombo.SelectedItem as CustomerModel;

            MerchandiseModel tmpMerchModel = GetMerchModel(MerchCombo);


            int orderedAmount;

            if (isOrderPending)
            {
                orderedAmount = Int32.Parse(AmountTextBox.Text);
                pendingOrder.CreateOrder(tmpCustModel.Name, tmpMerchModel, orderedAmount);

                vm.PendingOrder.Insert(0, pendingOrder);

                ResetAddOrderButton();
                ClearAmountDropDown();
            }

            else
            {
                orderedAmount = Int32.Parse(AmountDropDown.SelectedItem.ToString());
                tmpMerchModel.RemoveStock(orderedAmount);
                ClearAmountDropDown();

                newOrder.CreateOrder(tmpCustModel.Name, tmpMerchModel, orderedAmount);

                vm.Order.Insert(0, newOrder);

            }
            
            UpdateAmountDropDown(tmpMerchModel);
            UpdateMerchCombo(tmpMerchModel);

        }

        private async void ConfirmPendingOrderButton_Click(object sender, RoutedEventArgs e) //this is for each ConfirmPendingOrderButton, not for when creating new orders
        {
            var parent = (sender as Button).Parent;
            Grid grid = (Grid)parent;
            OrderModel tmpPendingOrder = grid.DataContext as OrderModel;

            var message = "Pending order will be forwarded to warehouse and saved in Order History.";
            MessageDialog messageDialog = new MessageDialog(message, "Forwarding pending order");
            await messageDialog.ShowAsync();

            vm.PendingOrder.Remove(tmpPendingOrder);
            vm.Order.Insert(0, tmpPendingOrder);
        }

        private void PendingOrdersList_Loaded(object sender, RoutedEventArgs e)
        {
            var parent = (sender as Button).Parent;
            Grid grid = (Grid)parent;
            OrderModel tmpPendingOrder = grid.DataContext as OrderModel;

            MerchandiseModel tmpMerch = vm.ObsMerch.First(x => x.ProductName == tmpPendingOrder.Merch.ProductName);

            if (tmpMerch.Amount >= tmpPendingOrder.OrderedAmount)
            {
                (sender as Button).IsEnabled = true;
            }
        }


    }

    


}
