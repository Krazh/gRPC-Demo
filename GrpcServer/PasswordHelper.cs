using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using DevOne.Security.Cryptography.BCrypt;

namespace GrpcServer
{
    public class PasswordHelper
    {
        public static string GetRandomSalt()
        {
            return BCryptHelper.GenerateSalt(12);
        }

        public static string HashPassword(string password)
        {
            return BCryptHelper.HashPassword(password, GetRandomSalt());
        }

        public static bool ValidatePassword(string password, string hashedPassword)
        {
            return BCryptHelper.CheckPassword(password, hashedPassword);
        }
    }
}
