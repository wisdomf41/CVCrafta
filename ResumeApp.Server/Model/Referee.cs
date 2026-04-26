namespace ResumeApp.Server.Model
{
    public class Referee
    {
        public int Id { get; set; }
        public string Name { get; set; } = string.Empty;
        public string Title { get; set; } = string.Empty;
        public string Company { get; set; } = string.Empty;
        public string Email { get; set; } = string.Empty;
        public string Phone { get; set; } = string.Empty;
        public string Notes { get; set; } = string.Empty;  // For "Available on request" etc.
        public int DisplayOrder { get; set; }
        public int ResumeId { get; set; }
        public Resume Resume { get; set; } = null;
    }
}
