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
using GoldstarrTrading.Classes;
using System.Diagnostics;
using System.ComponentModel;
using System.Collections.ObjectModel;
using Windows.UI.Popups;

// The Blank Page item template is documented at https://go.microsoft.com/fwlink/?LinkId=234238

namespace GoldstarrTrading
{
    /// <summary>
    /// An empty page that can be used on its own or navigated to within a Frame.
    /// </summary>
    public sealed partial class Deliveries : Page
    {
        private ViewModel ViewModel { get; set; }
        private string SelectedProductName{get;set;}
        private int DeliveryQuantity { get; set; }


        public Deliveries()
        {
            this.InitializeComponent();
        }
        protected override void OnNavigatedTo(NavigationEventArgs e)
        {
            FillDeliveriesList(e);
            FillDeliveryComboBox(e);
        }

        private void FillDeliveriesList(NavigationEventArgs e)
        {
            deliveriesList.ItemsSource = ((ViewModel)e.Parameter).ObsMerch;
        }

        private void FillDeliveryComboBox(NavigationEventArgs e)
        {
            ViewModel = (ViewModel)e.Parameter;

            foreach (var product in ViewModel.ObsMerch)
            {
                deliveryComboBox.Items.Add(product.ProductName);
            }
        }

        private void Add_Click(object sender, RoutedEventArgs e)
        {
            AddDeliveryToStock();
        }

        private void AddDeliveryToStock()
        {
            SelectedProductName = deliveryComboBox.SelectedValue.ToString();

            DeliveryQuantity = CheckDeliveryQuantity();

            for (int i = 0; i < ViewModel.ObsMerch.Count; ++i)
            {
                if (ViewModel.ObsMerch[i].ProductName == SelectedProductName)
                {
                    ViewModel.ObsMerch[i].AddStock(DeliveryQuantity);
                }
            }
        }

        private int CheckDeliveryQuantity()
        {
            int deliveryQuantity = 0;

            try
            {
                deliveryQuantity = Int32.Parse(deliveryTextBox.Text);
            }
            catch (FormatException fex)
            {
                var messageDialog = new MessageDialog(fex.Message);
                deliveryQuantity = 0;
                return deliveryQuantity;
            }
            catch (ArgumentOutOfRangeException)
            {
                deliveryQuantity = 0;
                return deliveryQuantity;
            }
            catch (Exception)
            {
                deliveryQuantity = 0;
                return deliveryQuantity;
            }

            return deliveryQuantity;
        }

        private void TextBox_TextChanged(object sender, TextChangedEventArgs e)
        {
        }
    }
}
