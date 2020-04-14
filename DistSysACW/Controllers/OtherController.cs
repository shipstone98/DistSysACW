using Microsoft.AspNetCore.Mvc;
using System.Threading.Tasks;

using DistSysACW.Models;

namespace DistSysACW.Controllers
{
    public class OtherController : BaseController
    {
        public OtherController(UserContext context): base(context) { }

        [ActionName("Clear")]
        [HttpGet]
        public async Task<IActionResult> ClearAsync()
        {
            this.Context.LogArchive.RemoveRange(this.Context.LogArchive);
            this.Context.Logs.RemoveRange(this.Context.Logs);
            this.Context.Users.RemoveRange(this.Context.Users);
            await this.Context.SaveChangesAsync();
            return this.Ok("Success, all data cleared.");
        }
    }
}
