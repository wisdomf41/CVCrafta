namespace ResumeApp.Server.Model
{
    public class Project
    {
        public int Id { get; set; }
        public string Name { get; set; } = string.Empty;
        public string Technologies { get; set; } = string.Empty;
        public string Description { get; set; } = string.Empty;
        public string ProjectUrl { get; set; } = string.Empty;  // Optional: link to live project
        public string GithubUrl { get; set; } = string.Empty;   // Optional: link to source code
        public int DisplayOrder { get; set; }
        public int ResumeId { get; set; }  // Foreign key
        public Resume Resume { get; set; } = null!;
    }
}
