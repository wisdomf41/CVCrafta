using System.ComponentModel.DataAnnotations;

namespace ResumeApp.Server.DTOs.Resume
{
    public class UpdateResumeDto
    {
        [Required]
        public string Name { get; set; } = string.Empty;
        [Required]
        public string Title { get; set; } = string.Empty;
        public string? Email { get; set; }
        [Required]
        public string Url { get; set; } = string.Empty;
        [Required]
        public string Stack { get; set; } = string.Empty;
        public string? Country { get; set; }
        public string? Summary { get; set; }
    }
}
