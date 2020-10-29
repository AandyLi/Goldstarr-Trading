using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace GoldstarrTrading
{
    class OrderModel
    {
        private DateTime OrderDate { get; set; }
        private string CustomerName { get; set; }
        private Merchandise Merch { get; set; }
        private int OrderedAmount { get; set; }

        private void AddMerchandiseToOrder(Merchandise merchandise)
        {

        }

    }
}
