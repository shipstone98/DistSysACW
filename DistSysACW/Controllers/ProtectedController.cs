using Microsoft.AspNetCore.Mvc;
using System;
using System.Security.Cryptography;
using System.Text;
using System.Threading.Tasks;

using DistSysACW.Models;

namespace DistSysACW.Controllers
{
    [ApiController]
    [Route("api/[controller]/[action]")]
    public class ProtectedController : BaseController
    {
        public ProtectedController(UserContext context) : base(context) { }

        private static String ConvertByteArrayToString(byte[] arr)
        {
            StringBuilder sb = new StringBuilder(arr.Length * 2);

            foreach (byte b in arr)
            {
                sb.AppendFormat("{0:x2}", b);
            }

            return sb.ToString();
        }

        [ActionName("Hello")]
        [HttpGet]
        public async Task<IActionResult> HelloAsync([FromHeader] String apiKey)
        {
            User user = UserDatabaseAccess.Get(this.Context, apiKey);

            if (user is null)
            {
                return this.BadRequest("ERROR: user not found");
            }
            
            else
            {
                String name = this.ControllerContext.ActionDescriptor.AttributeRouteInfo.Template;
                user.Logs.Add(new Log(name));
                await this.Context.SaveChangesAsync();
                return this.Ok($"Hello {user.UserName}");
            }
        }

        [ActionName("SHA1")]
        [HttpGet]
        public IActionResult SHA1([FromQuery] String message)
        {
            if (String.IsNullOrEmpty(message))
            {
                return this.BadRequest("Bad Request");
            }

            SHA1 provider = new SHA1CryptoServiceProvider();
            byte[] asciiMessage = Encoding.ASCII.GetBytes(message);
            byte[] encryptedMessage = provider.ComputeHash(asciiMessage);
            return this.Ok(ProtectedController.ConvertByteArrayToString(encryptedMessage));
        }

        [ActionName("SHA256")]
        [HttpGet]
        public IActionResult SHA256([FromQuery] String message)
        {
            if (String.IsNullOrEmpty(message))
            {
                return this.BadRequest("Bad Request");
            }

            SHA256 provider = new SHA256CryptoServiceProvider();
            byte[] asciiMessage = Encoding.ASCII.GetBytes(message);
            byte[] encryptedMessage = provider.ComputeHash(asciiMessage);
            return this.Ok(ProtectedController.ConvertByteArrayToString(encryptedMessage));
        }
    }
}