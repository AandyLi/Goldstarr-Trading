using GoldstarrTrading.Classes;
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

            AmountDropDown.Items.Clear();

            for (int i = 0; i < tmpMerchModel.Amount; i++)
            {
                AmountDropDown.Items.Add(i + 1);
            }

            // Always reset button when we change merchandise
            ConfirmOrderButton.IsEnabled = false;
            ConfirmOrderButton.Opacity = 0.5;
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
            MerchandiseModel tmpMerchModel = GetMerchModel(MerchCombo);

            CustomerModel tmpCustModel = CustomerCombo.SelectedItem as CustomerModel;

            int orderedAmount = orderedAmount = Int32.Parse(AmountDropDown.SelectedItem.ToString());

            newOrder.CreateOrder(tmpCustModel.Name, tmpMerchModel, orderedAmount);

            vm.Order.Add(newOrder);
        }
    }
}
