namespace ResumeApp.Server.Model
{
    public class Certification
    {
        public int Id { get; set; }
        public string Name { get; set; } = string.Empty;
        public string Issuer { get; set; } = string.Empty;  // Udemy, Coursera, Udacity, etc.
        public int YearObtained { get; set; }
        public string CredentialUrl { get; set; } = string.Empty;  // Optional: link to verify
        public int DisplayOrder { get; set; }
        public int ResumeId { get; set; }  // Foreign key
        public Resume Resume { get; set; } = null!;
    }
}
