using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using System;
using System.IO;
using System.Net;
using System.Threading.Tasks;

using Newtonsoft.Json;

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
            User admin = UserDatabaseAccess.Get(this.Context, apiKey);
            admin.Logs.Add(new Log(this.ControllerContext.ActionDescriptor.AttributeRouteInfo.Template));
            Task awaiter = this.Context.SaveChangesAsync();
            User user = null;

            foreach (User item in this.Context.Users)
            {
                if (item.UserName.Equals(inputUser.UserName))
                {
                    user = item;
                    break;
                }
            }

            await awaiter;

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
        public async Task<IActionResult> NewAsync()
        {
            const String emptyMessage = "Oops. Make sure your body contains a string with your username and your Content-Type is Content-Type:application/json";
            String body;

            using (StreamReader sr = new StreamReader(this.Request.Body))
            {
                body = await sr.ReadToEndAsync();
            }

            if (String.IsNullOrWhiteSpace(body) || !body.ToLower().StartsWith("\"username\":"))
            {
                return this.BadRequest(emptyMessage);
            }

            body = $"{{{body}}}";
            String userName = null;

            using (TextReader tr = new StringReader(body))
            {
                using (JsonReader jr = new JsonTextReader(tr))
                {
                    bool active = false;

                    try
                    {
                        while (await jr.ReadAsync())
                        {
                            if (jr.TokenType == JsonToken.PropertyName)
                            {
                                String value = jr.Value.ToString();
                                active = value.ToLower() == "username";
                            }

                            else if (jr.TokenType == JsonToken.String)
                            {
                                if (active)
                                {
                                    userName = jr.Value.ToString();
                                    break;
                                }
                            }
                        }
                    }

                    catch
                    {
                        userName = null;
                    }
                }
            }

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
        public async Task<bool> RemoveUserAsync([FromHeader] String apiKey, [FromQuery] String userName)
        {
            if (String.IsNullOrWhiteSpace(apiKey) || String.IsNullOrWhiteSpace(userName))
            {
                return false;
            }

            User user = UserDatabaseAccess.Get(this.Context, apiKey);

            if (!(user is null) && user.UserName == userName)
            {
                user.Logs.Add(new Log(this.ControllerContext.ActionDescriptor.AttributeRouteInfo.Template));
                await UserDatabaseAccess.DeleteAsync(this.Context, apiKey);
                return true;
            }

            return false;
        }
    }
}
