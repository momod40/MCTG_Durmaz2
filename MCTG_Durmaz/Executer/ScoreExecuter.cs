using MCTG_Durmaz.DB;
using MCTG_Durmaz.HTTP;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace MCTG_Durmaz.Executer
{
    public class ScoreExecuter : IRouteCommand
    {
        User.User user = new User.User();
        RequestContext requestcon;
        Datenbank db = new Datenbank();
        public ScoreExecuter(RequestContext request)
        {
            requestcon = request;
        }

        public Response Execute()
        {
            Response response = new Response();
            try
            {
                string[] name = requestcon.token.Split('-');
                //User.User player = db.getStatsByUser(name[0]);
                //response.Payload = $"User: {name[0]}, Wins: {player.Win}, Lose: {player.Lose}, Elo: {player.Elo}";
                List<User.User> playerList = db.getScoreBoard();
                foreach(var x in playerList)
                {
                    response.Payload += $"Username: {x.Username}, Wins: {x.Win}, Lose: {x.Lose}, Elo: {x.Elo}";
                }
                response.StatusCode = StatusCode.Ok;
                return response;
            }
            catch (Exception ex)
            {
                return new Response()
                {

                    Payload = $"You have no permissions {ex}",
                    StatusCode = StatusCode.Ok
                };
            }
        }

    }
}
