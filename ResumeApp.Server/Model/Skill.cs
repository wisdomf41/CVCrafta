namespace ResumeApp.Server.Model
{
    public class Skill
    {
            public int Id { get; set; }
            public string Category { get; set; } = string.Empty; // "Backend", "Database", "Frontend", etc.
            public string Skills { get; set; } = string.Empty;    // "C#, .NET Core Web API, REST APIs"
            public int DisplayOrder { get; set; }                 // To control order (1,2,3,4,5)
            public int ResumeId { get; set; }                     // Foreign key
            public Resume Resume { get; set; } = null!;
        
    }
}

