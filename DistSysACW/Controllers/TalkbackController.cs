using Microsoft.AspNetCore.Mvc;
using System;
using System.Collections.Generic;
using System.Text;

namespace DistSysACW.Controllers
{
    public class TalkBackController : BaseController
    {
        /// <summary>
        /// Constructs a TalkBack controller, taking the UserContext through dependency injection
        /// </summary>
        /// <param name="context">DbContext set as a service in Startup.cs and dependency injected</param>
        public TalkBackController(Models.UserContext context) : base(context) { }


        [ActionName("Hello")]
        public String Get() => "Hello World";

        [ActionName("Sort")]
        public IActionResult Get([FromQuery(Name = "integers")] String[] integerStrings)
        {
            if (integerStrings.Length == 0)
            {
                return this.Ok("[]");
            }

            int[] integers = new int[integerStrings.Length];

            for (int i = 0; i < integerStrings.Length; i ++)
            {
                try
                {
                    integers[i] = Int32.Parse(integerStrings[i]);
                }

                catch
                {
                    return this.BadRequest("Bad Request");
                }
            }

            List<int> sortedIntegers = new List<int>(integers);
            sortedIntegers.Sort();
            integers = sortedIntegers.ToArray();
            StringBuilder sb = new StringBuilder();
            sb.Append("[" + integers[0]);

            for (int i = 1; i < sortedIntegers.Count; i ++)
            {
                sb.Append("," + integers[i]);
            }

            sb.Append("]");
            return this.Ok(sb.ToString());
        }
    }
}
