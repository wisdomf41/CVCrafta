using Microsoft.EntityFrameworkCore;
using ResumeApp.Server.Data;

var builder = WebApplication.CreateBuilder(args);

// Add services to the Db.
builder.Services.AddDbContext<ResumeDbContext>(options =>
    options.UseSqlServer(builder.Configuration.GetConnectionString("DefaultConnection")));

//Swagger connection
builder.Services.AddEndpointsApiExplorer();
builder.Services.AddSwaggerGen();

builder.Services.AddControllers();
// Learn more about configuring OpenAPI at https://aka.ms/aspnet/openapi
builder.Services.AddOpenApi();

var app = builder.Build();

app.UseDefaultFiles(); // Must be here for publishing
app.MapStaticAssets(); // Must be here for publishing

// Configure the HTTP request pipeline.
if (app.Environment.IsDevelopment())
{
    app.UseSwagger();
    app.UseSwaggerUI();
}

app.UseHttpsRedirection();

app.UseAuthorization();

app.MapControllers();

app.MapFallbackToFile("/index.html"); // Must be here for publishing

app.Run();
