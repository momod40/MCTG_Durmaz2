using MCTG_Durmaz.DB;
using MCTG_Durmaz.HTTP;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace MCTG_Durmaz.Executer
{
    public class TradeExecuter : IRouteCommand
    {
        Datenbank db = new Datenbank();
        RequestContext requestcon = new RequestContext();
        string pfad = "";

        public TradeExecuter(RequestContext request, string path)
        {
            requestcon = request;
            pfad = path;
        }

        public Response Execute()
        {
            try
            {
                string[] cardId = pfad.Split('/');
                string[] tokenUser = requestcon.token.Split('-');
                //if (db.isCardMine(cardId[3], tokenUser[0]))
                //{
                //    db.deleteCard(cardId[3]);
                //    return new Response()
                //    {
                //        Payload = "Trade deleted",
                //        StatusCode = StatusCode.Ok
                //    };
                //}

                if (!db.isCardMine(cardId[2], tokenUser[0]))
                {
                    db.Trade(cardId[2], tokenUser[0]);
                    db.deleteCard(cardId[3]);
                    return new Response()
                    {
                        Payload = "Trade successfully done",
                        StatusCode = StatusCode.Ok
                    };
                }
                else
                {
                    return new Response()
                    {
                        Payload = "You cant trade with your own",
                        StatusCode = StatusCode.Ok
                    };
                }
            }
            catch (Exception ex)
            {
                return new Response()
                {
                    Payload = "failed",
                    StatusCode = StatusCode.Ok
                };
            }
        }
    }
}
