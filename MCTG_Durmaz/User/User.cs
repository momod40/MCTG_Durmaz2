using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace MCTG_Durmaz.User
{
    public class User
    {
        public string Username { get;  set; }
        public string Name { get; set; }

        public string Password { get; set; }
        public string Token => $"{Username}-msgToken";
        public int Coins { get; set; }

        public string Bio { get; set; }
        public string Image { get; set; }
        public int Win { get; set; }
        public int Lose { get; set; }
        public int Elo { get; set; }

    }
}
