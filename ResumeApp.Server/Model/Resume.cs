using Humanizer;
using ResumeApp.Server.ApplicationUserModel;

namespace ResumeApp.Server.Model
{
    public class Resume
    {
        public int Id { get; set; }
        public string Name { get; set; } = string.Empty;
        public string Title { get; set; } = string.Empty;
        public string? Email { get; set; }
        public string Url { get; set; } = string.Empty;
        public string Stack { get; set; } = string.Empty;
        public string? Country { get; set; }
        public string? Summary { get; set; }

        public DateTime CreatedAt { get; set; }
        public DateTime UpdatedAt { get; set; }

        /*
        UserId stores the logged-in user’s ID
        User is the navigation property back to the owner

        So one resume now belongs to one account.
        */
        public string? UserId { get; set; } 
        public ApplicationUser? User { get; set; }

        public ICollection<Experience> Experiences { get; set; } = new List<Experience>();
        public ICollection<Skill> Skills { get; set; } = new List<Skill>();
        public ICollection<Project> Projects { get; set; } = new List<Project>();
        public ICollection<Certification> Certifications { get; set; } = new List<Certification>();
        public ICollection<Education> Educations { get; set; } = new List<Education>();
        public ICollection<Interest> Interests { get; set; } = new List<Interest>();
        public ICollection<Referee> Referees { get; set; } = new List<Referee>();
    }
}