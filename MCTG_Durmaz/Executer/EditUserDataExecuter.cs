using MCTG_Durmaz.DB;
using MCTG_Durmaz.HTTP;
using Newtonsoft.Json;
using Npgsql;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace MCTG_Durmaz.Executer
{
    public class EditUserDataExecuter : IRouteCommand
    {
        User.User user = new User.User();
        RequestContext requestcon;
        Datenbank db = new Datenbank();
        string pfad = "";

        public EditUserDataExecuter(RequestContext request, string path)
        {
            requestcon = request;
            pfad = path;
        }
        public Response Execute()
        {
            try
            {
                string[] user = pfad.Split('/');
                string[] tokenUser = requestcon.token.Split('-');
                if (user[2] == tokenUser[0])
                {
                    //return new Response()
                    //{
                    //    Payload = $"Pfad: {pfad} , {user[2]}",
                    //    StatusCode = StatusCode.Ok
                    //};
                    User.User player = JsonConvert.DeserializeObject<User.User>(requestcon.Payload);
                    var x = db.changeUserData(player, tokenUser[0]);

                    return new Response()
                    {
                        Payload = $"Successfully changed",
                        StatusCode = StatusCode.Ok
                    };
                }
                return new Response()
                {
                    Payload = $"You have no permissions",
                    StatusCode = StatusCode.BadRequest
                };
            }
            catch (PostgresException)
            {
                return new Response()
                {
                    Payload = $"failed",
                    StatusCode = StatusCode.BadRequest
                };
            }
        }
    }
}
