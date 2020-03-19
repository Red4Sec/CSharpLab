using System;
using System.ComponentModel.DataAnnotations;
using System.Runtime.Serialization;
using System.Security.Cryptography;
using System.Text;

namespace CSharpApiLab.DB
{
    public class User
    {
        #region Roles

        public const string RoleUser = "User";
        public const string RoleSupport = "Support";
        public const string RoleAdministrator = "Administrator";

        #endregion

        public int ID { get; set; }

        [Required]
        public string Name { get; set; }
        [Required]
        public string Email { get; set; }
        [Required]
        public string Role { get; set; }

        public string Avatar { get; set; }

        [Required]
        [IgnoreDataMember]
        public string Password { get; set; }

        [Required]
        public DateTime CreatedAt { get; set; }

        [Required]
        public DateTime UpdatedAt { get; set; }

        public void UpdatePassword(string password)
        {
            this.Password = GetHashedPassword(password);
        }

        public static string GetHashedPassword(string password)
        {
            if (password == null) password = "";

            using (var md5 = MD5.Create())
            {
                var hash = md5.ComputeHash(Encoding.ASCII.GetBytes(password));
                return BitConverter.ToString(hash).Replace("-", "").ToLowerInvariant();
            }
        }
    }
}
