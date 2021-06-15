using System;
using System.Net;

namespace Pannotation.Common.Exceptions
{
    public class CustomException : Exception
    {
        public HttpStatusCode Code { get; }

        public string Key { get; set; }

        public CustomException(HttpStatusCode code, string key, string message) : base(message)
        {
            Code = code;
            Key = key;
        }
    }
}
