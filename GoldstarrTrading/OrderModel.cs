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
        public DateTime OrderDate { get; set; } = DateTime.Now;
        public string CustomerName { get; set; }
        public MerchandiseModel Merch { get; set; }
        public int OrderedAmount { get; set; }
        public bool IsPendingOrder { get; set; }

        public void CreateOrder(string customerName, MerchandiseModel merchandise, int orderedAmount, bool isPending = false)
        {
            this.CustomerName = customerName;
            this.Merch = merchandise;
            this.OrderedAmount = orderedAmount;
            this.IsPendingOrder = isPending;
        }
    }
}
