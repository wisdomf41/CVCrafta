namespace ResumeApp.Server.DTOs.Skill
{
    public class CreateSkillDto
    {
        public string Category { get; set; } = string.Empty;
        public string Skills { get; set; } = string.Empty;
        public int DisplayOrder { get; set; }
        public int ResumeId { get; set; }
    }
}
