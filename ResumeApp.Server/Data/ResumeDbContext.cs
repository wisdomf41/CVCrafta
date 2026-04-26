using Microsoft.EntityFrameworkCore;
using ResumeApp.Server.Model;

namespace ResumeApp.Server.Data
{
    public class ResumeDbContext : DbContext
    {
        public ResumeDbContext(DbContextOptions<ResumeDbContext> options) : base(options)
        {
        }

        public DbSet<Resume> Resumes { get; set; }
        public DbSet<Experience> Experiences { get; set; }
        public DbSet<Skill> Skills { get; set; }
        public DbSet<Project> Projects { get; set; }
        public DbSet<Certification> Certifications { get; set; }
        public DbSet<Education> Educations { get; set; }
        public DbSet<Interest> Interests { get; set; }
        public DbSet<Referee> Referees { get; set; }

        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {
            base.OnModelCreating(modelBuilder);

            modelBuilder.Entity<Resume>()
                .HasMany(r => r.Experiences)
                .WithOne(e => e.Resume)
                .HasForeignKey(e => e.ResumeId)
                .OnDelete(DeleteBehavior.Cascade);

            modelBuilder.Entity<Resume>()
                .HasMany(r => r.Skills)
                .WithOne(s => s.Resume)
                .HasForeignKey(s => s.ResumeId)
                .OnDelete(DeleteBehavior.Cascade);

            modelBuilder.Entity<Resume>()
                .HasMany(r => r.Projects)
                .WithOne(p => p.Resume)
                .HasForeignKey(p => p.ResumeId)
                .OnDelete(DeleteBehavior.Cascade);

            modelBuilder.Entity<Resume>()
                .HasMany(r => r.Certifications)
                .WithOne(c => c.Resume)
                .HasForeignKey(c => c.ResumeId)
                .OnDelete(DeleteBehavior.Cascade);

            modelBuilder.Entity<Resume>()
                .HasMany(r => r.Educations)
                .WithOne(e => e.Resume)
                .HasForeignKey(e => e.ResumeId)
                .OnDelete(DeleteBehavior.Cascade);

            modelBuilder.Entity<Resume>()
                .HasMany(r => r.Interests)
                .WithOne(i => i.Resume)
                .HasForeignKey(i => i.ResumeId)
                .OnDelete(DeleteBehavior.Cascade);

            modelBuilder.Entity<Resume>()
                .HasMany(r => r.Referees)
                .WithOne(rf => rf.Resume)
                .HasForeignKey(rf => rf.ResumeId)
                .OnDelete(DeleteBehavior.Cascade);

            modelBuilder.Entity<Resume>().HasData(
                new Resume
                {
                    Id = 1,
                    Name = "Future Ibeche",
                    Title = "C# /.NET Developer",
                    Email = "wisdomf41@gmail.com",
                    Url = "GitHub (@wisdomf41) / LinkedIn (@ibechefuture)",
                    Stack = "React | SQL | ASP.NET Core | REST API",
                    Country = "Nigeria",
                    Summary = "Full-Stack .NET Developer with experience in building full-stack applications using C#, ASP.NET Core Web API, and React. Skilled in developing RESTful APIs, SQL Server, and Entity Framework Core, with a focus on performance and scalability. Experienced in implementing CI/CD pipelines (GitHub Actions/Azure DevOps) to automate builds and deployments. Strong collaborator in Agile and remote teams, passionate about building efficient, maintainable systems.",
                    CreatedAt = new DateTime(2026, 1, 1),
                    UpdatedAt = new DateTime(2026, 1, 1)
                }
            );

            modelBuilder.Entity<Experience>().HasData(
                new Experience
                {
                    Id = 1,
                    Role = "Full-Stack .NET Developer",
                    Company = "Metclan Technologies",
                    Duration = "2025 - 2026",
                    Description = "Developed 3+ full-stack applications using ASP.NET Core Web API and React.|Built reusable React components, reducing development time by 30%.|Integrated REST APIs and optimized queries with Entity Framework Core, improving performance by 25%.|Implemented CI/CD pipelines using GitHub Actions and Azure DevOps.|Collaborated in an Agile team using Git for version control.",
                    ResumeId = 1
                },
                new Experience
                {
                    Id = 2,
                    Role = "Software Developer",
                    Company = "Loctech Training Institute",
                    Duration = "2024 - 2025",
                    Description = "Developed applications using .NET, C#, HTML, CSS, JavaScript, and jQuery.|Contributed to the development of a healthcare management system, improving patient record handling and data accessibility.|Diagnosed and resolved 20+ software bugs, enhancing system reliability and user satisfaction.|Assisted in application testing, debugging, and deployment processes.",
                    ResumeId = 1
                },
                new Experience
                {
                    Id = 3,
                    Role = "IT/Network Specialist",
                    Company = "FirstBank Limited",
                    Duration = "2021 - 2024",
                    Description = "Installed, customized, and supported desktop and laptop workstations plus software systems for 100+ in-office and remote users.|Refurbished 100+ Windows 8 machines to Windows 10 in preparation for an Office 365 rollout across eight regional offices.|Mentored junior team members and supported IT systems operations.|Managed cloud migration for storage and backup systems.|Handled 20 to 40 onsite IT support calls daily.",
                    ResumeId = 1
                }
            );

            // Seed Skills data
            modelBuilder.Entity<Skill>().HasData(
                new Skill
                {
                    Id = 1,
                    Category = "Backend",
                    Skills = "C#, .NET Core Web API, REST APIs",
                    DisplayOrder = 1,
                    ResumeId = 1
                },
                new Skill
                {
                    Id = 2,
                    Category = "Database",
                    Skills = "SQL Server, Entity Framework Core, LINQ",
                    DisplayOrder = 2,
                    ResumeId = 1
                },
                new Skill
                {
                    Id = 3,
                    Category = "Frontend",
                    Skills = "React, HTML 5, CSS3, JavaScript (ES6+), Vite",
                    DisplayOrder = 3,
                    ResumeId = 1
                },
                new Skill
                {
                    Id = 4,
                    Category = "Tools",
                    Skills = "Git, GitHub, Visual Studio, Azure",
                    DisplayOrder = 4,
                    ResumeId = 1
                },
                new Skill
                {
                    Id = 5,
                    Category = "Concepts",
                    Skills = "Full-Stack Development, API Integration, Component-Based Architecture, Authentication & Authorization",
                    DisplayOrder = 5,
                    ResumeId = 1
                }
            );

            modelBuilder.Entity<Project>().HasData(
                new Project
                {
                    
                    Id = 1,
                    Name = "Resume Website (This Project)",
                    Technologies = "React + Vite frontend, ASP.NET Core backend, Azure App Service",
                    Description = "Full-stack resume application with responsive design, deployed to Azure cloud.",
                    DisplayOrder = 1,
                    ResumeId = 1
                },
                new Project
                {
                    Id = 2,
                    Name = "Task Management API with React Frontend",
                    Technologies = "ASP.NET Core Web API, React, SQL Server, Entity Framework",
                    Description = "Full-stack task management system with CRUD operations, authentication, and responsive UI.",
                    DisplayOrder = 2,
                    ResumeId = 1
                },
                new Project
                {
                    Id = 3,
                    Name = "E-Commerce MVC Application",
                    Technologies = "ASP.NET Core MVC, HTML 5, CSS 3, JavaScript, SQL Server",
                    Description = "Complete e-commerce solution with product management, shopping cart, and order processing.",
                    DisplayOrder = 3,
                    ResumeId = 1
                },
                new Project
                {
                    Id = 4,
                    Name = "Hotel Booking API",
                    Technologies = "ASP.NET Core Web API, SQL Server, Entity Framework",
                    Description = "RESTful API for hotel room booking with customer management and reservation tracking.",
                    DisplayOrder = 4,
                    ResumeId = 1
                }
            );
            modelBuilder.Entity<Certification>().HasData(
                new Certification {
                    Id = 1,
                    Name = ".NET Core Developer Certification",
                    Issuer = "Udemy",
                    YearObtained = 2025,
                    DisplayOrder = 1,
                    ResumeId = 1
                },
                new Certification
                {
                    Id = 2,
                    Name = ".NET Core API Developer Certification",
                    Issuer = "Udemy",
                    YearObtained = 2025,
                    DisplayOrder = 2,
                    ResumeId = 1
                },
                new Certification
                {
                    Id = 3,
                    Name = "Google IT Support Professional Certificate",
                    Issuer = "Coursera",
                    YearObtained = 2024,
                    DisplayOrder = 3,
                    ResumeId = 1
                },
                new Certification
                {
                    Id = 4,
                    Name = "SQL for Data Analysis",
                    Issuer = "Udacity",
                    YearObtained = 2022,
                    DisplayOrder = 4,
                    ResumeId = 1
                }
            );
            modelBuilder.Entity<Education>().HasData(
                new Education {
                    Id = 1,
                    Institution = "University of Port-Harcourt",
                    Degree = "Bachelor of Engineering",
                    FieldOfStudy = "Mechanical Engineering",
                    Grade = "2nd Class Upper Division",
                    YearGraduated = 2018,
                    DisplayOrder = 1,
                    ResumeId = 1
                }
            );
            modelBuilder.Entity<Interest>().HasData(
                new Interest
                {
                    Id = 1,
                    Title = "Professional Interests",
                    Description = "Continuous learning in the .NET ecosystem. Building scalable and maintainable software solutions.",
                    DisplayOrder = 1,
                    ResumeId = 1
                }
            );
            modelBuilder.Entity<Referee>().HasData(
                new Referee
                {
                    Id = 1,
                    Name = "Available on request",
                    Title = "",
                    Company = "",
                    Email = "",
                    Phone = "",
                    Notes = "Professional references available upon request",
                    DisplayOrder = 1,
                    ResumeId = 1
                }
            );
        }
    }
}