using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using GoldstarrTrading.Classes;

namespace GoldstarrTrading.Classes
{
    public class MerchandiseModel : IAddToListView
    {
        public string ProductName { get; set; }
        public string Supplier { get; set; }
        public int Amount { get; set; } = 0;

        public int ViewStockAmount()
        {
            return Amount;
        }

        public void RemoveStock(int amount)
        {

        }

        public void AddStock(int amount)
        {

        }
        public string GetInfo()
        {
            throw new NotImplementedException();
        }
    }

    //public static class Merchandises // Skapade en klass som innehåller en lista av Merchandise Klassen.
    //{
    //    public static List<Merchandise> MerchandisesList;

    //    /// <summary>
    //    /// Metod som returnerar listan.
    //    /// </summary>
    //    /// <returns>List<Merchandise></returns>
    //    /// 
    //    public static List<Merchandise> GetMerchandises()
    //    {
    //        return MerchandisesList;
    //    }
    //}

    

    
}
