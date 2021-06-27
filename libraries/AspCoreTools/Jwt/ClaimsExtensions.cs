using System;
using System.Collections.Generic;
using System.Linq;
using System.Security.Claims;
using System.Text;
using Microsoft.AspNetCore.Http;

namespace AspCoreTools.Jwt
{
    public static class ClaimsExtensions
    {
        /// <summary>
        /// Returns true if the user has the specified role for the entity with the given Id.
        /// </summary>
        /// <param name="user"></param>
        /// <param name="role">The role, e.x. Instructor</param>
        /// <param name="entityId">The id of the entity</param>
        /// <returns></returns>
        public static bool IsInRoleForEntity(this ClaimsPrincipal user, RoleType role, string entityId)
        {
            var claims = user.Claims;

            return claims.Any(c => c.Type == role && c.Value == entityId);
        }

        public static string GetId(this ClaimsPrincipal user)
        {
            var claim = user.Claims.FirstOrDefault(c => c.Type == RoleType.UserIdClaimType);

            return claim?.Value;
        }
    }
}
