namespace ResumeApp.Server.DTOs.Experience
{
    public class UpdateExperienceDto
    {
       
        public string Role { get; set; } = string.Empty;
        public string Company { get; set; } = string.Empty;
        public string Duration { get; set; } = string.Empty;
        public string Description { get; set; } = string.Empty;
        public int ResumeId { get; set; } //Foreign key
    }
}
