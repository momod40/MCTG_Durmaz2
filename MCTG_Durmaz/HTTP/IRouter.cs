using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace MCTG_Durmaz.HTTP
{
    public interface IRouter
    {
        IRouteCommand? Resolve(RequestContext request);
    }
}
