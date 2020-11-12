using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace GoldstarrTrading.Classes
{
    class CustomerModel : AssociateModel, IAddToListView
    {
       public string Email { get; set; }
        public string GetInfo()
        {
            throw new NotImplementedException();
        }
    }
}
