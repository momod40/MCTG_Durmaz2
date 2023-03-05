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
    public class ChangeDeckExecuter : IRouteCommand
    {
        User.User user = new User.User();
        RequestContext requestcon;
        Datenbank db = new Datenbank();

        public ChangeDeckExecuter(RequestContext request)
        {
            requestcon = request;
        }
        public Response Execute()
        {
            Response response = new Response();
            try
            {
                List<string> newCards = JsonConvert.DeserializeObject<List<string>>(requestcon.Payload);
                string[] name = requestcon.token.Split('-');
                if (newCards.Count != 4)
                {
                    response.Payload = "Es müssen mindestens 4 Karten sein";
                    response.StatusCode = StatusCode.NoContent;

                }
                else
                {
                    for (int i = 0; i < newCards.Count; i++)
                    {
                        if (!db.IsCardFromUser(newCards[i], name[0])){  
                            return new Response() { Payload = "Not your card!", StatusCode = StatusCode.BadRequest };
                        }
                    }
                    db.deleteDeckByUser(name[0]);

                    foreach (var x in newCards)
                    {
                        db.InsertIntoDeck(x, name[0]);
                    }
                    response.Payload = "Karten erfolgreich hinzugefügt";
                    response.StatusCode = StatusCode.Ok;
                }
            }
            catch (Exception ex)
            {
                response.Payload = "failed";
                response.StatusCode = StatusCode.BadRequest;
                Console.WriteLine("Fehler: " + ex.Message);
            }
            return response;
        }
    }
}
