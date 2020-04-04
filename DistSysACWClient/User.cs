using System;

namespace DistSysACWClient
{
    public class User
    {
        public String ApiKey { get; set; }
        public String ErrorMessage { get; set; }
        public RoleType Role { get; set; }
        public String UserName { get; set; }

        public enum RoleType
        {
            User,
            Admin
        }
    }
}