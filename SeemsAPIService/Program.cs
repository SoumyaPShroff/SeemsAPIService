using Microsoft.EntityFrameworkCore;
using SeemsAPIService.Application.DTOs;
using SeemsAPIService.Application.Interfaces;
using SeemsAPIService.Application.Mapper;
using SeemsAPIService.Application.Mapper.SeemsAPIService.Application.Mapper;
using SeemsAPIService.Application.Services;
using SeemsAPIService.Domain.Entities;
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
builder.Services.AddScoped<IUserQueryService, UserQueryService>();
builder.Services.AddScoped<IEmailService, EmailService>();
builder.Services.AddScoped<IEmailRecipientService, EmailRecipientService>();
builder.Services.AddScoped<IUserAccessService, UserAccessService>();
builder.Services.AddScoped<IJobService, JobService>();
builder.Services.AddScoped<ICommonQueryService, CommonQueryService>();
builder.Services.AddScoped<ISalesService, SalesService>();                           // Sales service dependency injection
builder.Services.AddScoped<IReusableService, ReusableService>();
builder.Services.AddScoped<IReportService, ReportService>();

builder.Services.AddScoped<IUserQueryRepository, UserQueryRepository>();
builder.Services.AddScoped<ICommonQueryRepository, CommonQueryRepository>();
builder.Services.AddScoped<IReusableRepository, ReusableRepository>();               // Repository dependency injection
builder.Services.AddScoped<ISalesRepository, SalesRepository>();
builder.Services.AddScoped<IReportRepository, ReportRepository>();
builder.Services.AddScoped<IAuthRepository, AuthRepository>();
builder.Services.AddScoped<IJobRepository, JobRepository>();
builder.Services.AddScoped<IUserAccessRepository, UserAccessRepository>();
builder.Services.AddScoped<IEntityMapper<EnquiryDto, se_enquiry, string?>, EnquiryMapper>(); //mapper dependency injection
builder.Services.AddScoped<IEntityMapper<QuotationDto, se_quotation, string?>, QuotationMapper>();

builder.Services.AddCors(options =>
{
    options.AddPolicy("AllowAll",
        policy => policy.AllowAnyOrigin().AllowAnyMethod().AllowAnyHeader());
});

builder.Services.AddHttpClient<IEmailService, EmailService>();    // Email service dependency injection
var app = builder.Build();          //follow order from this line, or else app breaks for CORS policy

//capture exception
if (app.Environment.IsDevelopment())
{
    app.UseDeveloperExceptionPage();   // shows detailed error in browser
}
else
{
    app.UseExceptionHandler("/error"); // generic error handler in prod
}

// Configure the HTTP request pipeline.
if (app.Environment.IsDevelopment() || app.Environment.IsProduction())
{
    app.UseSwagger();
    app.UseSwaggerUI();
}

app.UseHttpsRedirection();

app.UseRouting();

app.UseCors("AllowAll");

app.UseAuthorization();

// Use CORS policy
app.UseCors("AllowViteApp"); //already declared above

app.UseDefaultFiles();   // serve index.html by default
app.UseStaticFiles();    // serve files from wwwroot

app.MapControllers();
// Fallback to index.html for React Router
app.MapFallbackToFile("index.html");

app.Run();
