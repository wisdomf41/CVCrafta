namespace ResumeApp.Server.DTOs.Resume
{
    public class UpdateResumeDto
    {
        public string Name { get; set; } = string.Empty;
        public string Title { get; set; } = string.Empty;
        public string? Email { get; set; }
        public string Url { get; set; } = string.Empty;
        public string Stack { get; set; } = string.Empty;
        public string? Country { get; set; }
        public string? Summary { get; set; }
    }
}
