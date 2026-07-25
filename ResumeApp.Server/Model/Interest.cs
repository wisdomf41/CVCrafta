namespace ResumeApp.Server.Model
{
    public class Interest
    {
        public int Id { get; set; }
        public string Title { get; set; } = string.Empty;
        public string Description { get; set; } = string.Empty;
        public int DisplayOrder { get; set; }
        public int ResumeId { get; set; }
        public Resume Resume { get; set; } = null!;
    }
}
