using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Net;
using System.Net.Mime;
using System.Text;
using System.Threading.Tasks;
using AspCoreTools.Middleware.Exceptions;
using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Filters;
using Microsoft.AspNetCore.Mvc.Internal;
using Microsoft.AspNetCore.Mvc.ModelBinding;
using Microsoft.AspNetCore.Mvc.ViewFeatures;
using Newtonsoft.Json;

namespace AspCoreTools.Middleware
{
    public static class ExceptionHandlerMiddleware
    {
        public static async Task CatchExceptions(HttpContext context, Func<Task> next)
        {
            try
            {
                await next();
            }
            catch (ClientException ex)
            {
                await WriteError(
                    ex.StatusCode,
                    ex.Payload ?? new
                    {
                        Error = (ex.Message + " " + ex.InnerException?.Message).Trim()
                    },
                    context);
            }
//            catch (Exception ex)
//            {
//                await WriteError(
//                    (int)HttpStatusCode.InternalServerError,
//                    new
//                    {
//                        Error = (ex.Message + " " + ex.InnerException?.Message).Trim(),
//                        StackTrace = ex.StackTrace //TODO: Check hosting env and remvoe if prod
//                    },
//                    context);
//            }
        }

        private static async Task WriteError(int statusCode, object payload, HttpContext context)
        {
            var headers = context.Response.Headers.Select(x => x).ToList();
            context.Response.Clear();
            headers.ForEach(h => context.Response.Headers.Add(h));
            context.Response.StatusCode = statusCode;
            context.Response.ContentType = "application/json";
            var response =
                JsonConvert.SerializeObject(payload);
            await context.Response.WriteAsync(response);
        }

        public static IApplicationBuilder UseExceptionHandler(this IApplicationBuilder builder)
        {
            builder.Use(CatchExceptions);
            return builder;
        }
    }
}