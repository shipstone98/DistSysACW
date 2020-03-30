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
        private const String FalseString = "False - User Does Not Exist! Did you mean to do a POST to create a new user?";
        private const String TrueString = "True - User Does Exist! Did you mean to do a POST to create a new user?";

        /// <summary>
        /// Constructs a User controller, taking the UserContext through dependency injection
        /// </summary>
        /// <param name="context">DbContext set as a service in Startup.cs and dependency injected</param>
        public UserController(Models.UserContext context) : base(context) { }

        [ActionName("ChangeRole")]
        [Authorize(Roles = "Admin")]
        [HttpPost]
        public async Task<IActionResult> ChangeRole([FromHeader] String apiKey, [FromBody] User inputUser)
        {


            /*UserRole role;

            try
            {
                role = (UserRole) Enum.Parse(typeof (UserRole), userName);
            }

            catch
            {
                return this.BadRequest("NOT DONE: Role does not exist");
            }
            */

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
        public async Task<IActionResult> NewAsync([FromBody] User userName)
        {
            if (userName is null || String.IsNullOrWhiteSpace(userName.UserName))
            {
                return this.BadRequest("Oops. Make sure your body contains a string with your username and your Content-Type is Content-Type:application/json");
            }

            foreach (User user in this.Context.Users)
            {
                if (user.UserName.Equals(userName.UserName))
                {
                    return this.StatusCode((int) HttpStatusCode.Forbidden, "Oops. This username is already in use. Please try again with a new username.");
                }
            }

            User createdUser = await UserDatabaseAccess.CreateAsync(this.Context, userName.UserName);
            return this.Ok(createdUser.ApiKey);
        }

        [ActionName("RemoveUser")]
        [Authorize(Roles = "Admin")]
        [HttpDelete]
        public async Task<bool> RemoveUserAsync([FromHeader] String apiKey, [FromQuery] String userName)
        {
            if (String.IsNullOrWhiteSpace(apiKey) || String.IsNullOrWhiteSpace(userName))
            {
                return false;
            }

            if (UserDatabaseAccess.Exists(this.Context, apiKey, userName))
            {
                await UserDatabaseAccess.DeleteAsync(this.Context, apiKey);
                return true;
            }

            return false;
        }
    }
}
