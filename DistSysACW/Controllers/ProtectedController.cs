using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

using DistSysACW.Models;

namespace DistSysACW.Controllers
{
    [ApiController]
    [Route("api/[controller]/[action]")]
    public class ProtectedController : BaseController
    {
        public ProtectedController(UserContext context) : base(context) { }

        [ActionName("Hello")]
        [HttpGet]
        public IActionResult Hello([FromHeader] String apiKey)
        {
            User user = UserDatabaseAccess.Get(this.Context, apiKey);
            return user is null ? (IActionResult) this.BadRequest("ERROR: user not found") : this.Ok($"Hello {user.UserName}");
        }

        [ActionName("SHA1")]
        [HttpGet]
        public IActionResult SHA1([FromQuery] String message)
        {
            throw new NotImplementedException();
        }

        [ActionName("SHA256")]
        [HttpGet]
        public IActionResult SHA256([FromQuery] String message)
        {
            throw new NotImplementedException();
        }
    }
}