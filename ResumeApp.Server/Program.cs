using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;
using Microsoft.IdentityModel.Tokens;
using Microsoft.OpenApi;
using ResumeApp.Server.ApplicationUserModel;
using ResumeApp.Server.Data;
using ResumeApp.Server.Services.Implementations;
using ResumeApp.Server.Services.Interfaces;
using System.Text;




var builder = WebApplication.CreateBuilder(args);

// Add services to the container.
builder.Services.AddControllers();

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

//Add Identity and JWT config before builder.Build();
builder.Services.AddIdentity<ApplicationUser, IdentityRole>()
    .AddEntityFrameworkStores<ResumeDbContext>()
    .AddDefaultTokenProviders();

// Added a two-hour lifetime for email-confirmation tokens.
builder.Services.Configure<DataProtectionTokenProviderOptions>(options =>
{
    options.TokenLifespan = TimeSpan.FromHours(2);
});

//JWT Configuration Varables
var jwtKey = builder.Configuration["Jwt:Key"]
    ?? throw new InvalidOperationException(
        "JWT signing key is missing. Configure Jwt:Key using User Secrets or environment variables.");

var jwtIssuer = builder.Configuration["Jwt:Issuer"]
    ?? throw new InvalidOperationException(
        "JWT issuer is missing. Configure Jwt:Issuer.");

var jwtAudience = builder.Configuration["Jwt:Audience"]
    ?? throw new InvalidOperationException(
        "JWT audience is missing. Configure Jwt:Audience.");


builder.Services.AddAuthentication(options =>
{
    options.DefaultAuthenticateScheme = JwtBearerDefaults.AuthenticationScheme;
    options.DefaultChallengeScheme = JwtBearerDefaults.AuthenticationScheme;
})
.AddJwtBearer(options =>
{
    options.TokenValidationParameters = new TokenValidationParameters
    {
        ValidateIssuer = true,
        ValidateAudience = true,
        ValidateLifetime = true,
        ValidateIssuerSigningKey = true,
        ValidIssuer = jwtIssuer,
        ValidAudience = jwtAudience,
        IssuerSigningKey = new SymmetricSecurityKey (Encoding.UTF8.GetBytes(jwtKey))
    };
});

builder.Services.AddAuthorization();
builder.Services.AddScoped<IEmailVerificationSender, DevelopmentEmailVerificationSender>();
builder.Services.AddScoped<IAuthService, AuthService>();


//Registering Services
builder.Services.AddScoped<IResumeService, ResumeService>();
builder.Services.AddScoped<IExperienceService, ExperienceService>();
builder.Services.AddScoped<ISkillService, SkillService>();
builder.Services.AddScoped<IProjectService, ProjectService>();
builder.Services.AddScoped<ICertificationService, CertificationService>();
builder.Services.AddScoped<IEducationService, EducationService>();
builder.Services.AddScoped<IInterestService, InterestService>();
builder.Services.AddScoped<IRefereeService, RefereeService>();


// Add services to the Db.
builder.Services.AddDbContext<ResumeDbContext>(options =>
    options.UseSqlServer(builder.Configuration.GetConnectionString("DefaultConnection")));

//Redis configuration for distributed caching.
//This allows the application to store and retrieve data from a Redis cache,
//which can improve performance and scalability, especially in distributed environments..

// Get the Redis connection string from configuration.
// In Docker Compose this comes from:
// Redis__ConnectionString=redis:6379
var redisConnectionString = builder.Configuration["Redis:ConnectionString"];

if (!string.IsNullOrWhiteSpace(redisConnectionString))
{
    // Use Redis as the distributed cache when Redis is configured.
    builder.Services.AddStackExchangeRedisCache(options =>
    {
        options.Configuration = redisConnectionString;

        // Prefix our Redis keys so they are clearly owned by ResumeApp.
        options.InstanceName = "ResumeApp:";
    });
}
else
{
    // When running without Docker/Redis,
    // use an in-memory cache so the application can still start normally.
    builder.Services.AddDistributedMemoryCache();
}



//Swagger connection/documentation/Authentication configuration
builder.Services.AddEndpointsApiExplorer();

builder.Services.AddSwaggerGen(options =>
{

    options.SwaggerDoc("v1", new OpenApiInfo

    {
        Title = "ResumeApp API",
        Version = "v1"
    });

    options.AddSecurityDefinition("bearer", new OpenApiSecurityScheme
    {
        Name = "Authorization",
        Type = SecuritySchemeType.Http,
        Scheme = "bearer",
        BearerFormat = "JWT",
        In = ParameterLocation.Header,
        Description = "Enter: bearer {your JWT token}"
    });

    options.AddSecurityRequirement(document => new OpenApiSecurityRequirement
    {
        [new OpenApiSecuritySchemeReference("bearer", document)] = []
    });
});
var app = builder.Build();


//Apply migrations automatically if the configuration setting is enabled.
//This is useful for development and testing environments, but in production,
//you might want to handle migrations manually to avoid unexpected changes to the database schema.

if (builder.Configuration.GetValue<bool>("Database:ApplyMigrations"))
{
    using var scope = app.Services.CreateScope();

    var dbContext =
        scope.ServiceProvider.GetRequiredService<ResumeDbContext>();

    await dbContext.Database.MigrateAsync();
}


// Serves the React build and supports frontend routes.
    app.UseDefaultFiles(); // Must be here for publishing
    app.MapStaticAssets(); // Must be here for publishing

// Configure the HTTP request pipeline.
if (app.Environment.IsDevelopment())
{
    app.UseSwagger();
    app.UseSwaggerUI();
}


//Middleware section
app.UseHttpsRedirection();

app.UseCors();

app.UseAuthentication();
app.UseAuthorization();


app.MapControllers();


// Keep this last so API and Swagger routes are matched first.
app.MapFallbackToFile("/index.html"); // Must be here for publishing

app.Run();

public partial class Program { } // For integration API testing
