using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace MCTG_Durmaz.Card
{
    public class TradingShop
    {
        public string Id { get; set; }
        public string CardToTrade { get; set; }
        public string Type { get; set; }
        public int MinimumDamage { get; set; }
        public string Owner { get; set; }
    }
}
