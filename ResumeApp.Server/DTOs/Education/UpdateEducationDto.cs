namespace ResumeApp.Server.DTOs.Education
{
    public class UpdateEducationDto
    {
        public string Institution { get; set; } = string.Empty;
        public string Degree { get; set; } = string.Empty;
        public string FieldOfStudy { get; set; } = string.Empty;
        public string Grade { get; set; } = string.Empty;
        public int YearGraduated { get; set; }
        public int DisplayOrder { get; set; }
        public int ResumeId { get; set; }
    }
}
