using Microsoft.EntityFrameworkCore;
using ResumeApp.Server.Data;
using ResumeApp.Server.Services.Implementations;
using ResumeApp.Server.Services.Interfaces;

var builder = WebApplication.CreateBuilder(args);

//Registering Services
builder.Services.AddScoped<IResumeService, ResumeService>();
builder.Services.AddScoped<IExperienceService, ExperienceService>();
builder.Services.AddScoped<ISkillService, SkillService>();
builder.Services.AddScoped<IProjectService, ProjectService>();
builder.Services.AddScoped<ICertificationService, CertificationService>();
builder.Services.AddScoped<IEducationService, EducationService>();
builder.Services.AddScoped<IInterestService, InterestService>();
builder.Services.AddScoped<IRefereeService, RefereeService>();


builder.Services.AddControllers();
// Learn more about configuring OpenAPI at https://aka.ms/aspnet/openapi

//Add Cors to allow cross-origin requests from the client application. This is necessary when the client and
//server are hosted on different domains or ports. The configuration allows any origin, method, and header,
//which is suitable for development but should be restricted in production for security reasons.
builder.Services.AddCors(options =>
{
    options.AddDefaultPolicy(policy =>
    {
        policy.AllowAnyOrigin()
              .AllowAnyMethod()
              .AllowAnyHeader();
    });
});

// Add services to the Db.
builder.Services.AddDbContext<ResumeDbContext>(options =>
    options.UseSqlServer(builder.Configuration.GetConnectionString("DefaultConnection")));

//Swagger connection
builder.Services.AddEndpointsApiExplorer();
builder.Services.AddSwaggerGen();


builder.Services.AddOpenApi();

var app = builder.Build();

//app.UseDefaultFiles(); // Must be here for publishing
//app.MapStaticAssets(); // Must be here for publishing

// Configure the HTTP request pipeline.
if (app.Environment.IsDevelopment())
{
    app.UseSwagger();
    app.UseSwaggerUI();
}

app.UseHttpsRedirection();

app.UseCors();

app.UseAuthorization();

app.MapControllers();

//app.MapFallbackToFile("/index.html"); // Must be here for publishing

app.Run();
