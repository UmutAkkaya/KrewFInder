using System;
using System.Collections.Generic;
using System.Text;

namespace AspCoreTools.Middleware.Exceptions
{
    public class ClientException : Exception
    {
        public int StatusCode { get; private set; }
        public object Payload { get; private set; }
        public ClientException(int statusCode, object payload = null)
        {
            StatusCode = statusCode;
            Payload = payload;
        }

        public ClientException(string message, int statusCode, object payload = null) : base(message)
        {
            StatusCode = statusCode;
            Payload = payload;
        }

        public ClientException(string message, Exception innerException, int statusCode, object payload = null) : base(message, innerException)
        {
            StatusCode = statusCode;
            Payload = payload;
        }
    }
}
