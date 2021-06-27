using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Filters;
using Microsoft.IdentityModel.Tokens;
using System.Linq;

namespace AspCoreTools.Jwt
{
    /// <summary>
    /// Place this attribute on any function that needs to have a user be authenticated for access.
    /// Use NoValueClaims to enforce claims by type (regardless of value). Useful for global roles.
    /// 
    /// This attribute will return unauth to anyone who tries to call its action and is not authenticated
    /// at all or with one of the novalueclaims. After that, it is up to the action to verify specific
    /// claims and decide whether the user can call it.
    /// </summary>
    public class JwtAuthAttribute : ActionFilterAttribute
    {
        public string[] NoValueClaims { get; set; } = new string[0];
        public JwtAuthAttribute(params string[] noValueClaims)
        {
            NoValueClaims = noValueClaims;
        }

        public override void OnActionExecuting(ActionExecutingContext context)
        {
            var user = context.HttpContext.User;
            if (user == null || 
                !(user.Identity?.IsAuthenticated ?? false) || 
                (NoValueClaims.Any() && !NoValueClaims.Any(c=>user.Claims.Any(uc=>uc.Type == c))))
            {
                context.Result = new UnauthorizedResult();
            }
        }
    }
}