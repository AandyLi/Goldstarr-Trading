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
        public event PropertyChangedEventHandler PropertyChanged;

        public ObservableCollection<MerchandiseModel> ObsMerch;

        private OrderModel Order { get; set; }

        public List<MerchandiseModel> MerchList { get; set; }

        public ViewModel()
        {
            ObsMerch = new ObservableCollection<MerchandiseModel>();
        }

        public void UpdateList()
        {
            foreach (var item in MerchList)
            {
                ObsMerch.Add(item);
            }
        }
    }

}
