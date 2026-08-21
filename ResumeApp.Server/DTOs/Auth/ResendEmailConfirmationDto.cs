using System.ComponentModel.DataAnnotations;

namespace ResumeApp.Server.DTOs.Auth
{
    // Added to validate resend-verification requests.
    public class ResendEmailConfirmationDto
    {
        [Required]
        [EmailAddress]
        public string Email { get; set; } = string.Empty;
    }
}