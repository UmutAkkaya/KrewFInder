using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Security.Claims;
using System.Threading.Tasks;
using AspCoreTools;
using AspCoreTools.Jwt;
using Microsoft.AspNetCore.Authentication;
using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace AuthenticationService.Controllers
{
    using DatabaseLayer.Models;
    using User = DatabaseLayer.Models.User;
    using MongoDBLayer = DatabaseLayer.MongoDBLayer;
    using Crypto;

    [Route("api/Auth")]
    public class AuthenticationController : Controller
    {
        [MultiArgJsonBody]
        [HttpPost("login")]
        public async Task<User> Login(string login, string password)
        {
            // login is email, password is plaintext password
            var user = await MongoDBLayer.FindOne<User>(usr => usr.Email.Equals(login));

            if (user is null || !PasswordUtils.ComparePassword(password, user.Password))
            {
                return HttpContext.SendUnauthorized<User>("This combination of email and password was not found");
            }

            
            user.GlobalRoles.Add(GlobalRole.CourseCreator);
            await user.Save();

            JwtTools.AddLoginHeader(HttpContext, user.IdStr, null,
                user.GlobalRoles.Select(x => x.ToString()).ToArray());
            user.Password = null;
            
            return user;
        }

        [HttpPost("logout")]
        public async Task Logout([UidFromToken] String userId)
        {
            var user = await DatabaseLayer.Models.User.Load(userId);
            if (user is null)
            {
                HttpContext.SendBadRequest("Bad Request");
                return;
            }

            JwtTools.AddLoginHeader(HttpContext, user.IdStr, new[] {new Claim(RoleType.VoidToken, "*")});
            HttpContext.SendOk();
        }
    }
}