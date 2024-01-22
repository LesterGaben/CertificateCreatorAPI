using CertificateCreator.BLL.MappingProfiles;
using CertificateCreator.BLL.Services;
using CertificateCreator.BLL.Services.Interfaces;
using CertificateCreator.DAL.Context;
using CertificateCreator.DAL.Repositories;
using CertificateCreator.DAL.Repositories.Interfaces;
using Microsoft.EntityFrameworkCore;
using System.Reflection;

var builder = WebApplication.CreateBuilder(args);

// Додавання служб
builder.Services.AddControllers();
builder.Services.AddEndpointsApiExplorer();
builder.Services.AddSwaggerGen();

//string connectionString = "Server=localhost;Database=CertificateCreatorDB;Trusted_Connection=True;TrustServerCertificate=True;";

builder.Services.AddDbContext<CertificateCreatorContext>(options =>
    options.UseSqlServer(builder.Configuration.GetConnectionString("CertificateCreatorDB"))
           .LogTo(Console.WriteLine, LogLevel.Information));

builder.Services.AddScoped<ICertificateService, CertificateService>();
builder.Services.AddScoped<IConvertCertificateToPDFService, ConvertCertificateToPDFService>();
builder.Services.AddScoped<ICertificateRepository, CertificateRepository>();
builder.Services.AddAutoMapper(Assembly.GetAssembly(typeof(CertificateProfile)));


// Додавання та налаштування CORS
builder.Services.AddCors(options => {
    options.AddPolicy("MyAllowSpecificOrigins",
        builder => {
            builder.WithOrigins("http://localhost:4200") // Тут вкажіть джерела, які потрібно дозволити
                   .AllowAnyHeader()
                   .AllowAnyMethod();
        });
});

var app = builder.Build();

// Налаштування конвеєра HTTP-запитів
if (app.Environment.IsDevelopment()) {
    app.UseSwagger();
    app.UseSwaggerUI();
}

app.UseHttpsRedirection();

app.UseCors("MyAllowSpecificOrigins"); // Використання CORS політики

app.UseRouting(); // Додайте цей рядок

app.UseAuthorization();

app.MapControllers();

app.Run();
