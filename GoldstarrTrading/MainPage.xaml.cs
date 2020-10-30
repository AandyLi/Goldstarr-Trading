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


// The Blank Page item template is documented at https://go.microsoft.com/fwlink/?LinkId=402352&clcid=0x409

namespace GoldstarrTrading
{
   
    /// <summary>
    /// An empty page that can be used on its own or navigated to within a Frame.
    /// </summary>
    public sealed partial class MainPage : Page
    {

        public MainPage()
        {
            this.InitializeComponent();
            CreateMerchandisesList();
        }

        private void CreateMerchandisesList()
        {
            Merchandises.MerchandisesList = new List<Classes.Merchandise>()
            {
                new Classes.Merchandise {ProductName = "Airscoop", Supplier = "Acne AB", Amount = 0 },
                new Classes.Merchandise {ProductName = "Hyper-transceiver", Supplier = "Corelian Inc", Amount = 0 },
                new Classes.Merchandise {ProductName = "Nanosporoid", Supplier = "Corelian Inc", Amount = 0},
                new Classes.Merchandise {ProductName = "Boarding-spike", Supplier = "Joruba Consortium", Amount = 0}
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
                    this.ContentFrame.Navigate(typeof(Customers));
                    break;
                case "Merchandise":
                    this.ContentFrame.Navigate(typeof(Merchandise));
                    break;
                case "Stock":
                    this.ContentFrame.Navigate(typeof(Stock));
                    break;
                case "Test":
                    this.ContentFrame.Navigate(typeof(Test));
                    break;
                case "Orders":
                    this.ContentFrame.Navigate(typeof(CreateOrderView));
                    break;
            }
            
        }
    }
}
