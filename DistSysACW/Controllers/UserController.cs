using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using System;
using System.Net;
using System.Threading.Tasks;

using DistSysACW.Models;

namespace DistSysACW.Controllers
{
    public class UserController : BaseController
    {
        private const String FalseString = "\"False - User Does Not Exist! Did you mean to do a POST to create a new user?\"";
        private const String TrueString = "\"True - User Does Exist! Did you mean to do a POST to create a new user?\"";

        /// <summary>
        /// Constructs a User controller, taking the UserContext through dependency injection
        /// </summary>
        /// <param name="context">DbContext set as a service in Startup.cs and dependency injected</param>
        public UserController(Models.UserContext context) : base(context) { }

        [ActionName("ChangeRole")]
        [Authorize(Roles = "Admin")]
        [HttpPost]
        public async Task<IActionResult> ChangeRoleAsync([FromHeader] String apiKey, [FromBody] User inputUser)
        {
            User admin = UserDatabaseAccess.Get(this.Context, apiKey);
            Log log = new Log(this.ControllerContext.ActionDescriptor.AttributeRouteInfo.Template);
            admin.Logs.Add(log);
            this.Context.Logs.Add(log);
            User user = null;

            foreach (User item in this.Context.Users)
            {
                if (item.UserName.Equals(inputUser.UserName))
                {
                    user = item;
                    break;
                }
            }

            if (user is null)
            {
                return this.BadRequest("NOT DONE: Username does not exist");
            }

            try
            {
                if (!inputUser.IsRoleCorrect)
                {
                    return this.BadRequest("NOT DONE: Role does not exist");
                }

                user.Role = inputUser.Role;
                await this.Context.SaveChangesAsync();
                return this.Ok("DONE");
            }

            catch
            {
                return this.BadRequest("NOT DONE: An error occured");
            }
        }

        [ActionName("New")]
        [HttpGet]
        public IActionResult New([FromQuery] String userName)
        {
            if (String.IsNullOrWhiteSpace(userName))
            {
                return this.Ok(UserController.FalseString);
            }

            foreach (User user in this.Context.Users)
            {
                if (user.UserName.Equals(userName))
                {
                    return this.Ok(UserController.TrueString);
                }
            }

            return this.Ok(UserController.FalseString);
        }

        [ActionName("New")]
        [HttpPost]
        public async Task<IActionResult> NewAsync([FromBody] String userName)
        {
            const String emptyMessage = "Oops. Make sure your body contains a string with your username and your Content-Type is Content-Type:application/json";

            if (userName is null)
            {
                return this.BadRequest(emptyMessage);
            }

            userName = userName.TrimEnd('"').TrimStart('"');

            if (String.IsNullOrWhiteSpace(userName))
            {
                return this.BadRequest(emptyMessage);
            }

            foreach (User user in this.Context.Users)
            {
                if (user.UserName.Equals(userName))
                {
                    return this.StatusCode((int) HttpStatusCode.Forbidden, "Oops. This username is already in use. Please try again with a new username.");
                }
            }

            User createdUser = await UserDatabaseAccess.CreateAsync(this.Context, userName);
            return this.Ok(createdUser.ApiKey);
        }

        [ActionName("RemoveUser")]
        [Authorize(Roles = "Admin,User")]
        [HttpDelete]
        public async Task<IActionResult> RemoveUserAsync([FromHeader] String apiKey, [FromQuery] String userName)
        {
            if (String.IsNullOrWhiteSpace(apiKey) || String.IsNullOrWhiteSpace(userName))
            {
                return this.Ok(false);
            }

            User user = UserDatabaseAccess.Get(this.Context, apiKey);

            if (!(user is null) && user.UserName == userName)
            {
                Log log = new Log(this.ControllerContext.ActionDescriptor.AttributeRouteInfo.Template);
                user.Logs.Add(log);
                this.Context.Logs.Add(log);
                await UserDatabaseAccess.DeleteAsync(this.Context, apiKey);
                return this.Ok(true);
            }

            return this.Ok(false);
        }
    }
}
