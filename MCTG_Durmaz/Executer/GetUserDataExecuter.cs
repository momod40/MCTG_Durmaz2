using MCTG_Durmaz.DB;
using MCTG_Durmaz.HTTP;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace MCTG_Durmaz.Executer
{
    class GetUserDataExecuter : IRouteCommand 
    {
        RequestContext requestcon = new RequestContext();
        string pfad = "";
        Datenbank db = new Datenbank();
        public GetUserDataExecuter(RequestContext request, string path)
        {
            requestcon = request;
            pfad = path;
        }
        public Response Execute()
        {
            string[] user = pfad.Split('/');
            string[] tokenUser = requestcon.token.Split('-');
            if(user[2] == tokenUser[0])
            {
                //return new Response()
                //{
                //    Payload = $"Pfad: {pfad} , {user[2]}",
                //    StatusCode = StatusCode.Ok
                //};
                User.User player = db.getUserData(tokenUser[0]);
                return new Response()
                {
                    Payload = $"Username: {player.Username} , Password: {player.Password}, token = {player.Token}, Coins: {player.Coins}, Bio: {player.Bio}, Image: {player.Image}",
                    StatusCode = StatusCode.Ok
                };
            }
            else { 
            return new Response()
            {
                Payload = $"You have no permissions",
                StatusCode = StatusCode.Ok
            };
            }
        }
    }
}
