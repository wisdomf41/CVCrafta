namespace ResumeApp.Server.DTOs.Interest
{
    public class UpdateInterestDto
    {

        public string Title { get; set; } = string.Empty;
        public string Description { get; set; } = string.Empty;
        public int DisplayOrder { get; set; }
        public int ResumeId { get; set; }
    }
}
