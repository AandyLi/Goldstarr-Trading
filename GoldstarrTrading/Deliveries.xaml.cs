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

// The Blank Page item template is documented at https://go.microsoft.com/fwlink/?LinkId=234238

namespace GoldstarrTrading
{
    /// <summary>
    /// An empty page that can be used on its own or navigated to within a Frame.
    /// </summary>
    public sealed partial class Deliveries : Page
    {
        private ViewModel ViewModel { get; set; }

        public Deliveries()
        {
            this.InitializeComponent();
        }
        protected override void OnNavigatedTo(NavigationEventArgs e)
        {
            deliveriesList.ItemsSource = ((ViewModel)e.Parameter).ObsMerch;

            ViewModel = (ViewModel)e.Parameter;

            foreach (var product in ViewModel.ObsMerch)
            {
                deliveryComboBox.Items.Add(product.ProductName);
            }
        }

        private void Add_Click(object sender, RoutedEventArgs e)
        {
            string selectedProductName = deliveryComboBox.SelectedValue.ToString();

            int quantity = Int32.Parse(deliveryTextBox.Text);

            for (int i = 0; i < ViewModel.ObsMerch.Count; ++i)
            {
                if(ViewModel.ObsMerch[i].ProductName == selectedProductName)
                {
                    ViewModel.ObsMerch[i].AddStock(quantity);
                }
            }

            FileManager.SaveToFile(ViewModel.ObsMerch);
        }
        
        private void TextBox_TextChanged(object sender, TextChangedEventArgs e)
        {
        }
    }
}
