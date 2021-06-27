using System;
using System.Collections.Generic;
using System.Text;
using Microsoft.AspNetCore.Http;
using Newtonsoft.Json;
using AspCoreTools.Middleware.Exceptions;

namespace AspCoreTools
{
    public static class HttpContextExtensions
    {
        public static bool SendBadRequest(this HttpContext context, String message, int statusCode = 400)
        {
            throw new ClientException(message, statusCode);
        }

        public static T SendBadRequest<T>(this HttpContext context, String message, int statusCode = 400)
        {
            SendBadRequest(context, message, statusCode);
            return default(T);
        }

        public static bool SendOk(this HttpContext context)
        {
            context.Response.StatusCode = 200;
            return true;
        }

        public static T SendUnauthorized<T>(this HttpContext context, String message)
        {
            return SendBadRequest<T>(context, message, 401);
        }

        public static bool SendUnauthorized(this HttpContext context, String message)
        {
            return SendBadRequest(context, message, 401);
        }

        public static T SendForbidden<T>(this HttpContext context, String message)
        {
            return SendBadRequest<T>(context, message, 403);
        }

        public static bool SendForbidden(this HttpContext context, String message)
        {
            return SendBadRequest(context, message, 403);
        }
    }
}
