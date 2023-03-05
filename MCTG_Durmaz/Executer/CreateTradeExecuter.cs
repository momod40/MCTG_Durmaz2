using MCTG_Durmaz.Card;
using MCTG_Durmaz.DB;
using MCTG_Durmaz.HTTP;
using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace MCTG_Durmaz.Executer
{
    public class CreateTradeExecuter : IRouteCommand
    {
        Datenbank db = new Datenbank();
        RequestContext requestcon = new RequestContext();

        public CreateTradeExecuter(RequestContext request)
        {
            requestcon = request;
        }

        public Response Execute()
        {
            Response response = new Response();
            try
            {
                string[] name = requestcon.token.Split('-');
                TradingShop item = JsonConvert.DeserializeObject<TradingShop>(requestcon.Payload);
                db.createNewTrade(item, name[0]);
                response.Payload = $"Item: {item.Id} succesfully created";
                response.StatusCode = StatusCode.Ok;
            }
            catch (Exception ex)
            {
                response.Payload = "failed";
                response.StatusCode = StatusCode.BadRequest;
            }
            return response;

        }
    }
}
