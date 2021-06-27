using System;
using System.IdentityModel.Tokens.Jwt;
using System.Linq;
using System.Net;
using System.Security.Claims;
using System.Text.RegularExpressions;
using Microsoft.AspNetCore.Http;
using Microsoft.IdentityModel.Tokens;

namespace AspCoreTools.Jwt
{
    /// <summary>
    /// Utility class for doing things with JWT
    /// </summary>
    public static class JwtTools
    {
        public static SigningCredentials Credentials;
        public static TimeSpan TokenLifeSpan = TimeSpan.FromDays(30);

        private static readonly Regex BearerParser = new Regex(@"(?<=Bearer\s).+", RegexOptions.Compiled);
        private const string Issuer = "KrewFindrIss";
        private const string Audience = "KrewFindrAud";
        private const string AuthenticationType = "DaKrewAuth";

        /// <summary>
        /// Creates a ClaimsPrincipal from the headers in the request context.
        /// </summary>
        /// <param name="context"></param>
        /// <returns></returns>
        internal static ClaimsPrincipal DecodeUser(HttpContext context)
        {
            if (Credentials == null)
            {
                throw new NullReferenceException($"{nameof(JwtTools)}.{nameof(Credentials)} was not initialized before a call to {nameof(DecodeUser)}.");
            }

            var headers = context.Request.Headers[HttpRequestHeader.Authorization.ToString()];
            if (headers.ToArray().Length == 0)
            {
                return null;
            }

            var jwtMatch = BearerParser.Match(headers[0] ?? "");

            if (!jwtMatch.Success)
            {
                return null;
            }

            var jwt = jwtMatch.Value;

            var handler = new JwtSecurityTokenHandler();
            SecurityToken token;

            var principal = handler.ValidateToken(jwt, new TokenValidationParameters()
            {
                ValidIssuer = Issuer,
                ValidAudience = Audience,
                IssuerSigningKey = Credentials.Key
            }, out token);

            return principal.HasClaim(c => c.Type == RoleType.VoidToken) ? null : principal;
        }

        /// <summary>
        /// Adds a token header with the given claims to the response for the given context.
        /// 
        /// This will create a new token that will be sent along with whatever response gets sent. 
        /// This method can be safely called multiple times. Header is identical to the one that might be present in a request
        /// (Authorize: Bearer [token] ).
        /// </summary>
        /// <param name="context">The HttpContext to add the header to.</param>
        /// <param name="globalRoles">These are roles that this user has applying to all aspects of the site. can be queried with User.IsInRole(globalRole)</param>
        /// <param name="userId">User id to include in the token</param>
        /// <param name="entityRoles">These are roles that apply to only a specific entity (e.x. course or group). The claim type is the role (e.x. Instructor) and the value is the
        /// id of the entity. These can be queried with User.IsInRoleForEntity.
        /// The same role type can be passed with many entityIds.</param>
        public static void AddLoginHeader(HttpContext context, string userId, Claim[] entityRoles, params string[] globalRoles)
        {
            if (Credentials == null)
            {
                throw new NullReferenceException($"{nameof(JwtTools)}.{nameof(Credentials)} was not initialized before a call to {nameof(AddLoginHeader)}.");
            }

            var userIdentity = new ClaimsIdentity(AuthenticationType);
            userIdentity.AddClaim(new Claim(RoleType.UserIdClaimType, userId));
            userIdentity.AddClaims(entityRoles ?? new Claim[0]);
            userIdentity.AddClaims(globalRoles?.Select(role=>new Claim(userIdentity.RoleClaimType, role)) ?? new Claim[0]);

            var handler = new JwtSecurityTokenHandler();

            var token = handler.CreateEncodedJwt(Issuer, Audience, userIdentity, null, DateTime.Now + TokenLifeSpan,
                DateTime.Now, Credentials);


            context.Response.Headers[HttpRequestHeader.Authorization.ToString()] = "Bearer " + token;
        }
    }
}