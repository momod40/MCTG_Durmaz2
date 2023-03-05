using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace MCTG_Durmaz.HTTP
{
    public enum HttpMethod
    {
        Get,
        Post,
        Put,
        Delete,
        Patch
    }

    public static class MethodUtilities
    {
        public static HttpMethod GetMethod(string method)
        {
            return method.ToLower() switch
            {
                "get" => HttpMethod.Get,
                "post" => HttpMethod.Post,
                "delete" => HttpMethod.Delete,
                "put" => HttpMethod.Put,
                "patch" => HttpMethod.Patch,
                _ => throw new InvalidDataException()
            };
        }
    }
}
