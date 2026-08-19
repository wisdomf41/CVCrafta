using System.ComponentModel.DataAnnotations;

namespace ResumeApp.Server.DTOs.Auth
{
    public class RegisterDto
    {   [Required]
        public string FullName { get; set; } = string.Empty;
        [Required]
        [EmailAddress]
        public string Email { get; set; } = string.Empty;
        [Required]
        [StringLength(50)]
        public string Password { get; set; } = string.Empty;
    }
}
