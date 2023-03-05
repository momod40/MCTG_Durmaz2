using MCTG_Durmaz.DB;
using MCTG_Durmaz.HTTP;
using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Npgsql;

namespace MCTG_Durmaz.Executer
{
    class PackageCreater : IRouteCommand
    {
        User.User user = new User.User();
        RequestContext requestcon;
        Datenbank db = new Datenbank();

        public PackageCreater(RequestContext request)
        {
            requestcon = request;
        }
        public Response Execute()
        {
            var response = new Response();
            if (requestcon.token != "admin-mtcgToken")
            {
                response.Payload += $"Only admins can add {requestcon.token}";
                response.StatusCode = StatusCode.BadRequest;
            }
            else
            {
                var cards = JsonConvert.DeserializeObject<List<Cards.Cards>>(requestcon.Payload);
                foreach (var x in cards)
                {
                    try
                    {
                        if (x.Name.Contains("Fire")) {
                            x.Element = "Fire";
                        }
                        else if (x.Name.Contains("Water"))
                        {
                            x.Element = "Water";
                        }
                        else
                        {
                            x.Element = "Normal";
                        }
                        x.Art = x.Name.Contains("Spell") ? "Spell" : "Monster";


                        db.InsertPackage(x.Id, x.Name, x.Damage, x.Element, x.Art);




                        response.Payload = "Cards has been succesfully added";
                        response.StatusCode = StatusCode.Created;

                    }
                    catch (PostgresException)
                    {
                        response.Payload = "Cards couldnt be added";
                        response.StatusCode = StatusCode.BadRequest;
                    }
                }
            }
            return response;
        }
    }
}
