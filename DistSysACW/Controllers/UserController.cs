using Microsoft.AspNetCore.Mvc;
using System;
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
            throw new NotImplementedException();
        }
    }
}
