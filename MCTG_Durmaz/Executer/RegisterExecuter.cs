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
    public class RegisterExecuter : IRouteCommand
    {
        User.User user = new User.User();
        RequestContext requestcon;
        Datenbank db = new Datenbank();
        public RegisterExecuter(RequestContext request)
        {
            //if (request.Header.TryGetValue("Authorization", out var auth))
            //{
            //    Console.WriteLine(auth);
            //}
            requestcon = request;
        }
        public Response Execute()
        {
            user = JsonConvert.DeserializeObject<User.User>(requestcon.Payload!);
            //Console.WriteLine(user.Username);
            //Console.WriteLine(user.Password);
            //Console.WriteLine(user.Token);


            var response = new Response();
            if (db.checkAndRegister(user)) { 
                response.Payload = "User created";
                response.StatusCode = StatusCode.Created;
            }
            else
            {
                response.Payload = "User couldnt be created";
                response.StatusCode = StatusCode.BadRequest;
            }
            return response;
        }
    }
}
