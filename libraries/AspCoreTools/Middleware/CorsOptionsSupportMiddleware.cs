using System;
using System.Collections.Generic;
using System.Net;
using System.Text;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Http;

namespace AspCoreTools.Middleware
{
    public static class CorsOptionsSupportMiddleware
    {
        private static readonly string[] AllowMethods = new[]
        {
            HttpMethods.Get, HttpMethods.Post, HttpMethods.Put, HttpMethods.Delete, HttpMethods.Connect,
            HttpMethods.Options
        };

        public static async Task RespondToOptions(HttpContext context, Func<Task> next)
        {

            context.Response.Headers["Access-Control-Allow-Origin"] = "*";
            context.Response.Headers["Access-Control-Expose-Headers"] = "Authorization";
            if (HttpMethods.IsOptions(context.Request.Method))
            {
                context.Response.Headers["Access-Control-Allow-Methods"] = context.Request.Headers["Access-Control-Request-Methods"];
                context.Response.Headers["Access-Control-Allow-Headers"] = context.Request.Headers["Access-Control-Request-Headers"];
                context.Response.StatusCode = (int)HttpStatusCode.NoContent;
                return;
            }


            await next();

        }


        /// <summary>
        /// MUST BE CALLED BEFORE app.UseMVC() !!!!
        /// 
        /// Adds crude HTTP OPTIONS method support. Modern browsers will send a preflight request (see <see cref="https://developer.mozilla.org/en-US/docs/Web/HTTP/CORS"/>) 
        /// before certain CORS requests, so we need to respond with the allowed methods (basically, anything), so the browser will continue with the actual request.
        /// This middleware will respond to any OPTIONS request with a list of all the methods we generally support (regardless of the route requested).
        /// 
        /// </summary>
        /// <param name="builder"></param>
        public static IApplicationBuilder UseCorsOptionsAutoResponder(this IApplicationBuilder builder)
        {
            return builder.Use(RespondToOptions);
        }
    }
}
