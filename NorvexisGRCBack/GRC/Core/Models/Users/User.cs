using Microsoft.AspNetCore.Identity;

namespace GRC.Core.Models.Users
{
    public class User : IdentityUser
    {
        public string Name { get; set; } = string.Empty;
        public string LastName { get; set; } = string.Empty;
        public bool IsActive { get; set; } = true;
    }
}