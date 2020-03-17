using Microsoft.AspNetCore.Mvc;
using System;
using System.Threading.Tasks;

namespace DistSysACW.Controllers
{
    public class UserController : BaseController
    {
        /// <summary>
        /// Constructs a User controller, taking the UserContext through dependency injection
        /// </summary>
        /// <param name="context">DbContext set as a service in Startup.cs and dependency injected</param>
        public UserController(Models.UserContext context) : base(context) { }

        [ActionName("New")]
        [HttpGet]
        public IActionResult New([FromQuery] String userName)
        {
            throw new NotImplementedException();
        }

        [ActionName("New")]
        [HttpPost]
        public async Task<IActionResult> NewAsync([FromBody] String userName)
        {
            throw new NotImplementedException();
        }
    }
}
