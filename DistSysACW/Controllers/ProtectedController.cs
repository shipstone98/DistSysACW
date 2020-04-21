using Microsoft.AspNetCore.Authorization;
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

        [ActionName("AddFifty")]
        [Authorize(Roles = "Admin")]
        [HttpGet]
        public IActionResult AddFifty([FromHeader] String apiKey, [FromQuery] String encryptedInteger, [FromQuery] String encryptedSymKey, [FromQuery] String encryptedIV)
        {
            const String badRequestMessage = "Bad Request";
            User user = null;

            if (String.IsNullOrWhiteSpace(apiKey) || String.IsNullOrWhiteSpace(encryptedInteger) || String.IsNullOrWhiteSpace(encryptedSymKey) || String.IsNullOrWhiteSpace(encryptedIV) || (user = UserDatabaseAccess.Get(this.Context, apiKey)) is null)
            {
                return this.BadRequest(badRequestMessage);
            }

            try
            {
                String decryptedInteger = ProtectedRepository.Decrypt(encryptedInteger);
                long integer = Int64.Parse(decryptedInteger);
                String decryptedSymKey = ProtectedRepository.Decrypt(encryptedSymKey);
                String decryptedIV = ProtectedRepository.Decrypt(encryptedIV);
                integer += 50;
                String encryptedFifty = ProtectedRepository.Encrypt(integer.ToString());
                Log log = new Log(this.ControllerContext.ActionDescriptor.AttributeRouteInfo.Template);
                user.Logs.Add(log);
                this.Context.SaveChangesAsync();
                return this.Ok(encryptedFifty);
            }
            
            catch
            {
                return this.BadRequest(badRequestMessage);
            }
        }

        [ActionName("GetPublicKey")]
        [HttpGet]
        public IActionResult GetPublicKey([FromHeader] String apiKey) => UserDatabaseAccess.Exists(this.Context, apiKey) ? (IActionResult) this.Ok(ProtectedRepository.PublicKey) : this.NotFound();

        [ActionName("Hello")]
        [Authorize(Roles = "Admin,User")]
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
                Log log = new Log(name);
                user.Logs.Add(log);
                this.Context.Logs.Add(log);
                await this.Context.SaveChangesAsync();
                return this.Ok($"Hello {user.UserName}");
            }
        }

        [ActionName("SHA1")]
        [Authorize(Roles = "Admin,User")]
        [HttpGet]
        public async Task<IActionResult> SHA1Async([FromHeader] String apiKey, [FromQuery] String message)
        {
            if (String.IsNullOrWhiteSpace(apiKey) || String.IsNullOrEmpty(message))
            {
                return this.BadRequest("Bad Request");
            }

            User user = UserDatabaseAccess.Get(this.Context, apiKey);

            if (user is null)
            {
                return this.BadRequest("ERROR: user not found");
            }
            
            else
            {
                String name = this.ControllerContext.ActionDescriptor.AttributeRouteInfo.Template;
                Log log = new Log(name);
                user.Logs.Add(log);
                this.Context.Logs.Add(log);
                await this.Context.SaveChangesAsync();
                SHA1 provider = new SHA1CryptoServiceProvider();
                byte[] asciiMessage = Encoding.ASCII.GetBytes(message);
                byte[] encryptedMessage = provider.ComputeHash(asciiMessage);
                return this.Ok(ProtectedRepository.ConvertByteArrayToString(encryptedMessage));
            }
        }

        [ActionName("SHA256")]
        [Authorize(Roles = "Admin,User")]
        [HttpGet]
        public async Task<IActionResult> SHA256Async([FromHeader] String apiKey, [FromQuery] String message)
        {
            if (String.IsNullOrWhiteSpace(apiKey) || String.IsNullOrEmpty(message))
            {
                return this.BadRequest("Bad Request");
            }

            User user = UserDatabaseAccess.Get(this.Context, apiKey);

            if (user is null)
            {
                return this.BadRequest("ERROR: user not found");
            }
            
            else
            {
                String name = this.ControllerContext.ActionDescriptor.AttributeRouteInfo.Template;
                Log log = new Log(name);
                user.Logs.Add(log);
                this.Context.Logs.Add(log);
                await this.Context.SaveChangesAsync();
                SHA256 provider = new SHA256CryptoServiceProvider();
                byte[] asciiMessage = Encoding.ASCII.GetBytes(message);
                byte[] encryptedMessage = provider.ComputeHash(asciiMessage);
                return this.Ok(ProtectedRepository.ConvertByteArrayToString(encryptedMessage));
            }
        }

        [ActionName("Sign")]
        [HttpGet]
        public async Task<IActionResult> SignAsync([FromHeader] String apiKey, [FromQuery] String message) => UserDatabaseAccess.Exists(this.Context, apiKey) ? (IActionResult) this.Ok(await ProtectedRepository.SignMessageAsync(this.Context, apiKey, message, this.ControllerContext.ActionDescriptor.AttributeRouteInfo.Template)) : this.BadRequest("Bad Request");
    }
}