using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace MCTG_Durmaz.Cards
{
    public enum Type { Monster, Spell };
    public class Cards
    {
        public string Id { get; set; }
        public string Name { get; set; }
        public double Damage { get; set; }
        public string Element { get; set; }
        public string Art { get; set; }
    }
}
