using MCTG_Durmaz.DB;
using MCTG_Durmaz.HTTP;
using Npgsql;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace MCTG_Durmaz.Executer
{
    public class GetCardsExecuter : IRouteCommand
    {
        User.User user = new User.User();
        RequestContext requestcon;
        Datenbank db = new Datenbank();
        public GetCardsExecuter(RequestContext request)
        {
            requestcon = request;
        }

        public Response Execute()
        {
            Response response = new Response();
            try
            {
                if(requestcon.token == null)
                {
                    return new Response() { Payload = "No token given", StatusCode = StatusCode.BadRequest };
                }
                string[] name = requestcon.token.Split('-');
                var cards = db.getCardsByUser(name[0]);
                if (cards.Count != 0)
                {
                    foreach (var x in cards)
                    {
                        response.Payload += $"ID: {x.Id}, Name: {x.Name}, Damage: {x.Damage} ";

                    }
                    
                    response.StatusCode = StatusCode.Ok;
                }
                else
                {
                    response.Payload = $"No cards for user {name[0]}";
                    response.StatusCode = StatusCode.NoContent;
                }
            }
            catch (Exception ex)
            {
                response.Payload = "token is missing";
                response.StatusCode = StatusCode.BadRequest;
            }
            return response;
        }
    }
}
