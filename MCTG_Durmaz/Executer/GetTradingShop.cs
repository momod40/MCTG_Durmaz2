using MCTG_Durmaz.Card;
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
    public class GetTradingShop : IRouteCommand
    {
        RequestContext requestcon;
        Datenbank db = new Datenbank();

        public GetTradingShop(RequestContext request)
        {
            requestcon = request;
        }
       
        public Response Execute()
        {
            Response response = new Response();
            try
            {
                string[] name = requestcon.token.Split('-');
                List<TradingShop> trades = db.getItemShop(name[0]);
                if(trades.Count == 0)
                {
                    return new Response { Payload = "Zurzeit keine Tradingangebote", StatusCode = StatusCode.Ok };
                }
                foreach(var x in trades)
                {
                    response.Payload += $"Id: {x.Id}, CardToTrade: {x.CardToTrade}, Type: {x.Type}, MinimumDamage: {x.MinimumDamage}, current Owner: {x.Owner}";
                }
                response.StatusCode = StatusCode.Ok;
                return response;
            }
            catch (Exception ex)
            {

                response.Payload = $"failed {ex.Message}";
                response.StatusCode = StatusCode.BadRequest;
            }
            return response;
        }

    }
}
