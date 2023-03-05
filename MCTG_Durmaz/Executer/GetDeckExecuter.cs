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
    public class GetDeckExecuter : IRouteCommand
    {
        User.User user = new User.User();
        RequestContext requestcon;
        Datenbank db = new Datenbank();
        public GetDeckExecuter(RequestContext request)
        {
            requestcon = request;
        }
        public Response Execute()
        {
            Response response = new Response();
            try
            {
                string[] name = requestcon.token.Split('-');
                var deck = db.getDeckByUser(name[0]);
                foreach(var x in deck)
                {
                    response.Payload += $"ID: {x}";
                }
                response.StatusCode = StatusCode.Ok;
                return response;
            }
            catch (PostgresException)
            {
                response.Payload = "Falsch";
                response.StatusCode = StatusCode.BadRequest;
                return response;
            }

        }
    }
}
