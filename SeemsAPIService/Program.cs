using Microsoft.EntityFrameworkCore;
using SeemsAPIService.Application.Services;
using SeemsAPIService.Infrastructure.Persistence;

var builder = WebApplication.CreateBuilder(args);

// ✅ Add DbContext and inject DatabaseConfig too
builder.Services.AddDbContext<AppDbContext>((serviceProvider, options) =>
{
    var connectionString = builder.Configuration.GetConnectionString("DefaultConnection");
    options.UseMySql(connectionString, ServerVersion.AutoDetect(connectionString));
});

builder.Services.AddControllers();
// Learn more about configuring Swagger/OpenAPI at https://aka.ms/aspnetcore/swashbuckle
builder.Services.AddEndpointsApiExplorer();
builder.Services.AddSwaggerGen();
builder.Services.AddScoped<IReusableServices, ReusableServices>(); //for resuable services which can be used through out app irrespective of injecting controllers

builder.Services.AddCors(options =>
{
    options.AddPolicy("AllowAll",
        policy => policy.AllowAnyOrigin().AllowAnyMethod().AllowAnyHeader());
});

// inject email service
builder.Services.AddHttpClient<EmailTriggerService>();

var app = builder.Build();
app.UseCors("AllowAll");

// Configure the HTTP request pipeline.
if (app.Environment.IsDevelopment() || app.Environment.IsProduction())
{
    app.UseSwagger();
    app.UseSwaggerUI();
}

app.UseHttpsRedirection();

app.UseAuthorization();

// Use CORS policy
app.UseCors("AllowViteApp");

app.UseDefaultFiles();   // serve index.html by default
app.UseStaticFiles();    // serve files from wwwroot

app.MapControllers();

// Fallback to index.html for React Router
app.MapFallbackToFile("index.html");

app.Run();
