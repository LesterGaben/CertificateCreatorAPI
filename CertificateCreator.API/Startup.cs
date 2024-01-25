using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Hosting;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using Microsoft.EntityFrameworkCore;
using System.Reflection;
using CertificateCreator.BLL.MappingProfiles;
using CertificateCreator.BLL.Services;
using CertificateCreator.BLL.Services.Interfaces;
using CertificateCreator.DAL.Context;
using CertificateCreator.DAL.Repositories;
using CertificateCreator.DAL.Repositories.Interfaces;


namespace CertificateCreator.API {
    public class Startup {
        public Startup(IConfiguration configuration) {
            Configuration = configuration;
        }

        public IConfiguration Configuration { get; }

        public void ConfigureServices(IServiceCollection services) {
            services.AddControllers();
            services.AddEndpointsApiExplorer();
            services.AddSwaggerGen();

            services.AddDbContext<CertificateCreatorContext>(options =>
                options.UseSqlServer(Configuration.GetConnectionString("CertificateCreatorDB"))
                       .LogTo(Console.WriteLine, LogLevel.Information));

            services.AddScoped<ICertificateService, CertificateService>();
            services.AddScoped<IConvertCertificateToPDFService, ConvertCertificateToPDFService>();
            services.AddScoped<ICertificateRepository, CertificateRepository>();
            services.AddAutoMapper(Assembly.GetAssembly(typeof(CertificateProfile)));

            services.AddCors(options => {
                options.AddPolicy("MyAllowSpecificOrigins",
                    builder => {
                        builder.WithOrigins("http://localhost:4200")
                               .AllowAnyHeader()
                               .AllowAnyMethod();
                    });
            });
        }

        public void Configure(IApplicationBuilder app, IWebHostEnvironment env) {
            if (env.IsDevelopment()) {
                app.UseSwagger();
                app.UseSwaggerUI();
            }

            app.UseHttpsRedirection();

            app.UseCors("MyAllowSpecificOrigins");

            app.UseRouting();

            app.UseAuthorization();

            app.UseEndpoints(endpoints =>
            {
                endpoints.MapControllers();
            });
        }
    }

}
