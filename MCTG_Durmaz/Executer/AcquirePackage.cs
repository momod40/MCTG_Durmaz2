using MCTG_Durmaz.DB;
using MCTG_Durmaz.HTTP;
using Newtonsoft.Json;
using Npgsql;
using System;
using MCTG_Durmaz.User;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace MCTG_Durmaz.Executer
{
    public class AcquirePackage : IRouteCommand
    {
        User.User user = new User.User();
        RequestContext requestcon;
        Datenbank db = new Datenbank();
        public AcquirePackage(RequestContext request)
        {
            requestcon = request;
        }
        public Response Execute()
        {
            Response response = new Response();
            //check if he has enough coins
            string[] name = requestcon.token.Split('-');
            if (db.userCoins(name[0]) >= 5)
            {
                //buy package
                //create package first
                //get 5 cards
                // acquire to user
                var freecards = db.getFreeCards();
                if (freecards.Count != 0)
                {
                    foreach (var x in freecards)
                    {
                        db.setUserPackage(x, name[0]);
                    }
                    db.setCoins(name[0], -5);
                    db.deleteDeckByUser(name[0]);
                    var bestCards = db.getBest4Cards(name[0]);
                    foreach (var bCard in bestCards)
                    {
                        db.InsertIntoDeck(bCard.Id, name[0]);
                    }

                    response.Payload = $"Package created for user {requestcon.token}";
                    response.StatusCode = StatusCode.Ok;


                }
                else
                {
                    response.Payload = "No package left";
                    response.StatusCode = StatusCode.BadRequest;
                }
            }
            else
            {
                response.Payload = "Not enough coins";
                response.StatusCode = StatusCode.BadRequest;
            }
            return response;
        }
    }
}
