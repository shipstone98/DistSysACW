using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Threading.Tasks;

namespace DistSysACW.Models
{
    public class User
    {
        public const String AdminRole = "Admin";
        public const String UserRole = "User";

        public bool IsRoleCorrect
        {
            get
            {
                if (this.Role is null)
                {
                    return false;
                }

                switch (this.Role.Trim().ToLower())
                {
                    case "admin":
                        this.Role = "Admin";
                        return true;
                    case "user":
                        this.Role = "User";
                        return true;
                    default:
                        return false;
                }
            }
        }
        
        [Key]
        public String ApiKey { get; set; }
        public ICollection<Log> Logs { get; set; }
        public String Role { get; set; }
        public String UserName { get; set; }

        public User()
        {
            this.Logs = new List<Log>();
        }
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
                Role = context.Users.Count() == 0 ? User.AdminRole : User.UserRole,
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
                foreach (Log log in userToDelete.Logs)
                {
                    LogArchive archive = new LogArchive(log, apiKey);
                    context.Logs.Remove(log);
                    context.LogArchive.Add(archive);
                }

                userToDelete.Logs.Clear();
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