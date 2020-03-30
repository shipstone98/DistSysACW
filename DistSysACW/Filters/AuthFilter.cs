using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Filters;
using System;
using System.Linq;
using System.Net;

using DistSysACW.Models;

namespace DistSysACW.Filters
{
    public class AuthFilter : AuthorizeAttribute, IAuthorizationFilter
    {
        public void OnAuthorization(AuthorizationFilterContext context)
        {
            try
            {
                AuthorizeAttribute authAttribute = (AuthorizeAttribute) context.ActionDescriptor.EndpointMetadata.Where(e => e.GetType() == typeof (AuthorizeAttribute)).FirstOrDefault();

                if (authAttribute != null)
                {
                    /*String[] roles = authAttribute.Roles.Split(',');

                    foreach (String role in roles)
                    {
                        if (context.HttpContext.User.IsInRole(role))
                        {
                            return;
                        }
                    }*/

                    if (context.HttpContext.User.IsInRole(UserRole.Admin.ToString()))
                    {
                        return;
                    }

                    throw new UnauthorizedAccessException();
                }
            }

            catch
            {
                context.HttpContext.Response.StatusCode = (int) HttpStatusCode.Unauthorized;
                context.Result = new JsonResult("Unauthorized. Admin access only.");
            }
        }
    }
}
