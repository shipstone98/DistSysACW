using System;
using System.ComponentModel.DataAnnotations;
using System.Threading.Tasks;

namespace DistSysACW.Models
{
    public class User
    {
        [Key]
        public String ApiKey { get; set; }
        public UserRole Role { get; set; }
        public String UserName { get; set; }
    }

    #region Task13?
    // TODO: You may find it useful to add code here for Logging
    #endregion

    public static class UserDatabaseAccess
    {
        public static async Task<User> CreateAsync(UserContext context, String userName)
        {
            throw new NotImplementedException();
        }

        public static async Task DeleteAsync(UserContext context, String apiKey)
        {
            throw new NotImplementedException();
        }

        public static bool Exists(UserContext context, String apiKey)
        {
            throw new NotImplementedException();
        }

        public static bool Exists(UserContext context, String apiKey, String userName)
        {
            throw new NotImplementedException();
        }

        public static User Get(UserContext context, String apiKey)
        {
            throw new NotImplementedException();
        }
    }
}