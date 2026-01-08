using Microsoft.EntityFrameworkCore;
using SeemsAPIService.Application.Interfaces;
using SeemsAPIService.Application.Services;
using SeemsAPIService.Infrastructure.ExternalServices;
using SeemsAPIService.Infrastructure.Persistence;
using SeemsAPIService.Infrastructure.Persistence.Repository;
using SeemsAPIService.Infrastructure.Repositories;


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

builder.Services.AddScoped<IAuthService, AuthService>();
builder.Services.AddScoped<IAuthRepository, AuthRepository>();
builder.Services.AddScoped<IEmailService, EmailService>();
builder.Services.AddScoped<IEmailRecipientService, EmailRecipientService>();

builder.Services.AddScoped<IJobService, JobService>();
builder.Services.AddScoped<IJobRepository, JobRepository>();
builder.Services.AddScoped<IUserAccessService, UserAccessService>();
builder.Services.AddScoped<IUserAccessRepository, UserAccessRepository>();
builder.Services.AddScoped<IUserQueryService, UserQueryService>();
builder.Services.AddScoped<IUserQueryRepository, UserQueryRepository>();
builder.Services.AddScoped<ICommonQueryService, CommonQueryService>();
builder.Services.AddScoped<ICommonQueryRepository, CommonQueryRepository>();

builder.Services.AddScoped<ISalesService, SalesService>();                           // Sales service dependency injection
//builder.Services.AddScoped<IQuotationService, QuotationService>();                  // Quotation service dependency injection
builder.Services.AddScoped<IReusableService, ReusableService>();                     // Application service dependency injection
builder.Services.AddScoped<IReusableRepository, ReusableRepository>();               // Repository dependency injection
builder.Services.AddScoped<ISalesRepository, SalesRepository>();
builder.Services.AddScoped<ISalesService, SalesService>();
builder.Services.AddScoped<IReportRepository, ReportRepository>();
builder.Services.AddScoped<IReportService, ReportService>();

builder.Services.AddCors(options =>
{
    options.AddPolicy("AllowAll",
        policy => policy.AllowAnyOrigin().AllowAnyMethod().AllowAnyHeader());
});

builder.Services.AddHttpClient<IEmailService, EmailService>();                    // Email service dependency injection
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
