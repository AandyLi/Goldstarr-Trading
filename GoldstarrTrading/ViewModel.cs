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

        public ObservableCollection<MerchandiseModel> ObsMerch
        {
            get { return _obsMerch; }
            set
            {
                _obsMerch = value;
                RaisePropertyChanged("ObsMerch");
            }
        }

        private OrderModel _order;

        public OrderModel Order
        {
            get { return _order; }

            set
            {
                _order = value;
                RaisePropertyChanged("Order");
            }
        }
        public ViewModel()
        {
            ObsMerch = new ObservableCollection<MerchandiseModel>();
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
