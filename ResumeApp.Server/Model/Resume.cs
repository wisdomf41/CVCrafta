namespace ResumeApp.Server.Model
{
    public class Resume
    {
        public int Id { get; set; }
        public string Name { get; set; } = string.Empty;
        public string Title { get; set; } = string.Empty;
        public string? Email { get; set; }
    }
}
