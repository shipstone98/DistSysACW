using Microsoft.AspNetCore.Http;
using System;
using System.Security.Claims;
using System.Threading.Tasks;

using DistSysACW.Models;

namespace DistSysACW.Middleware
{
    public class AuthMiddleware
    {
        private readonly RequestDelegate _next;

        public AuthMiddleware(RequestDelegate next)
        {
            _next = next;
        }

        public async Task InvokeAsync(HttpContext context, UserContext dbContext)
        {
            if (context.Request.Headers.ContainsKey("ApiKey"))
            {
                String apiKey = context.Request.Headers["ApiKey"];

                if (UserDatabaseAccess.Exists(dbContext, apiKey))
                {
                    User user = UserDatabaseAccess.Get(dbContext, apiKey);
                    Claim nameClaim = new Claim(ClaimTypes.Name, user.UserName);
                    Claim roleClaim = new Claim(ClaimTypes.Role, user.Role.ToString());
                    ClaimsIdentity identity = new ClaimsIdentity(new Claim[] { nameClaim, roleClaim }, "ApiKey");
                    context.User.AddIdentity(identity);
                }
            }
            // TODO:  Find if a header ‘ApiKey’ exists, and if it does, check the database to determine if the given API Key is valid
            //        Then set the correct roles for the User, using claims

            // Call the next delegate/middleware in the pipeline
            await _next(context);
        }

    }
}
