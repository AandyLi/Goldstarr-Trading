using System;
using System.Collections.Generic;
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
using GoldstarrTrading.Classes;
using System.Collections.ObjectModel;


// The Blank Page item template is documented at https://go.microsoft.com/fwlink/?LinkId=402352&clcid=0x409

namespace GoldstarrTrading
{
   
    /// <summary>
    /// An empty page that can be used on its own or navigated to within a Frame.
    /// </summary>
    public sealed partial class MainPage : Page
    {
        private ViewModel viewModel;

        public MainPage()
        {
            this.InitializeComponent();
            viewModel = new ViewModel();

            CreateMerchandisesList();
        }

        private void CreateMerchandisesList()
        {
            viewModel.ObsMerch = new ObservableCollection<MerchandiseModel>()
            {
                new MerchandiseModel {ProductName = "Airscoop", Supplier = "Acne AB", Amount = 0 },
                new MerchandiseModel {ProductName = "Hyper-transceiver", Supplier = "Corelian Inc", Amount = 0 },
                new MerchandiseModel {ProductName = "Nanosporoid", Supplier = "Corelian Inc", Amount = 8},
                new MerchandiseModel {ProductName = "Boarding-spike", Supplier = "Joruba Consortium", Amount = 10}
            };
            viewModel.ObsMerch = viewModel.ObsMerch;
            //viewModel.UpdateList();
        }

        

        private void Cusomers_Click(object sender, RoutedEventArgs eventArgs)
        {
            
        }

        private void Customers_Click(NavigationView sender, NavigationViewSelectionChangedEventArgs args)
        {
            Debug.WriteLine(args.SelectedItemContainer.Name);

            switch (args.SelectedItemContainer.Name)
            {
                case "Customers":
                    this.ContentFrame.Navigate(typeof(Customers));
                    break;
                case "Merchandise":
                    this.ContentFrame.Navigate(typeof(Merchandise), viewModel);
                    break;
                case "Stock":
                    this.ContentFrame.Navigate(typeof(Stock));
                    break;
                case "Test":
                    this.ContentFrame.Navigate(typeof(Test));
                    break;
                case "Orders":
                    this.ContentFrame.Navigate(typeof(CreateOrderView), viewModel);
                    break;
                case "Deliveries":
                    this.ContentFrame.Navigate(typeof(Deliveries), viewModel);
                    break;
            }
            
        }
    }
}
