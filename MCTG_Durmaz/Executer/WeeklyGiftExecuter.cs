using MCTG_Durmaz.DB;
using MCTG_Durmaz.HTTP;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace MCTG_Durmaz.Executer
{
    public class WeeklyGiftExecuter : IRouteCommand
    {
        User.User user = new User.User();
        RequestContext requestcon;
        Datenbank db = new Datenbank();

        public WeeklyGiftExecuter(RequestContext request)
        {
            requestcon = request;
        }
        
        public Response Execute()
        {
            Response response = new Response();
            string[] name = requestcon.token.Split('-');
            if (!db.hasUsedWeeklyGift(name[0]))
            {
                Random rnd = new Random();
                int randomNumber = rnd.Next(-10, 11);
                db.updateWeeklyGift(randomNumber, name[0]);
                response.Payload = $"Du hast {randomNumber} Siege dazu bekommen";
                response.StatusCode = StatusCode.Ok;
                return response;
            }
            else { 
            return new Response() { Payload = "Schau später nochmal vorbei", StatusCode = StatusCode.Ok };
            }
        }
    }
}
