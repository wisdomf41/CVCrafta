using Microsoft.AspNetCore.Identity;

namespace ResumeApp.Server.ApplicationUserModel
{
    public class ApplicationUser : IdentityUser
    {
        public string FullName { get; set; } = string.Empty;
    }
}
