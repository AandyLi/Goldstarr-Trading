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
using System.Threading;
using System.Threading.Tasks;


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
            CreateCustomers();

            FileManager.LoadAllDataFromFile(viewModel);

            // Delay the collection changed events so that they do not trigger when adding stuff from the file to the observable collections
            Task.Delay(3000).ContinueWith(t => viewModel.AllowCollectionChangedEvents());

        }

        private void CreateMerchandisesList()
        {
            viewModel.ObsMerch = new ObservableCollection<MerchandiseModel>()
            {
                new MerchandiseModel {ProductName = "Airscoop", Supplier = "Acne AB", Amount = 0 },
                new MerchandiseModel {ProductName = "Hyper-transceiver", Supplier = "Corelian Inc", Amount = 0 },
                new MerchandiseModel {ProductName = "Nanosporoid", Supplier = "Corelian Inc", Amount = 8},
                new MerchandiseModel {ProductName = "Boarding-spike", Supplier = "Joruba Consortium", Amount = 1000}
            };
            viewModel.ObsMerch = viewModel.ObsMerch;
            //viewModel.UpdateList();
        }
        private void CreateCustomers()
        {
            viewModel.CustomerList = new ObservableCollection<CustomerModel>()
            {
            new CustomerModel { Name = "Damien Satansson", Address = "Helsikesgatan 666", Phone = "0705-666 666" },
            new CustomerModel { Name = "Svenne Svensson", Address = "Perssons Väg 13", Phone = "0704-111 222" },
            new CustomerModel { Name = "Lotta Bråkmakarsson", Address = "Bråkmakargatan 8", Phone = "0706-987 456" },
            new CustomerModel { Name = "Abdi Abdi", Address = "Guddomliga Gatan 42", Phone = "0707-777 777" },
            new CustomerModel { Name = "Snurre Sprätt", Address = "Morotsgatan 1", Phone = "0702-123 456" }
            };
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
                    this.ContentFrame.Navigate(typeof(Customers), viewModel);
                    break;
                case "Merchandise":
                    this.ContentFrame.Navigate(typeof(Merchandise), viewModel);
                    break;
                case "Stock":
                    this.ContentFrame.Navigate(typeof(Stock), viewModel);
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
