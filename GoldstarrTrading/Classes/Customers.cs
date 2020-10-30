using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace GoldstarrTrading.Classes
{
    class Customers : IAddToListView
    {
        public string Name { get; set; }
        public string Address { get; set; }
        public string Phone { get; set; }
        public Customers()
        {
            List<Customers> CustomerList = new List<Customers>()
            {
            new Customers { Name = "Damien Satansson", Address = "Helsikesgatan 666", Phone = "0705-666 666" },
            new Customers { Name = "Svenne Svensson", Address = "Perssons Väg 13", Phone = "0704-111 222" },
            new Customers { Name = "Lotta Bråkmakarsson", Address = "Bråkmakargatan 8", Phone = "0706-987 456" },
            new Customers { Name = "Abdi Abdi", Address = "Guddomliga Gatan 42", Phone = "0707-777 777" },
            new Customers { Name = "Snurre Sprätt", Address = "Morotsgatan 1", Phone = "0702-123 456" }
            };
        }

        public string GetInfo()
        {
            throw new NotImplementedException();
        }
    }
}
