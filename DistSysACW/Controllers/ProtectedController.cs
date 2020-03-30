using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace DistSysACW.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class ProtectedController : ControllerBase
    {
        [ActionName("Hello")]
        [HttpGet]
        public IActionResult Hello([FromHeader] String apiKey)
        {
            throw new NotImplementedException();
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