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

        public ObservableCollection<MerchandiseModel> ObsMerch
        {
            get { return _obsMerch; }
            set
            {
                _obsMerch = value;
                RaisePropertyChanged("ObsMerch");
            }
        }

        private ObservableCollection<OrderModel> _order;

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
        public ViewModel()
        {
            ObsMerch = new ObservableCollection<MerchandiseModel>();
            CustomerList = new ObservableCollection<CustomerModel>();
            Order = new ObservableCollection<OrderModel>();
            PendingOrder = new ObservableCollection<OrderModel>();
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

        
       

        //public List<MerchandiseModel> MerchList { get; set; } // Skip for now
        //public void UpdateList()
        //{
        //    foreach (var item in MerchList)
        //    {
        //        ObsMerch.Add(item);
        //    }
        //}


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
