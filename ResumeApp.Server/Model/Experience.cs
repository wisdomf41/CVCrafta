namespace ResumeApp.Server.Model
{
    public class Experience
    {
        public int Id { get; set; }
        public string Role { get; set; } = string.Empty;
        public string Company { get; set; } = string.Empty;
        public string Duration { get; set; } = string.Empty;
        public string Description { get; set; } = string.Empty;
        public int ResumeId { get; set; } //Foreign key
        public Resume Resume { get; set; } = null;
    }
}
