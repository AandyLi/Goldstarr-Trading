using GoldstarrTrading.Classes;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace GoldstarrTrading
{
    class OrderModel
    {
        private DateTime OrderDate { get; set; } = DateTime.Now;
        private string CustomerName { get; set; }
        private MerchandiseModel Merch { get; set; }
        private int OrderedAmount { get; set; }

        public void CreateOrder(string customerName, MerchandiseModel merchandise, int orderedAmount)
        {
            this.CustomerName = customerName;
            this.Merch = merchandise;
            this.OrderedAmount = orderedAmount;
        }

    }
}
