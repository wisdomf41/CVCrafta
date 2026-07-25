using System.Reflection;

namespace ResumeApp.Server.Model
{
    public class Education
    {
        public int Id { get; set; }
        public string Institution { get; set; } = string.Empty;
        public string Degree { get; set; } = string.Empty;
        public string FieldOfStudy { get; set; } = string.Empty;
        public string Grade { get; set; } = string.Empty;
        public int YearGraduated { get; set; }
        public int DisplayOrder { get; set; }
        public int ResumeId { get; set; }
        public Resume Resume { get; set; } = null!;
    }
}
