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

// The Blank Page item template is documented at https://go.microsoft.com/fwlink/?LinkId=234238

namespace GoldstarrTrading
{
    /// <summary>
    /// An empty page that can be used on its own or navigated to within a Frame.
    /// </summary>
    public sealed partial class Stock : Page
    {
        List <MyClass> myList = new List<MyClass>();
        public Stock()
        {
            this.InitializeComponent();
            
            for (int i = 0; i < 40; i++)
            {
                MyClass a = new MyClass();
                a.ProductName = "Product name" + i;
                a.AmountInStock = 0;
                myList.Add(a);
            }

            StockList.ItemsSource = myList;
        }

        private void TextBox_TextChanged(object sender, TextChangedEventArgs e)
        {

        }


    }

    class MyClass
    {
        public string ProductName { get; set; }
        public int AmountInStock { get; set; }
    }


}
