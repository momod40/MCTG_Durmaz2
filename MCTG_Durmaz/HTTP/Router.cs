using MCTG_Durmaz.Executer;
using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace MCTG_Durmaz.HTTP
{
    public class Router : IRouter
    {
        //private readonly IUserManager _userManager;
        //private readonly IMessageManager _messageManager;
        //private readonly IdentityProvider _identityProvider;
        //private readonly IRouteParser _routeParser = new IdRouteParser();

        //public Router(IUserManager userManager, IMessageManager messageManager)
        //{
        //    _userManager = userManager;
        //    _messageManager = messageManager;

        //    // better: define IIdentityProvider interface and get concrete implementation passed in as dependency
        //    _identityProvider = new IdentityProvider(userManager);
        //}

        public IRouteCommand? Resolve(RequestContext request)
        {
            //var identity = (RequestContext request) => _identityProvider.GetIdentityForRequest(request) ?? throw new RouteNotAuthenticatedException();
            //var isMatch = (string path) => _routeParser.IsMatch(path, "/messages/{id}");
            //var parseId = (string path) => int.Parse(_routeParser.ParseParameters(path, "/messages/{id}")["id"]);


            IRouteCommand? command = request switch
            {
                { Method: HttpMethod.Post, ResourcePath: "/users" } => new RegisterExecuter(request),
                { Method: HttpMethod.Post, ResourcePath: "/sessions" } => new LoginExecuter(request),
                { Method: HttpMethod.Post, ResourcePath: "/packages" } => new PackageCreater(request),
                { Method: HttpMethod.Post, ResourcePath: "/transactions/packages" } => new AcquirePackage(request),
                { Method: HttpMethod.Get, ResourcePath: "/cards" } => new GetCardsExecuter(request),
                { Method: HttpMethod.Put, ResourcePath: "/deck" } => new ChangeDeckExecuter(request),
                { Method: HttpMethod.Get, ResourcePath: "/deck" } => new GetDeckExecuter(request),
                { Method: HttpMethod.Get, ResourcePath: "/stats" } => new StatsExecuter(request),
                { Method: HttpMethod.Get, ResourcePath: "/score" } => new ScoreExecuter(request),
                { Method: HttpMethod.Get, ResourcePath: "/tradings" } => new GetTradingShop(request),
                { Method: HttpMethod.Post, ResourcePath: "/tradings" } => new CreateTradeExecuter(request),
                { Method: HttpMethod.Get, ResourcePath: "/weeklygift" } => new WeeklyGiftExecuter(request),
                { Method: HttpMethod.Get, ResourcePath: var path } => path.StartsWith("/users") ? new GetUserDataExecuter(request, path) : null,
                { Method: HttpMethod.Put, ResourcePath: var path } => path.StartsWith("/users") ? new EditUserDataExecuter(request, path) : null,
                { Method: HttpMethod.Delete, ResourcePath: var path } => path.StartsWith("/tradings") ? new DeleteTradeExecuter(request, path) : null,
                { Method: HttpMethod.Post, ResourcePath: var path } => path.StartsWith("/tradings") ? new TradeExecuter(request, path) : null,

                //{ Method: HttpMethod.Post, ResourcePath: "/messages" } => new AddMessageCommand(_messageManager, identity(request), EnsureBody(request.Payload)),
                //{ Method: HttpMethod.Get, ResourcePath: "/messages" } => new ListMessagesCommand(_messageManager, identity(request)),

                //{ Method: HttpMethod.Get, ResourcePath: var path } when isMatch(path) => new ShowMessageCommand(_messageManager, identity(request), parseId(path)),
                //{ Method: HttpMethod.Put, ResourcePath: var path } when isMatch(path) => new UpdateMessageCommand(_messageManager, identity(request), parseId(path), EnsureBody(request.Payload)),
                //{ Method: HttpMethod.Delete, ResourcePath: var path } when isMatch(path) => new RemoveMessageCommand(_messageManager, identity(request), parseId(path)),

                _ => null
            };

            return command;
        }

        private string EnsureBody(string? body)
        {
            if (body == null)
            {
                throw new InvalidDataException();
            }
            return body;
        }

        private T Deserialize<T>(string? body) where T : class
        {
            var data = body != null ? JsonConvert.DeserializeObject<T>(body) : null;
            if (data == null)
            {
                throw new InvalidDataException();
            }
            return data;
        }
    }
}
