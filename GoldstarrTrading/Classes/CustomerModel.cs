using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace GoldstarrTrading.Classes
{
    class CustomerModel : IAddToListView
    {
        public string Name { get; set; }
        public string Address { get; set; }
        public string Phone { get; set; }
       

        public string GetInfo()
        {
            throw new NotImplementedException();
        }
    }
}
