using System;
using System.Collections.Generic;
using System.Text;
using AspCoreTools.Jwt;
using Microsoft.AspNetCore.Builder;
using Microsoft.Extensions.Configuration;
using Microsoft.IdentityModel.Tokens;
using Newtonsoft.Json;

namespace AspCoreTools.Middleware
{
    public static class MiddlewareBundle
    {
        //If a hacker got this far, they can certainly just as easily pull it out of the config file...
        public const string JwtKey = "MHcCAQEEIP2BKgKvWjS2A09oobfNJ8CdL0QXvaTWs1uCNl6TUPopoAoGCCqGSM49AwEHoUQDQgAEmxYZ485juVXV3fXBdai7xWSsWuo9JktfbTIgJIL0bd6aHnUoMgHwKzRhZidrcqjMdjG7F63w1NjzhCRo/HhSqQ==";

        public static IApplicationBuilder UseMiddlewareBundle(this IApplicationBuilder builder, IConfiguration config)
        {
            JsonConvert.DefaultSettings = () => new JsonSerializerSettings
            {
                PreserveReferencesHandling = PreserveReferencesHandling.All,
                ReferenceLoopHandling = ReferenceLoopHandling.Ignore
            };
            
            //var key = config.GetSection("Crypto:SecretKey").Value;
            var creds = new SigningCredentials(
                new SymmetricSecurityKey(Convert.FromBase64String(JwtKey)),
                SecurityAlgorithms.HmacSha256Signature);

            //TODO: Don't allow any origin.
            builder.UseCorsOptionsAutoResponder();
            builder.UseJwtAuth(creds);
            builder.UseExceptionHandler();
            return builder;
        }
    }
}
