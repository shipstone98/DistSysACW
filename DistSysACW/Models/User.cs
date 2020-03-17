using System;
using System.ComponentModel.DataAnnotations;
using System.Linq;
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
            String id;

            do
            {
                id = Guid.NewGuid().ToString();
            } while (UserDatabaseAccess.Exists(context, id));

            User user = new User
            {
                ApiKey = id,
                Role = context.Users.Count() == 0 ? UserRole.Admin : UserRole.User,
                UserName = userName
            };

            context.Users.Add(user);
            await context.SaveChangesAsync();
            return user;
        }

        public static async Task DeleteAsync(UserContext context, String apiKey)
        {
            User userToDelete = UserDatabaseAccess.Get(context, apiKey);

            if (!(userToDelete is null))
            {
                context.Users.Remove(userToDelete);
                await context.SaveChangesAsync();
            }
        }

        public static bool Exists(UserContext context, String apiKey) => !(UserDatabaseAccess.Get(context, apiKey) is null);

        public static bool Exists(UserContext context, String apiKey, String userName)
        {
            foreach (User user in context.Users)
            {
                if (user.ApiKey.Equals(apiKey))
                {
                    return user.UserName.Equals(userName);
                }
            }

            return false;
        }

        public static User Get(UserContext context, String apiKey)
        {
            foreach (User user in context.Users)
            {
                if (user.ApiKey.Equals(apiKey))
                {
                    return user;
                }
            }

            return null;
        }
    }
}