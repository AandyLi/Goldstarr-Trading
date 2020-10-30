﻿using GoldstarrTrading.Classes;
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
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
        public CreateOrderView()
        {
            this.InitializeComponent();

        }

        protected override void OnNavigatedTo(NavigationEventArgs e)
        {
            vm = (ViewModel)e.Parameter;

        }

        private void Combo_SelectionChanged(object sender, SelectionChangedEventArgs e)
        {
            var container = sender as ComboBox;
            var selectedItem = container.SelectedItem as MerchandiseModel;

            AmountDropDown.Items.Clear();

            for (int i = 0; i < selectedItem.Amount; i++)
            {
                AmountDropDown.Items.Add(i + 1);
            }
        }
    }
}