using System;
using System.ComponentModel.DataAnnotations;

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
        #region Task3 
        // TODO: Make methods which allow us to read from/write to the database 
        #endregion
    }
}