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
    public class LoginExecuter : IRouteCommand
    {
        User.User user = new User.User();
        RequestContext requestcon;
        Datenbank db = new Datenbank();
        public LoginExecuter(RequestContext request)
        {
            requestcon = request;
        }
        public Response Execute()
        {
            var response = new Response();
            var User = JsonConvert.DeserializeObject<User.User>(requestcon.Payload);
            if (db.logging(User))
            {
                response.Payload = "User successfully logged in";
                response.StatusCode = StatusCode.Ok;
            }
            else
            {
                response.Payload = "User couldnt be logged in";
                response.StatusCode = StatusCode.Accepted;
            }
            return response;
        }
    }
}