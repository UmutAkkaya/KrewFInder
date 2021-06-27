using System;
using System.Collections.Generic;
using System.Net;
using System.Security.Claims;
using System.Security.Principal;
using System.Text;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.IdentityModel.Tokens;

namespace AspCoreTools.Jwt
{
    public static class JwtAuthenticatorMiddleware
    {
        public static async Task DoAuth(HttpContext context, Func<Task> next)
        {
            try
            {
                var user = JwtTools.DecodeUser(context);
                context.User = user ?? new ClaimsPrincipal(); //Try to keep this up to date as best we can

                await next();
            }
            catch (SecurityTokenException ex)
            {
                Console.WriteLine(ex.Message);
                context.Response.StatusCode = (int)HttpStatusCode.Unauthorized;
            }
        }

        /// <summary>
        /// MUST BE CALLED BEFORE app.UseMVC() !!!!
        /// 
        /// Adds Json Web Token authentication to the app. Use the [JwtAuth] attribute to mark actions or controllers that need authentication.
        /// Then check User claims with HttpContext.DoesUserHaveRoleForEntity or HttpContext.User.Claims
        /// [Authorize] attribute will not work with this because f*** that s***.
        /// 
        /// </summary>
        /// <param name="builder"></param>
        /// <param name="creds"></param>
        /// <returns></returns>
        public static IApplicationBuilder UseJwtAuth(this IApplicationBuilder builder, SigningCredentials creds)
        {
            JwtTools.Credentials = creds;
            return builder.Use(DoAuth);
        }
    }
}
