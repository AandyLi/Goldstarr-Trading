using GoldstarrTrading.Classes;
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.ComponentModel;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Windows.ApplicationModel.Store.Preview.InstallControl;

namespace GoldstarrTrading
{
    class ViewModel : INotifyPropertyChanged
    {

        private ObservableCollection<MerchandiseModel> _obsMerch;
        private ObservableCollection<CustomerModel> _customerList;
        private ObservableCollection<OrderModel> _order;
        private ObservableCollection<SupplierModel> _supplier;

        public ObservableCollection<MerchandiseModel> ObsMerch
        {
            get { return _obsMerch; }
            set
            {
                _obsMerch = value;
                RaisePropertyChanged("ObsMerch");
            }
        }

        public ObservableCollection<OrderModel> Order
        {
            get { return _order; }

            set
            {
                _order = value;
                RaisePropertyChanged("Order");
            }
        }

        private ObservableCollection<OrderModel> _pendingOrder;

        public ObservableCollection<OrderModel> PendingOrder
        {
            get { return _pendingOrder; }

            set
            {
                _pendingOrder = value;
                RaisePropertyChanged("PendingOrder");
            }
        }

        public void AllowCollectionChangedEvents()
        {
            ObsMerch.CollectionChanged += ObsMerch_CollectionChanged;
            CustomerList.CollectionChanged += CustomerList_CollectionChanged;
            Order.CollectionChanged += Order_CollectionChanged;
            Supplier.CollectionChanged += Supplier_CollectionChanged;
            PendingOrder.CollectionChanged += PendingOrder_CollectionChanged;
        }
        private async void PendingOrder_CollectionChanged(object sender, System.Collections.Specialized.NotifyCollectionChangedEventArgs e)
        {
            await FileManager.SaveToFile(PendingOrder);
        }
        private async void Supplier_CollectionChanged(object sender, System.Collections.Specialized.NotifyCollectionChangedEventArgs e)
        {
            await FileManager.SaveToFile(Supplier);
        }

        private async void Order_CollectionChanged(object sender, System.Collections.Specialized.NotifyCollectionChangedEventArgs e)
        {
            await FileManager.SaveToFile(Order);
        }

        private async void CustomerList_CollectionChanged(object sender, System.Collections.Specialized.NotifyCollectionChangedEventArgs e)
        {
            await FileManager.SaveToFile(CustomerList);
        }

        private async void ObsMerch_CollectionChanged(object sender, System.Collections.Specialized.NotifyCollectionChangedEventArgs e)
        {
            await FileManager.SaveToFile(ObsMerch);
        }

        public ObservableCollection<CustomerModel> CustomerList
        {
            get { return _customerList; }
            set
            {
                _customerList = value;
                RaisePropertyChanged("CustomerList");
            }
        }

        public ObservableCollection<SupplierModel> Supplier
        {
            get { return _supplier; }
            set
            {
                _supplier = value;
                RaisePropertyChanged("Supplier");
            }
        }


        public ViewModel()
        {
            ObsMerch = new ObservableCollection<MerchandiseModel>();
            CustomerList = new ObservableCollection<CustomerModel>();
            Order = new ObservableCollection<OrderModel>();
            PendingOrder = new ObservableCollection<OrderModel>();
            Supplier = new ObservableCollection<SupplierModel>();
        }


        public event PropertyChangedEventHandler PropertyChanged;
        protected void RaisePropertyChanged(string name)
        {
            if (PropertyChanged != null)
            {
                PropertyChanged(this, new PropertyChangedEventArgs(name));
            }
        }
    }

}
